namespace NCacheClient;

using Alachisoft.NCache.Caching;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Common.Monitoring;
using Alachisoft.NCache.Common.Util;
using Alachisoft.NCache.Config.Dom;
using Alachisoft.NCache.Runtime.Caching;

public class BulkClient : NCache
{
    public BulkClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }

    public BulkClient(List<string>ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        // Implementation of the abstract method Test
        log.Debug("Initiating Test method in BulkClient");
        List<string> keys = new List<string> { "key1", "key2", "key3", "key4", "key5" };
        List<string> values = new List<string> { "value1", "value2", "value3", "value4", "value5" };
        log.Debug("Adding new keys through Add Bulk");
        AddBulk(keys, values);
        log.Debug("Getting the existing keys through Get Bulk");
        GetBulk(keys);
        log.Debug("Removing the existing keys through Get Bulk");
        RemoveBulk(keys);
        log.Debug("Getting the keys which do not exist through Get Bulk");
        GetBulk(keys);
        InsertBulk(keys, values);
        log.Debug("Getting the existing keys again through Get Bulk");
        GetBulk(keys);
        log.Debug("Adding the existing keys again through Add Bulk");
        AddBulk(keys, values);
    }

    public void AddBulk(List<string> keys, List<string> values)
    {
        try
        {
            if (keys.Count != values.Count)
            {
                log.Error("Cannot perform AddBulk operation, keys and Values count mismatch");
                return;
            }
            Dictionary<string, CacheItem> dictionaryCacheItems = new();
            for (int i = 0; i < keys.Count; i++)
            {
                CacheItem cacheItem = new CacheItem(values[i]);
                dictionaryCacheItems.Add(keys[i], cacheItem);
            }

            IDictionary<string, Exception> addExceptionItems = cache.AddBulk(dictionaryCacheItems);
            log.Error($"{addExceptionItems.Count} items failed to be inserted in cache");
            foreach (var v in addExceptionItems)
            {
                log.Error($"item: {v.Key} failed, exception: {v.Value}");
            }
        }
        catch (Exception exp)
        {
            log.Error($"BulkClient: Exception in AddBulk method: {exp.Message}");
        }
    }

    public void InsertBulk(List<string> keys, List<string> values)
    {
        try
        {
            if (keys.Count != values.Count)
            {
                log.Error("Cannot bulk insert keys and values do not match");
            }

            Dictionary<string, CacheItem> bulkInsertItems = new();
            for (int i = 0; i < keys.Count; i++)
            {
                CacheItem cacheItem = new CacheItem(values[i]);
                bulkInsertItems.Add(keys[i], cacheItem);
            }
            IDictionary<string, Exception> dicInsertExceptions = cache.InsertBulk(bulkInsertItems);
            log.Debug($"{dicInsertExceptions.Count} Failed");
            foreach (var v in dicInsertExceptions)
            {
                log.Error($"Item {v.Key} Failed, Exception: {v.Value.Message}");
            }
        }
        catch (Exception exp)
        {
            log.Error($"BulkClient: Exception in InsertBulk method: {exp.Message}");
        }
    }

    public void RemoveBulk(List<string> keys)
    {
        try
        {
            if (keys.Count == 0)
            {
                log.Error("Nothing to remove, Are you kidding?? ");
            }
            cache.RemoveBulk(keys);
            log.Debug("Hopefully items have been removed from cache, or are still being removed");
        }
        catch (Exception exp)
        {
            log.Error($"BulkClient: Exception in RemoveBulk method: {exp.Message}");
        }
    }

    public List<string> GetBulk(List<string> keys)
    {
        try
        {
            if (keys.Count == 0)
            {
                log.Error("Nothing to get, Are you kidding?? ");
            }
            IDictionary<string, CacheItem> dictGetItems = cache.GetCacheItemBulk(keys);
            List<string> getValues = new();
            foreach (var v in dictGetItems)
            {
                log.Debug($"Get Item key: {v.Key}, Value: {v.Value}");
                getValues.Add(v.Value.GetValue<string>());
            }
            return getValues;
        }
        catch (Exception exp)
        {
            log.Error($"BulkClient: Exception in GetBulk method: {exp.Message}");
            return null;
        }
    }
}