namespace NCacheClient;

using Alachisoft.NCache.Runtime.Dependencies;
using Alachisoft.NCache.Caching.CacheSynchronization;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Caching;
using Alachisoft.NCache.Common.Monitoring;
using Alachisoft.NCache.Common.Util;
using Alachisoft.NCache.Config.Dom;
using Alachisoft.NCache.MetricServer.MetricsAgents.Snmp.Pipeline;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using log4net;



public class DependencyClient : NCache
{
    public DependencyClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }
    public DependencyClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        TestSingleKeyBasedDependency();

        TestFileBasedDependency();

        TestCircularDependency();
    }

    public void TestFileBasedDependency()
    {
        try
        {
            string fileDependentKey = "fileDependentKey";
            string fileDependentValue = "fileDependentValue";
            string dependentFilePath = "D:\\temp\\file.txt";
            CreateFileBasedDependency(fileDependentKey, fileDependentValue, dependentFilePath);
            // Checking if key is inserted in cache
            var item = base.Get(fileDependentKey);
            log.Debug($"Item with file dependency added in cache: {item}, dependent on file: {dependentFilePath}");
            ConsoleKeyInfo keyPressed;
            do
            {
                Console.WriteLine("Press any key, after changing file, ESC to exit...");
                keyPressed = Console.ReadKey();
                // Checking if key is removed from cache after file change
                item = base.Get(fileDependentKey);
                log.Debug($"Item with file dependency after file change: [{item}]");
            } while (item != null && keyPressed.Key != ConsoleKey.Escape);
        }
        catch (Exception ex)
        {
            log.Error($"Error in file based dependency test: {ex.Message}");
        }
    }

    [Obsolete]
    public void TestCircularDependency()
    {
        // Adding parent Item
        Subscriber parentSubscriber = Subscriber.GetRandomSubscriber();
        log.Debug($"Adding parent item: {parentSubscriber.Msisdn}");
        parentSubscriber.Email = "parent" + parentSubscriber.Email;
        base.AddCacheItem(parentSubscriber.Msisdn.ToString(), Subscriber.Serialize(parentSubscriber));

        // Adding child Item with parent's dependency
        Subscriber childSubscriber = Subscriber.GetRandomSubscriber();
        childSubscriber.Email = "child" + childSubscriber.Email;
        AddWIthDependency(parentSubscriber.Msisdn.ToString(), childSubscriber.Msisdn, Subscriber.Serialize(childSubscriber));

        // Modifying parent item to have child's dependency to simulate circular dependency
        log.Debug($"Modifying parent item to have child's dependency: {parentSubscriber.Msisdn}");
        // cache.A
    }

    public void TestSingleKeyBasedDependency()
    {
        // Adding parent Item
        Subscriber parentSubscriber = Subscriber.GetRandomSubscriber();
        log.Debug($"Adding parent item: {parentSubscriber.Msisdn}");
        parentSubscriber.Email = "parent@dependency.com";
        base.AddCacheItem(parentSubscriber.Msisdn.ToString(), Subscriber.Serialize(parentSubscriber));

        // Adding child Item with parent's dependency
        Subscriber childSubscriber = Subscriber.GetRandomSubscriber();
        childSubscriber.Email = "child@dependency.com";
        log.Debug($"Adding child item: {childSubscriber.Msisdn}");
        AddWIthDependency(parentSubscriber.Msisdn.ToString(), childSubscriber.Msisdn, Subscriber.Serialize(childSubscriber));

        // Check if child is added 
        var childItem = base.Get(childSubscriber.Msisdn.ToString());
        log.Debug($"Child item added with parent's dependency, childItem: {childItem}");

        // Removing parent Item
        log.Debug($"Removing parent item: {parentSubscriber.Msisdn}");
        base.Remove(parentSubscriber.Msisdn.ToString());

        // Get child Item
        log.Debug($"Now Getting child item: {childSubscriber.Msisdn}");
        childItem = base.Get(parentSubscriber.Msisdn.ToString());
        if (childItem == null)
        {
            log.Debug($"Child item is removed with parent item: {parentSubscriber.Msisdn}");
        }
        else
        {
            log.Debug($"Child item is not removed with parent item: {parentSubscriber.Msisdn}");
        }

        // Adding child again
        log.Debug($"Adding child item again: {childSubscriber.Msisdn}");
        AddWIthDependency(parentSubscriber.Msisdn.ToString(), childSubscriber.Msisdn, Subscriber.Serialize(childSubscriber));

        // Now modifying parent item
        log.Debug($"Modifying parent item: {parentSubscriber.Msisdn}");
        parentSubscriber.Email += ".pk";
        cache.Insert(parentSubscriber.Msisdn.ToString(), Subscriber.Serialize(parentSubscriber));

        // Get child Item
        log.Debug($"Getting after modifying parent item: {childSubscriber.Msisdn}");
        childItem = base.Get(childSubscriber.Msisdn.ToString());
        if (childItem != null)
        {
            log.Debug($"Child item is not removed after updating parent item: {parentSubscriber.Msisdn}");
        }
        else
        {
            log.Debug($"Child item is removed after updating parent item: {parentSubscriber.Msisdn}");
        }
    }

    public void AddWIthDependency(string dependencyKey, string dependentItemKey, object dependentItem)
    {
        try
        {
            // Generate an instance of Key Dependency
            CacheDependency dependency = new KeyDependency(dependencyKey);
            // FileDependency fileDependency = new FileDependency("C:\\temp\\file.txt");
            // dependency.Dependencies.Add(fileDependency);
            // Create CacheItem to with your desired object
            CacheItem cacheItem = new CacheItem(dependentItem);
            // Add Key Dependency to order
            cacheItem.Dependency = dependency;
            // Add order in cache with dependency
            cache.Add(dependentItemKey, cacheItem);
            log.Debug($"Added dependent item in cache: {_cacheName}, dependencyKey: {dependencyKey}, dependentItem's key: {dependentItemKey}");
        }
        catch (Exception ex)
        {
            log.Error($"Error adding dependent item in cache: {_cacheName}, {ex.Message}");
        }
    }

    /// <summary>
    /// Create file based dependency, where a key is dependent on a file.
    /// If the file is renamed, removed or contents are changed, the item will be removed from cache.
    /// This is a test method and should be implemented with actual file path and logic.
    /// </summary>
    public void CreateFileBasedDependency(string key, object value, string filePath)
    {
        try
        {
            // Create a file dependency
            FileDependency fileDependency = new FileDependency(filePath);
            // Create a cache item with the value and the file dependency
            CacheItem cacheItem = new CacheItem(value) { Dependency = fileDependency };
            
            cacheItem.SetCacheDataNotification(CacheDataNotificationCallback, EventType.ItemRemoved, EventDataFilter.DataWithMetadata);
            // Add the item to the cache
            cache.Add(key, cacheItem);
            log.Debug($"Added item with file dependency: {key}");
        }
        catch (Exception ex)
        {
            log.Error($"Error adding item with file dependency: {ex.Message}");
        }
    }

    public void CacheDataNotificationCallback(string key, CacheEventArg cacheEventArgs)
    {
        log.Debug($"{key} is {cacheEventArgs.EventType}ed due to {cacheEventArgs.CacheItemRemovedReason}");
    }

}