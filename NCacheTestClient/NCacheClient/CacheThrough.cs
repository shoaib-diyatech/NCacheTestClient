namespace NCacheClient; 

using System.Configuration;
using System.Text.Json.Nodes;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using ServerSide.CacheThrough;


public class CacheThrough : NCache
{

    public CacheThrough(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }
    public CacheThrough(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }
    public override void Test()
    {
        Alachisoft.NCache.Runtime.CacheManagement.CacheHealth cacheHealth = CacheManager.GetCacheHealth("HomePart");
        cache.Clear();
        string key = "1000";
        //TestReadThru(key);
        TestReadThru("JsonString");
        TestReadThru("SystemJsonObject");
        TestReadThru("AlachisoftJsonObject");
        TestReadThru("CustomObject");
        //TestReadThru("TESTRESYNCONEXPIRATION");
        //TestForceReadThru();
        //TestWriteThru();
    }

    public void TestReadThru(string key)
    {
        try
        {
            log.Debug($"Getting the value for the key {key} which does not exist in the cache");
            var value = cache.Get<object>(key);
            log.Debug($"Value for the key {key} is [{value}]");
            // Now getting the key through ReadThru
            log.Debug($"Getting the value for the key {key} through ReadThru");
            // Specify the readThruOptions for Read-through operations
            var readThruOptions = new ReadThruOptions();
            readThruOptions.Mode = ReadMode.ReadThru;

            Product prod = null;
            if (key == "JsonString")
            {
                var prodJson = cache.Get<string>(key, readThruOptions);
                prod = Product.Parse(prodJson);
            }
            else if(key == "CustomObject")
            {
                prod = cache.Get<Product>(key, readThruOptions);
            }
            else
            {
                var prodJson = cache.Get<object>(key, readThruOptions);
                prod = Product.Parse(prodJson.ToString());
            }
            //var prodJson = cache.Get<object>(key, readThruOptions);

            if (prod != null)
            {
                //prod = Product.Parse(prodJson.ToString());
                log.Debug($"Value for the key {key} through ReadThru is [{prod.ToString()}]");
                log.Debug($"ReadThru test SUCCESSFUL for key: {key}");
            }
            else
            {
                log.Error($"ReadThru test FAILED for key: {key}");
            }
        }
        catch (Exception exp)
        {
            log.Error($"Exception: ReadThru test FAILED for key: {key}", exp);
        }
    }

    public void TestReadThru()
    {
        log.Debug("Initiating TestReadThru method in CacheThrough");
        // Geting a key which does not exist in the cache
        string key = new Random().Next(100000001, 999999999).ToString();
        TestReadThru(key);
    }

    public void TestReadThruBulk()
    {
        log.Debug("Initiating TestReadThruBulk method in CacheThrough");
        // Geting keys which do not exist in the cache
        string[] keys = new string[5];
        for (int i = 0; i < 5; i++)
        {
            keys[i] = new Random().Next(100000001, 999999999).ToString();
        }
        string key = new Random().Next(100000001, 999999999).ToString();
        log.Debug($"Getting the value for the key {key} which does not exist in the cache");
        string value = cache.Get<string>(key);
        log.Debug($"Value for the key {key} is [{value}]");
        // Now getting the key through ReadThru
        log.Debug($"Getting the value for the key {key} through ReadThru");
        // Specify the readThruOptions for Read-through operations
        var readThruOptions = new ReadThruOptions();
        readThruOptions.Mode = ReadMode.ReadThru;

        value = cache.GetBulk<string>(keys, readThruOptions).Values.FirstOrDefault();
        log.Debug($"Value for the key {key} through ReadThru is [{value}]");
        if (value != null)
        {
            log.Debug("ReadThru test successful");
        }
        else
        {
            log.Error("ReadThru test failed");
        }
    }

    public void TestForceReadThru()
    {
        log.Debug("Initiating TestForceReadThru method in CacheThrough");
        // Specifing the key of the item
        string key = new Random().Next(100000001, 999999999).ToString();
        // Adding the key to the cache
        log.Debug($"Adding the key {key} to the cache, with value: {key}");
        cache.Add(key, key);

        // Now getting the key through ReadThruForced

        // Specifing the readThruOptions for Read-through operations
        var readThruOptions = new ReadThruOptions();
        readThruOptions.Mode = ReadMode.ReadThruForced;

        // Retrieve the data of the corresponding item with read thru enabled
        string value = cache.Get<string>(key, readThruOptions);
        log.Debug($"Value for the key {key} through ReadThruForced is [{value}]");
        if (value != key)
        {
            log.Info("ReadThruForced test successful");
        }
        else
        {
            log.Error("ReadThruForced test failed");
        }
    }

    public void TestWriteThru()
    {
        log.Debug("Initiating WriteThru method in CacheThrough");
        // Specifing the key of the item
        string key = new Random().Next(100000001, 999999999).ToString();
        string value = key;
        // Adding the key to the cache
        log.Debug($"Adding the key [{key}] with writethrough, with value: {value}");

        // Specifing the writeThruOptions for Add operation
        var writeThruOptions = new WriteThruOptions();
        writeThruOptions.Mode = WriteMode.WriteThru;
        CacheItem item = new CacheItem(value);
        cache.Add(key, item, writeThruOptions);

        // Retrieve the data of the corresponding item
        value = cache.Get<string>(key);
        log.Debug($"Value for the key [{key}] is [{value}]");
        if (value == key)
        {
            log.Info("WriteThru test successful");
        }
        else
        {
            log.Error("WriteThru test failed");
        }
    }

    public void TestWriteBehind()
    {
        log.Debug("Initiating WriteBehind method in CacheThrough");
        // Specifing the key of the item
        string key = new Random().Next(100000001, 999999999).ToString();
        string value = key;
        // Adding the key to the cache
        log.Debug($"Adding the key {key} with WriteBehind with value: {value}");

        // Specifing the writeThruOptions for Add operation
        var writeThruOptions = new WriteThruOptions();
        writeThruOptions.Mode = WriteMode.WriteBehind;
        CacheItem item = new CacheItem(value);
        cache.Add(key, item, writeThruOptions);

        // Retrieve the data of the corresponding item
        value = cache.Get<string>(key);
        log.Debug($"Value for the key [{key}] is [{value}]");
        if (value == key)
        {
            log.Info("WriteBehind test successful");
        }
        else
        {
            log.Error("WriteBehind test failed");
        }
    }
}