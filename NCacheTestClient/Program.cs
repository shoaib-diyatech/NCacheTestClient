// namespace NcacheClient;

using log4net;
using log4net.Config;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCacheClient;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.CacheManagement;
using NCacheTestClient.Util;
using static System.Net.WebRequestMethods;

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

int port;// = 9080;// int.Parse(serverPort);


// List<string> serverIps = new List<string> { serverIp1, serverIp2, serverIp3 };
List<string> serverIps = new List<string> {
    // "20.200.20.32" 
    //, 
    //  "20.200.20.42"
    //, 
    "20.200.20.103"
    };
port = 9800;

List<string> serverIps2 = new List<string>{
    //"20.200.20.24" // Ayesha's PC
    "20.200.20.103"
    };

Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
//Console.WriteLine($"SomeSetting: [{someSetting}]");
//Console.WriteLine($"NCache Server IP: {nCacheServerSettings.ServerIP}");
//Console.WriteLine($"NCache Server Port: {nCacheServerSettings.ServerPort}");
Console.WriteLine($"serverIps: [{string.Join(", ", serverIps)}]");
//Console.WriteLine($"serverPort: [{serverPort}]");

// string CacheName = "RemoteMirror";
string CacheName = "demoCache"; //"HomePart";// "SNCache"; // "InProcCache";
string CacheName2 = "demoCache2";
//string CacheName = "TestMirror2";

Console.WriteLine($"Cache1: [{CacheName}]");
Console.WriteLine($"Cache2: [{CacheName2}]");

//Alachisoft.NCache.Runtime.CacheManagement.CacheHealth cacheHealth = CacheManager.GetCacheHealth(CacheName);
//Console.WriteLine($"Cache Health: cacheHealth.ServerNodesStatus: [{cacheHealth.ServerNodesStatus}], cacheHealth.Status: [{cacheHealth.Status}]");


//NCache nCacheClient = new EventClient(serverIps, port, CacheName); // Just registering the events
// NCache nCacheClient = new BulkClient(serverIps, port, CacheName)
// NCache nCacheClient = new PubSubClient(serverIps, port, CacheName);
// NCache nCacheClient = new PartitionClient(serverIps, port, CacheName);
// NCache nCacheClient = new LockingClient(serverIps, port, CacheName);
//  NCache nCacheClient = new GroupClient(serverIps, port, CacheName);
//  NCache nCacheClient = new TagClient(serverIps, port, CacheName);
// NCache nCacheClient = new DependencyClient(serverIps, port, CacheName);
// CacheThrough nCacheClient = new CacheThrough(serverIps, port, CacheName);
// NCache nCacheClient = new InProcClient(CacheName);
// NCache nCacheClient = new AsyncClient(serverIps, port, CacheName);
//NCache nCacheClient = new DependencyClientOleDbPolling(serverIps, port, CacheName);
 //NCache nCacheClient = new SimpleClient(serverIps, port, CacheName);
 NCache nCacheClient = new SimpleClient(serverIps, port, CacheName);
 NCache nCacheClient2 = new SimpleClient(serverIps2, port, CacheName2);

nCacheClient.Initialize();
nCacheClient2.Initialize();
((SimpleClient)nCacheClient).PopulateRandom(10);
((SimpleClient)nCacheClient2).PopulateRandom(10);
Console.ReadLine();
//(((SimpleClient)nCacheClient).GetCacheObj()).Remove("5")
CacheDataComparer.CompareAndReportCacheDifferences(((SimpleClient)nCacheClient).GetCacheObj(), ((SimpleClient)nCacheClient2).GetCacheObj());
Console.ReadLine();

//ICache cache = CacheManager.GetCache("democache");

// CacheLoaderTest cacheLoaderTest = new CacheLoaderTest();
// cacheLoaderTest.Test();

Console.ReadLine();
Console.ReadLine();


