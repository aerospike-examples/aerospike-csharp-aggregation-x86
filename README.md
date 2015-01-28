# Overview
Simple app written in C# VS 2013 to specifically showcase that both `AerospikeClient.dll` built for **x86 (32-bit)** target and external application referencing the `.dll` can successfully run aggregations on top of the Aerospike DB. 

## Build Aerospike C# Client

This is how you should build Aerospike C# Client for **x86 (32-bit)** platform to be used with this app:

- Download it from [here](http://www.aerospike.com/download/client/csharp/3.0.12/)
- Open **AerospikeClient.sln** in Visual Studio 2013
- Change the following settings via `Project` > `AerospikeClient Properties`
    + Configuration: **Active (Release)**
    + Platform: **Active (x86)** 
    + Platform target: **x86**
- Run `Build` > `Build Solution` &mdash; this should generate `AerospikeClient.dll` in `<local-client-download-folder>\bin\x86\Release` folder

***Keep Note Of:*** `AerospikeClient.dll` in `<local-client-download-folder>\bin\x86\Release` folder

# Aggregation In Action 

Follow these steps to run this application and see aggregation runnning on x86 (32-bit) target.

## Prerequisites

- [Aerospike Server](http://www.aerospike.com/download/server/latest) – It should be running and accessible from this app
- Visual Studio 2013

## Build

- Download, clone or fork this repo -- basicially get the code one way or another :)
- Open **ASConsoleApp.sln** in VS 2013
- Add reference to generated `AerospikeClient.dll` located in `<local-client-download-folder>\bin\x86\Release` folder via `Project` > `Add Reference...`
- In [**Program.cs**](https://github.com/aerospike/aerospike-csharp-aggregation-x86/blob/master/ASConsoleApp/Program.cs), update ***asServerIP*** and ***asServerPort*** such that it points to your instance running the Aerospike Server
- Build and Start the application

If all goes well, you should message **"INFO: Connection to Aerospike cluster succeeded!"** along with a menu presenting couple of options. See Usage.

## Usage

- First option:
  - Create 10,000 dummy user records with randomly selected ***region*** ('n', 'e', 'w', 's') and a ***tweetcount*** of up to 20. NOTE: You should definitely do this the first time you run the app.
- Second option:
  - Create `secondary index` on ***tweetcount*** 
  - Register streaming UDF [`aggregationByRegion`](https://github.com/aerospike/aerospike-csharp-aggregation-x86/blob/master/ASConsoleApp/udf/aggregationByRegion.lua)
  - Execute range query on ***tweetcount***
  - Run aggregation on the range query resultset 
  - Output aggregation result to the console
  
## Takeaway

Anything is possible :)

![Check This Out](/ASConsoleApp/app_console.png?raw=true)
