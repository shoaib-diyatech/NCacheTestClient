namespace NCacheClient;

using Alachisoft.NCache.Licensing.DOM;
using Alachisoft.NCache.Runtime.Dependencies;
using NCacheTestClient.NCacheClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Events;

public class DependencyClientOleDbPolling : NCache
{
    private IConfiguration _configuration;

    private string _connectionString;

    // Update your constructors to accept IConfiguration and assign it
    public DependencyClientOleDbPolling(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
        //_configuration = configuration;
    }
    public DependencyClientOleDbPolling(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
        //_configuration = configuration;
    }

    private void setConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        _connectionString = _configuration["ConnectionString"];
        log.Debug($"connectionString in DependencyClientOleDbPolling: {_connectionString}");
    }
    public override void Test()
    {
        setConfiguration();
        string key = "1001";
        Subscriber sub = GetFromDatabase(key);
        TestDependency(sub);
        TestIfItemExistsInCache(key);
    }

    public void TestIfItemExistsInCache(string key)
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            Console.WriteLine("Press Enter to check cache, ESC to exit...");
            keyInfo = Console.ReadKey(intercept: true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                bool exists = Contains(key);
                Console.WriteLine(exists
                    ? $"Key '{key}' exists in cache."
                    : $"Key '{key}' [[[DOES NOT]]] exist in cache.");
            }
        } while (keyInfo.Key != ConsoleKey.Escape);
    }

    public Subscriber GetFromDatabase(string msisdn)
    {
        DataLayer dl = new DataLayer();
        dl.Connect();
        bool isConnected = dl.IsConnected;
        if (isConnected)
        {
            log.Debug("Connected to database successfully.");
            Subscriber sub = dl.GetData(msisdn);
            log.Info($"Subscriber: {sub.Name}, {sub.Msisdn}, {sub.Email}, {sub.IsActive}, {sub.DateOfBirthString}");
            return sub;
        }
        else
        {
            log.Error("Failed to connect to database.");
            return null;
        }
    }

    public void TestDependency(Subscriber sub)
    {
        // Precondition: Cache is already connected
        // Get product from database against given product ID
        //Subscriber sub = GetFromDatabase("1000");

        // Generate a unique cache key for this subscriber
        string key = sub.Msisdn;

        // Create a connection string to establish connection with the database
        // Connection String is in [AppSettings] in App.config


        // Replace the line in TestDependency method:
        string connectionString = _configuration["connectionstring"]+ "Provider=SQLOLEDB";

        log.Debug($"Connection string: {connectionString}");

        // Creating Polling based dependency
        DBCacheDependency oledbDependency = DBDependencyFactory.CreateOleDbCacheDependency(connectionString, key);

        // Create a new cacheitem and add oledb dependency to it
        var cacheItem = new CacheItem(sub);
        cacheItem.Dependency = oledbDependency;
        cacheItem.SetCacheDataNotification(CacheDataNotificationCallback, EventType.ItemRemoved, EventDataFilter.DataWithMetadata);

        // Add cache item in the cache with OleDb Dependency
        cache.Insert(key, cacheItem);
        log.Info($"Added item with key: {key} with polling dependency");
        //System.Threading.Thread.Sleep(5000);
        // For successful addition of item with OleDb Dependency
        // Update the record in the database and check if key is present
    }

    public void CacheDataNotificationCallback(string key, CacheEventArg cacheEventArgs)
    {
        log.Debug($"{key} is {cacheEventArgs.EventType}ed due to {cacheEventArgs.CacheItemRemovedReason}");
    }
}

