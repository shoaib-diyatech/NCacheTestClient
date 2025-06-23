namespace NCacheClient;

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

public abstract class NCache
{
    protected static readonly ILog log = LogManager.GetLogger(typeof(NCache));

    public List<string> _clusterIPs;
    public int _port;
    public string _cacheName;

    private bool _isConnected = false;

    protected ICache cache;

    public NCache(string ip, int port, string cacheName)
    {
        _clusterIPs = new List<string> { ip };
        _port = port;
        _cacheName = cacheName;
    }

    public NCache(List<string> ips, int port, string cacheName)
    {
        _clusterIPs = ips;
        _port = port;
        _cacheName = cacheName;
    }

    public NCache(string cacheName)
    {
        _cacheName = cacheName;
        CacheManager.StartCache(_cacheName);
        cache = CacheManager.GetCache(_cacheName);
        if (cache != null)
        {
            _isConnected = true;
            log.Info($"Cache {_cacheName} Initialized");
        }
    }

    public bool Initialize()
    {
        // Connect to the cache
        try
        {
            if (_isConnected)
            {
                log.Info($"Cache {_cacheName} is already Initialized");
                return true;
            }
            var serverInfoList = new List<ServerInfo>();
            foreach (var ip in _clusterIPs)
            {
                serverInfoList.Add(new ServerInfo(ip, _port));
            }

            var connectionOptions = new CacheConnectionOptions
            {
                ServerList = serverInfoList,
                CommandRetries = 1,
                ConnectionRetries = 1,
                ConnectionTimeout = System.TimeSpan.FromMilliseconds(500),
                LogLevel = Alachisoft.NCache.Client.LogLevel.Debug,
                EnableClientLogs = true
            };

            cache = CacheManager.GetCache(_cacheName, connectionOptions);
            _isConnected = true;
            log.Info($"Connected to cache: [{_cacheName}]");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error connecting to cache: [{_cacheName}]" + e.Message);
            log.Error("Error connecting to cache: ", e);
            return false;
        }
        return true;
    }
    abstract public void Test();

    /// <summary>
    /// Adds a key-value pair to the cache.
    /// If the key already exists, it will throw an exception.
    /// </summary>
    public bool Add(string key, string value)
    {
        if (!_isConnected)
        {
            Console.WriteLine("Cache is not connected");
            return false;
        }
        try
        {
            cache.Add(key, value);
            if (log.IsDebugEnabled)
            {
                log.Debug($"Added: {key}, Value: {value}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error adding to cache: " + e.Message);
            log.Error("Error adding to cache: ", e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Inserts a key-value pair into the cache.
    /// If the key already exists, it updates the value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Insert(string key, string value)
    {
        if (!_isConnected)
        {
            Console.WriteLine("Cache is not connected");
            return false;
        }
        try
        {
            cache.Insert(key, value);
            if (log.IsDebugEnabled)
            {
                log.Debug($"Inserted: {key}, Value: {value}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error inserting to cache: " + e.Message);
            log.Error("Error inserting to cache: ", e);
            return false;
        }
        return true;
    }

    protected CacheItemVersion AddCacheItem(string key, object value)
    {
        try
        {
            CacheItem cacheItem = new CacheItem(value);
            CacheItemVersion cacheItemVersion = cache.Add(key, cacheItem);
            log.Debug($"Added, key: [{key}], value: [{value}], to cache: {_cacheName}");
            return cacheItemVersion;
        }
        catch (Exception ex)
        {
            // Handle exceptions
            log.Error($"Error adding item to cache: {ex.Message}");
            return null;
        }
    }

    public void Add(string key, object value, int ttlInSecs)
    {
        try
        {
            // Set absolute expiration
            DateTime absoluteExpiration = DateTime.Now.AddSeconds(ttlInSecs);
            CacheItem cacheItem = new CacheItem(value);
            cacheItem.Expiration = new Expiration(ExpirationType.Absolute, TimeSpan.FromSeconds(ttlInSecs));

            // Add item to cache with absolute expiration
            //WriteThruOptions writeThruOptions = new WriteThruOptions();

            //cache.Add(key, cacheItem);
            cache.Insert(key, cacheItem);

            Console.WriteLine($"Inserted, key:{key}, value: {value.ToString()}, TTL {ttlInSecs} seconds.");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error adding item to cache: {ex.Message}");
        }
    }

    public object Get(string key)
    {
        if (!_isConnected)
        {
            log.Error("Cache is not connected");
            return false;
        }
        try
        {
            var value = cache.Get<object>(key);
            log.Debug($"Value retreived: [{value}]");
            return value;
        }
        catch (Exception e)
        {
            log.Error($"Error getting from cache {e.Message}");
            return null;
        }
    }


    public object Remove(string key)
    {
        if (!_isConnected)
        {
            log.Error("Cache is not connected");
            return false;
        }
        try
        {
            if (cache.Remove<string>(key, out var valueRemoved))
            {
                log.Debug($"Item removed: {valueRemoved}");
                return valueRemoved;
            }
            else
            {
                log.Debug($"Item not found against key: {key}");
                return null;
            }
        }
        catch (Exception e)
        {
            log.Error($"Error removing from cache: {e.Message} against key: {key}");
            return null;
        }
    }

    public void Remove2(string key)
    {
        try
        {
            cache.Remove(key);
            log.Debug($"Key removed: {key}");
        }
        catch (Exception e)
        {
            log.Error("Error removing from cache: ", e);
        }
    }

    /// <summary>
    /// Deletes a key from the cache wheather it exists or not.
    /// </summary>
    /// <param name="key"></param>
    public void Delete(string key)
    {
        if (!_isConnected)
        {
            log.Error("Cache is not connected");
            return;
        }
        try
        {
            cache.Remove(key);
            log.Debug($"Key removed: {key}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error removing from cache: " + e.Message);
            log.Error("Error removing from cache: ", e);
        }
    }

    /// <summary>
    /// Get a cache item and lock it for 10 seconds. Update the item and release the lock.
    /// </summary>
    /// <param name="key"></param>
    public void GetWithLockAndUpdate()
    {
        // add a product to the cache. if it already exists, the operation will fail
        string msisdn = "+1234567890";
        long id = 11001;
        Subscriber subscriber = new Subscriber { Msisdn = msisdn, Id = id };
        CacheItem cacheItem = new CacheItem(subscriber);
        cache.Insert(msisdn, cacheItem);
        log.Debug($"Subscriber added successfully, with key: {msisdn}");

        // Get subscriber 11001 and lock it in the cache
        log.Debug($"Get and lock subscriber {msisdn} in the cache...");
        LockHandle lockHandle = null;

        // Passing "acquireLock:" flag also locks the item and "lockTimeout:" releases the lock after 10sec
        var lockedCacheItem = cache.GetCacheItem(msisdn, acquireLock: true, lockTimeout: TimeSpan.FromSeconds(10), ref lockHandle);
        if (lockedCacheItem != null)
        {
            log.Debug("Subscriber retrieved and locked successfully:");
            var retrievedSub = lockedCacheItem.GetValue<CacheItem>();
            log.Debug($"Retrieved Subscriber: {retrievedSub}");

            // Change the service type to Postpaid
            // retrievedSub.ServiceType = ServiceType.Postpaid;
            //retrievedSub.Id = 22222;

            // Insert() updates the item and releases the lock
            cache.Insert(msisdn, lockedCacheItem, lockHandle, releaseLock: true);
            log.Debug("Updated Subscriber 11001 and released the lock simultaneously in the cache...");
        }
        else
        {
            log.Debug($"Subscriber {msisdn} not found in the cache.");
        }
        log.Debug("Sample completed successfully.");
    }

    public void AddCacheItem(string key, object value, int ttlInSecs)
    {
        try
        {
            // Set absolute expiration
            DateTime absoluteExpiration = DateTime.Now.AddSeconds(ttlInSecs);
            CacheItem cacheItem = new CacheItem(value);
            cacheItem.Expiration = new Expiration(ExpirationType.Absolute, TimeSpan.FromSeconds(ttlInSecs));

            // Add item to cache with absolute expiration
            //WriteThruOptions writeThruOptions = new WriteThruOptions();

            //cache.Add(key, cacheItem);
            cache.Insert(key, cacheItem);

            Console.WriteLine($"Inserted, key:{key}, value: {value.ToString()}, TTL {ttlInSecs} seconds.");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error adding item to cache: {ex.Message}");
        }
    }

    public object GetCacheItem(string key)
    {
        CacheItem cacheItem = cache.GetCacheItem(key);
        string cacheItemValue = cacheItem.GetValue<string>();
        return cacheItemValue;
    }

    public object GetAndModifyCacheItem(string key, string valueToModify)
    {
        CacheItem cacheItem = cache.GetCacheItem(key);
        string cacheItemValue = cacheItem.GetValue<string>();
        cacheItem.SetValue(valueToModify);
        CacheItemVersion version = cache.Insert(key, cacheItem);
        return cacheItemValue;
    }

    public void TestAsyncOperations(string key, string value)
    {
        cache.AddAsync(key, value);
        cache.InsertAsync(key, value);
        cache.RemoveAsync<string>(key);
    }

    public void TestBulkOperations(List<string> keys, List<string> values)
    {
        Dictionary<string, CacheItem> cacheDictItems = new();
        CacheItem[] items = new CacheItem[keys.Count];
        for (int i = 0; i < keys.Count; i++)
        {
            items[i] = new CacheItem(values[i]);
            cacheDictItems.Add(keys[i], items[i]);
        }
        Dictionary<string, Exception> d = (Dictionary<string, Exception>)cache.AddBulk(cacheDictItems);
        log.Debug($"{d.Count} items could not be added in cache");
        foreach (var item in d)
        {
            log.Debug($"item key: {item.Key}, Exception: {item.Value}");
        }

        IDictionary<string, Exception> di = cache.InsertBulk(cacheDictItems);
        log.Debug($"{d.Count} items could not be inserted in cache");
        cache.RemoveBulk(keys);
        // Todo: Need to check if GetBulk takes duplicate keys, and if it does, what is the behavoiur of the result
        IDictionary<string, string> dg = cache.GetBulk<string>(keys);
        foreach (var item in dg)
        {
            log.Debug($"Key: {item.Key}, Value: {item.Value}");
        }
    }

    public void ClearCache()
    {
        if (!_isConnected)
        {
            log.Error("Cache is not connected");
            return;
        }
        cache.Clear();
        log.Debug("Cache cleared successfully");
    }

    public bool Contains(string key)
    {
        if (!_isConnected)
        {
            log.Error("Cache is not connected");
            return false;
        }
        return cache.Contains(key);
    }
}