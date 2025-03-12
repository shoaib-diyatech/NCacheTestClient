namespace NCacheClient;
using Alachisoft.NCache.Licensing.DOM;

using Alachisoft.NCache.Caching;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Common.Monitoring;
using Alachisoft.NCache.Common.Util;
using Alachisoft.NCache.Config.Dom;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;

/// <summary>
/// Testing Events and Notifications of NCache
/// </summary>
class EventClient : NCache
{
    public EventClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {

    }

    public EventClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {

    }

    public override void Test()
    {
        RegisterAllEvents();
        DummyAddInsertRemove();
        RegisterCallbackOnItem("KumailKey");
    }

    public void RegisterAllManagementEvents()
    {
        // Register cache cleared event
        // OnCacheCleared callback will be triggered on cache clear event
        cache.NotificationService.CacheCleared += OnCacheCleared;

        // Register cache stopped event
        // OnCacheStopped callback will be triggered when cache is stopped
        cache.NotificationService.CacheStopped += OnCacheStopped;


        // Register memebr join event
        // OnMemeberJoined callback will be triggered when a new member joins cache
        // cache.NotificationService.MemberJoined += OnMemberJoined;


        // Register memebr left event
        // OnMemeberleft callback will be triggered when a member leaves cache
        // cache.NotificationService.MemberLeft += OnMemberLeft;
    }

    public void RegisterAllEvents()
    {
        CacheEventDescriptor updateEventDescriptor = cache.MessagingService.RegisterCacheNotification(new CacheDataNotificationCallback(OnUpdate), EventType.ItemUpdated, EventDataFilter.None);
        CacheEventDescriptor addEventDescriptor = cache.MessagingService.RegisterCacheNotification(new CacheDataNotificationCallback(OnAdd), EventType.ItemAdded, EventDataFilter.None);
        CacheEventDescriptor removeEventDescriptor = cache.MessagingService.RegisterCacheNotification(new CacheDataNotificationCallback(OnRemoved), EventType.ItemRemoved, EventDataFilter.None);
        if (updateEventDescriptor.IsRegistered && addEventDescriptor.IsRegistered && removeEventDescriptor.IsRegistered)
        {
            log.Debug($"All events registered successfully on cache: {cache}");
        }
        else
        {
            log.Error($"Failed to register all events on cache: {cache}");
        }
    }

    public void DummyAddInsertRemove()
    {
        cache.Clear();
        cache.Add("T1", "T1");
        log.Debug("T1 added");
        cache.Insert("T1", "T1");
        log.Debug("T1 inserted");
        cache.Insert("T2", "T2");
        log.Debug("T2 inserted");
        cache.Insert("T3", "T3");
        log.Debug("T3 inserted");
        cache.Remove("T1");
        log.Debug("T1 removed");
        cache.Remove("T2");
        log.Debug("T2 removed");
        cache.Remove("T3");
        log.Debug("T3 removed");
    }


    public void RegisterCallbackOnItem(string key)
    {
        //string key = "Product:Chai";

        // Fetch item from cache
        CacheItem cacheItem = cache.GetCacheItem(key);

        if (cacheItem == null)
        {
            Subscriber subscriber = new Subscriber() { Msisdn = "1234567890", Id = 1 };
            cacheItem = new CacheItem(subscriber);
        }

        // create CacheDataNotificationCallback object
        var updateCallback = new CacheDataNotificationCallback(OnUpdate);
        var removedCallback = new CacheDataNotificationCallback(OnRemoved);
        var addCallback = new CacheDataNotificationCallback(OnAdd);


        cacheItem.SetCacheDataNotification(updateCallback, EventType.ItemUpdated, EventDataFilter.None);
        log.Debug($"Update Event registered for key: {key}");

        cacheItem.SetCacheDataNotification(removedCallback, EventType.ItemRemoved, EventDataFilter.None);
        log.Debug($"Remove Event registered for key: {key}");

        cacheItem.SetCacheDataNotification(addCallback, EventType.ItemAdded, EventDataFilter.None);
        log.Debug($"Add Event registered");
        //Re-inserts the cacheItem into cache with events registered
        try
        {
            cache.Insert(key, cacheItem);
        }
        catch (Exception e)
        {
            log.Error(e.Message);
        }
        cache.Clear();
        CacheItemVersion cacheItemVersion = cache.Insert(key, "T1");
        log.Debug($"CacheItemVersion: {cacheItemVersion}");
        cache.Insert("T1", "T1sdfsdfsdfs");
        cache.Remove("T1");
    }
    private void OnAdd(string key, CacheEventArg args)
    {
        // Handle cache data modification event
        log.Debug($"Cache item with key '{key}' has been removed. Event Type: {args.EventType}");
    }

    private void OnUpdate(string key, CacheEventArg args)
    {
        // Handle cache data modification event
        log.Debug($"Cache item with key '{key}' has been modified. Event Type: {args.EventType}");
    }


    private void OnRemoved(string key, CacheEventArg args)
    {
        // Handle cache data modification event
        log.Debug($"Cache item with key '{key}' has been modified. Event Type: {args.EventType}");
    }

    private void OnCacheCleared()
    {
        log.Debug($"Cache cleared event triggered on cache: {_cacheName}");
    }

    private void OnCacheStopped(string cacheName)
    {
        log.Debug($"Cache stopped event triggered on cache: {cacheName}");
    }

    private void OnMemberJoined(NodeInfo nodeInfo)
    {
        log.Debug($"Member joined with ip:port {nodeInfo.IpAddress}:{nodeInfo.Port} event triggered on cache: {_cacheName}");
    }

    private void OnMemberLeft(NodeInfo nodeInfo)
    {
        log.Debug($"Member left with ip:port {nodeInfo.IpAddress}:{nodeInfo.Port} event triggered on cache: {_cacheName}");
    }
}

