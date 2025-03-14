// namespace NcacheClient;

using log4net;
using log4net.Config;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCacheClient;

// Initialize log4net
#region log4net
var entryAssembly = Assembly.GetEntryAssembly();
if (entryAssembly == null)
{
    throw new InvalidOperationException("Entry assembly is null.");
}
var logRepository = LogManager.GetRepository(entryAssembly);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
#endregion

// Build configuration
#region Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Configure services
var services = new ServiceCollection();
services.Configure<NCacheServerSettings>(options => configuration.GetSection("NCacheServerSettings"));
var serviceProvider = services.BuildServiceProvider();

// Access configuration values
var someSetting = configuration["SomeSetting"];
var serverIP = configuration["NCacheServerSettings:ServerIP"];
var serverPort = configuration["NCacheServerSettings:ServerPort"];
var nCacheServerSettings = serviceProvider.GetService<IOptions<NCacheServerSettings>>().Value;
#endregion

int port = int.Parse(serverPort);


// List<string> serverIps = new List<string> { serverIp1, serverIp2, serverIp3 };
List<string> serverIps = new List<string> {
    // "20.200.20.32" 
    //, 
    "20.200.20.42"
    //, "20.200.20.103"
    };
port = 9800;

Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
Console.WriteLine($"SomeSetting: [{someSetting}]");
//Console.WriteLine($"NCache Server IP: {nCacheServerSettings.ServerIP}");
//Console.WriteLine($"NCache Server Port: {nCacheServerSettings.ServerPort}");
Console.WriteLine($"serverIps: [{string.Join(", ", serverIps)}]");
Console.WriteLine($"serverPort: [{serverPort}]");

// string CacheName = "EventsCluster";
string CacheName = "SKOnly";
// string CacheName = "TestMirrorCache";

Console.WriteLine($"Cache: [{CacheName}]");

//NCache nCacheEventClient = new EventClient(serverIps, port, CacheName); // Just registering the events
// NCache nCacheClient = new BulkClient(serverIps, port, CacheName)
//NCache nCacheClient = new PubSubClient(serverIps, port, CacheName);
// NCache nCacheClient = new PartitionClient(serverIps, port, CacheName);
// NCache nCacheClient = new LockingClient(serverIps, port, CacheName);
 //NCache nCacheClient = new GroupClient(serverIps, port, CacheName);
 NCache nCacheClient = new TagClient(serverIps, port, CacheName);
//NCache nCacheClient = new DependencyClient(serverIps, port, CacheName);

nCacheClient.Initialize();
nCacheClient.Test();
Console.ReadLine();


Console.ReadLine();
Console.ReadLine();


