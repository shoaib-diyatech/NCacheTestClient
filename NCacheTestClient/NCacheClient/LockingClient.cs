using System.Text;
using Alachisoft.NCache.Client;

namespace NCacheClient;

public class LockingClient : NCache
{
    public LockingClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }
    public LockingClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        string key = "ExclusiveLockKey1";
        string value = "ExclusiveLockValue1";
        log.Debug($"Inserting item {key} with value {value} with ExclusiveLock");
        ExclusiveLock(key, 10);
        log.Debug($"Getting item {key} without any lock");
        string valueWithoutGetLock = cache.Get<string>(key);
        string newValue = "ExclusiveLockValue1Updated";
        log.Debug($"Updating item {key} with value {newValue} without any lock");
        cache.Insert(key, newValue);
        //Now Get with lock
        GetWithLock(key, 10);
        //Insert with lock and finally release lock
        bool releaseLock = true;
        string newValueWithLock = "ExclusiveLockValue1UpdatedWithLock";
        InsertWithLock(key, newValueWithLock, releaseLock);

        // Testing Insert if Same
        InsertIfSame();

        InsertWithWrongHandle();
    }

    public bool ExclusiveLock(string key, int lockTimeInSecs = 0)
    {
        try
        {
            bool lockAcquired = cache.Lock(key, TimeSpan.FromSeconds(lockTimeInSecs), out LockHandle lockHandle);
            if (lockAcquired)
            {
                log.Debug($"Item {key} locked successfully");
            }
            else
            {
                log.Error($"Failed to lock item {key}");
            }
            return lockAcquired;
        }
        catch (Exception ex)
        {
            log.Error($"Error locking item {key}: {ex.Message}");
            return false;
        }
    }
    public void ExclusiveUnlock(string key)
    {
        try
        {
            LockHandle lockHandle = new LockHandle();
            cache.Unlock(key, lockHandle);
            log.Debug($"Item {key} unlocked successfully");
        }
        catch (Exception ex)
        {
            log.Error($"Error unlocking item {key}: {ex.Message}");
        }
    }

    public object GetWithLock(string key, int lockTimeInSecs = 0)
    {
        try
        {
            bool acquireLock = true;
            LockHandle lockHandle = new LockHandle();
            String value = cache.Get<string>(key, acquireLock, TimeSpan.FromSeconds(lockTimeInSecs), ref lockHandle);
            return value;
        }
        catch (Exception ex)
        {
            log.Error($"Error getting item {key}: {ex.Message}");
            return null;
        }
    }

    public void InsertWithLock(string key, string value, bool releaseLock = true)
    {
        try
        {
            CacheItem cacheItem = new CacheItem(value);
            LockHandle lockHandle = new LockHandle();
            CacheItemVersion cacheItemVersion = cache.Insert(key, cacheItem, lockHandle, releaseLock);
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item {key}: {ex.Message}");
        }
    }

    public void InsertIfSame()
    {
        try
        {
            string key = "k1";
            string value = "v1";
            CacheItem cacheItem = new CacheItem(value);

            cache.Remove(key); // Just making sure if the key is not present in the cache, to test the add method, otherwise Add throws exception
            log.Debug($"LockingClient: Adding item {key} with value {value}");
            CacheItemVersion cacheItemVersion = cache.Add(key, cacheItem);
            // Waiting for leting the data to be modified
            log.Debug("LockingClient: Waiting for 1 second to let the data be modified");
            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex.Message}");
            }
            string latestValue = cache.GetIfNewer<string>(key, ref cacheItemVersion); // returns null if the item in the cache is not newer than the one we have
            if (latestValue == null)
            {
                log.Debug($"LockingClient: Item {key} is the latest one which we have");
            }
            else
            {
                log.Debug($"LockingClient: Item {key} is not the latest one which we have, latest is: {latestValue}");
            }
        }
        catch (Exception ex)
        {
            log.Error($"LockingClient: Error: {ex.Message}");
        }
    }

    public void InsertWithWrongHandle()
    {
        string key = "k1";
        string value = "v1";
        CacheItemVersion cacheItemVersion = AddCacheItem(key, value);

        // Get Item with Lock
        bool acquireLock = true;
        LockHandle correctLockHandle = new LockHandle();
        String valueFromGetLock = cache.Get<string>(key, acquireLock, TimeSpan.FromSeconds(0), ref correctLockHandle);
        log.Debug($"Item {key} with value {valueFromGetLock} is locked");

        // Insert Item with wrong handle
        string newValue = "v1Updated";
        LockHandle wrongLockHandle = new LockHandle();
        bool releaseLock = true;
        try
        {
            CacheItemVersion newCacheItemVersion = cache.Insert(key, new CacheItem(newValue), wrongLockHandle, releaseLock);
            log.Debug($"Item updated with wrong lock handle"); // Should never be printed
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item with wrong lock handle {key}: {ex.Message}");
        }

        // Now trying to update with null lockhandle
        try
        {
            CacheItemVersion newCacheItemVersion = cache.Insert(key, new CacheItem(newValue), null, releaseLock);
            log.Debug($"Item updated with null lock handle");
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item with correct lock handle {key}: {ex.Message}");
        }


        // Now updating with correct lock handle
        try
        {
            CacheItemVersion newCacheItemVersion = cache.Insert(key, new CacheItem(newValue), correctLockHandle, releaseLock);
            log.Debug($"Item updated with correct lock handle");
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item with correct lock handle {key}: {ex.Message}");
        }



    }
}