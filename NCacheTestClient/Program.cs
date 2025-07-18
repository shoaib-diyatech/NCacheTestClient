﻿// namespace NcacheClient;

using log4net;
using log4net.Config;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCacheClient;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.CacheManagement;

// Initialize log4net
#region log4net
var entryAssembly = Assembly.GetEntryAssembly();
if (entryAssembly == null)
{
    throw new InvalidOperationException("Entry assembly is null.");
}
var logRepository = LogManager.GetRepository(entryAssembly);

FileInfo logConfFile = new FileInfo("log4net.config");
if (!logConfFile.Exists)
{
    throw new FileNotFoundException("log4net.config file not found.", logConfFile.FullName);
}
XmlConfigurator.Configure(logRepository, logConfFile);

// Checking if log4net is configured correctly
if (!logRepository.Configured)
{
    throw new InvalidOperationException("log4net configuration failed.");
}
else{
    Console.WriteLine("log4net configured successfully.");
}
#endregion

// Build configuration
#region Build configuration
//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //.Build();

// Configure services
var services = new ServiceCollection();
//services.Configure<NCacheServerSettings>(options => configuration.GetSection("NCacheServerSettings"));
var serviceProvider = services.BuildServiceProvider();

// Access configuration values
//var someSetting = configuration["SomeSetting"];
//var serverIP = configuration["NCacheServerSettings:ServerIP"];
//var serverPort = configuration["NCacheServerSettings:ServerPort"];
//var nCacheServerSettings = serviceProvider.GetService<IOptions<NCacheServerSettings>>().Value;
#endregion

int port = 9080;// int.Parse(serverPort);


// List<string> serverIps = new List<string> { serverIp1, serverIp2, serverIp3 };
List<string> serverIps = new List<string> {
    // "20.200.20.32" 
    //, 
    //  "20.200.20.42"
    //, 
    "20.200.20.103"
    };
port = 9800;

Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
//Console.WriteLine($"SomeSetting: [{someSetting}]");
//Console.WriteLine($"NCache Server IP: {nCacheServerSettings.ServerIP}");
//Console.WriteLine($"NCache Server Port: {nCacheServerSettings.ServerPort}");
Console.WriteLine($"serverIps: [{string.Join(", ", serverIps)}]");
//Console.WriteLine($"serverPort: [{serverPort}]");

// string CacheName = "RemoteMirror";
string CacheName = "demoCache"; //"HomePart";// "SNCache"; // "InProcCache";
//string CacheName = "TestMirror2";

Console.WriteLine($"Cache: [{CacheName}]");

Alachisoft.NCache.Runtime.CacheManagement.CacheHealth cacheHealth = CacheManager.GetCacheHealth("HomePart");
Console.WriteLine($"Cache Health: cacheHealth.ServerNodesStatus: [{cacheHealth.ServerNodesStatus}], cacheHealth.Status: [{cacheHealth.Status}]");


//NCache nCacheClient = new EventClient(serverIps, port, CacheName); // Just registering the events
// NCache nCacheClient = new BulkClient(serverIps, port, CacheName)
// NCache nCacheClient = new PubSubClient(serverIps, port, CacheName);
// NCache nCacheClient = new PartitionClient(serverIps, port, CacheName);
// NCache nCacheClient = new LockingClient(serverIps, port, CacheName);
//  NCache nCacheClient = new GroupClient(serverIps, port, CacheName);
//  NCache nCacheClient = new TagClient(serverIps, port, CacheName);
// NCache nCacheClient = new DependencyClient(serverIps, port, CacheName);
CacheThrough nCacheClient = new CacheThrough(serverIps, port, CacheName);
// NCache nCacheClient = new InProcClient(CacheName);
// NCache nCacheClient = new AsyncClient(serverIps, port, CacheName);
//NCache nCacheClient = new DependencyClientOleDbPolling(serverIps, port, CacheName);

nCacheClient.Initialize();
nCacheClient.Test();
Console.ReadLine();

//ICache cache = CacheManager.GetCache("democache");

// CacheLoaderTest cacheLoaderTest = new CacheLoaderTest();
// cacheLoaderTest.Test();

Console.ReadLine();
Console.ReadLine();


