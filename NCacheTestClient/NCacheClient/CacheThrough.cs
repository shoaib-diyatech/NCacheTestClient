using System.Configuration;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;

namespace NCacheClient;

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
        TestReadThru();
        TestForceReadThru();
        TestWriteThru();
    }

    public void TestReadThru()
    {
        log.Debug("Initiating TestReadThru method in CacheThrough");
        // Geting a key which does not exist in the cache
        string key = new Random().Next(100000001, 999999999).ToString();
        log.Debug($"Getting the value for the key {key} which does not exist in the cache");
        string value = cache.Get<string>(key);
        log.Debug($"Value for the key {key} is [{value}]");
        // Now getting the key through ReadThru
        log.Debug($"Getting the value for the key {key} through ReadThru");
        // Specify the readThruOptions for Read-through operations
        var readThruOptions = new ReadThruOptions();
        readThruOptions.Mode = ReadMode.ReadThru;

        value = cache.Get<string>(key, readThruOptions);
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

    public void TestWriteThru(){
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

    public void TestWriteBehind(){
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