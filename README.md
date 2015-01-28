# Overview
Simple app written in C# VS 2013 that targets x86 (32-bit) platform to specifically showcase that both `AerospikeClient.dll` built for x86 (32-bit) target and external application referencing the `.dll` can sucessfully run aggregations on top of the Aerospike DB. 

This is how Aerospike C# Client was built for x86 (32-bit) platform:

- Downloaded it from [here](http://www.aerospike.com/download/client/csharp/3.0.12/)
- Opened **AerospikeClient.sln** in Visual Studio 2013
- Changed the following settings via `Project` > `AerospikeClient Properties`
    + Configuration: **Active (Release)**
    + Platform: **Active (x86)** 
    + Platform target: **x86**
- Ran `Build` > `Build Solution` &mdash; this generated `AerospikeClient.dll` in `<local-client-download-folder>\bin\x86\Release` folder

Then, in this (ASConsoleApp) app, reference to the generated `AerospikeClient.dll` was added via `Project` > `Add Reference...`

***Imp To Note:*** This repo contains source code for the referencing application (ASConsoleApp) which already has a reference to `AerospikeClient.dll` built for x86 (32-bit).

# Aggregation In Action 

## Prerequisites

- [Aerospike Server](http://www.aerospike.com/download/server/latest) – It should be running and accessible from this app
- Visual Studio 2013

## Build

- Download, clone or fork this repo -- bascially get the code one way or another :)
- Open **ASConsoleApp.sln** in VS 2013
- In **Program.cs**, update ***asServerIP*** and ***asServerPort*** such that it points to your instance running the Aerospike Server
- Build and Start the application

If all goes well, you should message **"INFO: Connection to Aerospike cluster succeeded!"** along with a menu presenting couple of options. See Usage.

## Usage

- First option:
  - Create 10,000 dummy user records with randomly selected ***region*** ('n', 'e', 'w', 's') and a ***tweetcount*** of up to 20. NOTE: You should definitely do this the first time you run the app.
- Second option:
  - Create `secondary index` on ***tweetcount*** 
  -  Register streaming UDF `aggregationByRegion`
  -  Execute range query on ***tweetcount***
  -  Run aggregation on the range query resultset 
  -  Output aggregation result
  
## Takeaway

Anything is possible :)

