using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aerospike.Client;
using System.IO;

namespace ASConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AerospikeClient client = null;
            try
            {
                Console.WriteLine("INFO: Connecting to Aerospike cluster...");

                // Connecting to Aerospike cluster

                // Specify Aerospike server IP of one of the nodes in the cluster
                string asServerIP = "127.0.0.0";
                // Specity Port that the node is listening on
                int asServerPort = 3000;
                // Establish connection
                client = new AerospikeClient(asServerIP, asServerPort);

                // Check to see if the cluster connection succeeded
                if (client.Connected)
                {
                    Console.WriteLine("INFO: Connection to Aerospike cluster succeeded!\n");

                    Program app = new Program();

                    // Present options
                    Console.WriteLine("What would you like to do:");
                    Console.WriteLine("1> Create Users");
                    Console.WriteLine("2> Aggregate Based on Tweet Count By Region");
                    Console.WriteLine("0> Exit");
                    Console.Write("\nSelect 0-2 and hit enter:");
                    byte feature = byte.Parse(Console.ReadLine());

                    if (feature != 0)
                    {
                        switch (feature)
                        {
                            case 1:
                                Console.WriteLine("\n********** Create Users **********\n");
                                app.createUsers(client);
                                break;
                            case 2:
                                Console.WriteLine("\n********** Aggregate Users **********\n");
                                app.aggregateUsersByTweetCountByRegion(client);
                                break;
                            default:
                                break;
                        }
                    }
                    
                }
                else
                {
                    Console.Write("ERROR: Connection to Aerospike cluster failed! Please check IP & Port settings and try again!");
                    Console.ReadLine();
                }
            }
            catch (AerospikeException e)
            {
                Console.WriteLine("AerospikeException - Message: " + e.Message);
                Console.WriteLine("AerospikeException - StackTrace: " + e.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception - Message: " + e.Message);
                Console.WriteLine("Exception - StackTrace: " + e.StackTrace);
            }
            finally
            {
                if (client != null && client.Connected)
                {
                    // Close Aerospike server connection
                    client.Close();
                }
                Console.Write("\n\nINFO: Press any key to exit...");
                Console.ReadLine();
            }

        } //main

        public void aggregateUsersByTweetCountByRegion(AerospikeClient client)
        {
            ResultSet rs = null;
            try
            {
                int min;
                int max;
                Console.WriteLine("\nEnter Min Tweet Count:");
                min = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Max Tweet Count:");
                max = int.Parse(Console.ReadLine());

                Console.WriteLine("\nAggregating users with " + min + "-" + max + " tweets by region. Hang on...\n");

                // NOTE: Index creation has been included in here for convenience and to demonstrate the syntax. 
                // NOTE: The recommended way of creating indexes in production env is via AQL.
                IndexTask task = client.CreateIndex(null, "test", "testusers", "tweetcountindex", "tweetcount", IndexType.NUMERIC);
                task.Wait();

                // NOTE: UDF registration has been included here for convenience and to demonstrate the syntax. 
                // NOTE: The recommended way of registering UDFs in production env is via AQL
                string luaDirectory = @"..\..\udf";
                LuaConfig.PackagePath = luaDirectory + @"\?.lua";

                string filename = "aggregationByRegion.lua";
                string path = Path.Combine(luaDirectory, filename);

                RegisterTask rt = client.Register(null, path, filename, Language.LUA);
                rt.Wait();

                Statement stmt = new Statement();
                stmt.SetNamespace("test");
                stmt.SetSetName("testusers");
                stmt.SetIndexName("tweetcountindex");
                stmt.SetFilters(Filter.Range("tweetCount", min, max));

                rs = client.QueryAggregate(null, stmt, "aggregationByRegion", "sum");

                if (rs.Next())
                {
                    Dictionary<object, object> result = (Dictionary<object, object>)rs.Object;

                    Console.WriteLine("Here's the breakdown...\n");
                    Console.WriteLine("Total Users in North: " + result["n"]);
                    Console.WriteLine("Total Users in South: " + result["s"]);
                    Console.WriteLine("Total Users in East: " + result["e"]);
                    Console.WriteLine("Total Users in West: " + result["w"]);
                }
            }
            finally
            {
                if (rs != null)
                {
                    // Close record set
                    rs.Close();
                }
            }
        } //aggregateUsersByTweetCountByRegion

        public void createUsers(AerospikeClient client)
        {
            string[] regions = { "n", "s", "e", "w" };
            string username;
            int start = 0;
            int end = 10000;
            int totalUsers = end - start;
            Random rnd1 = new Random();
            Random rnd2 = new Random();
            Random rnd3 = new Random();

            WritePolicy wPolicy = new WritePolicy();
            wPolicy.recordExistsAction = RecordExistsAction.UPDATE;

            Console.WriteLine("\nCreate " + totalUsers + " users in set 'testusers'. Press any key to continue...");
            Console.ReadLine();

            for (int j = start; j <= end; j++)
            {
                // Write user record
                username = "user" + j;
                Key key = new Key("test", "testusers", username);
                Bin bin1 = new Bin("username", "user" + j);
                Bin bin2 = new Bin("region", regions[rnd2.Next(0, 4)]);
                Bin bin3 = new Bin("tweetcount", rnd3.Next(0, 20));

                client.Put(wPolicy, key, bin1, bin2, bin3);

                Console.WriteLine("\nCreated " + username);
            }

            Console.WriteLine("\nDone creating " + totalUsers + "!");
        } //createUsers

    }
}
