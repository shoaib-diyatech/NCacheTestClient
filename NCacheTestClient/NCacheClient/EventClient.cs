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
        //RegisterAllEvents();
        //RegisterAllManagementEvents();
        //DummyAddInsertRemove();
        //RegisterCallbackOnNewCacheItem("KumailKey");
        string existingKey = "ExistingKey";
        cache.Add(existingKey, "ExistingValue");
        RegisterCallbackOnExistingCacheItem(existingKey);
        //cache.Add(existingKey, "ExistingValue");
        log.Debug($"inserting key: {existingKey}");
        cache.Insert(existingKey,"123");
    }

    public void RegisterAllManagementEvents()
    {
        try
        {
            // Register cache cleared event
            // OnCacheCleared callback will be triggered on cache clear event
            cache.NotificationService.CacheCleared += OnCacheCleared;

            // Register cache stopped event
            // OnCacheStopped callback will be triggered when cache is stopped
            cache.NotificationService.CacheStopped += OnCacheStopped;


            // Register memebr join event
            // OnMemeberJoined callback will be triggered when a new member joins cache
            cache.NotificationService.MemberJoined += OnMemberJoined;


            // Register memebr left event
            // OnMemeberleft callback will be triggered when a member leaves cache
            cache.NotificationService.MemberLeft += OnMemberLeft;
            log.Debug($"All management events registered successfully on cache: {cache}");
        }
        catch (Exception e)
        {
            log.Error($"Failed to register all management events on cache: {cache}, error: {e.Message}");
        }
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


    /// <summary>
    /// Creates a new CacheItem, adds it in cache with <paramref name="key"/> and registers callbacks for item added, updated, and removed events.
    /// </summary>
    /// <param name="key"></param>
    public void RegisterCallbackOnNewCacheItem(string key)
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

        // Creating Removed Callback
        var removedCallback = new CacheDataNotificationCallback(OnRemoved);

        // Creating Add Callback
        var addCallback = new CacheDataNotificationCallback(OnAdd);


        cacheItem.SetCacheDataNotification(updateCallback, EventType.ItemUpdated, EventDataFilter.None);
        log.Debug($"Update Event registered for key: {key}");

        // Registering removed callback against the cacheItem
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
        //cache.Clear();
        //CacheItemVersion cacheItemVersion = cache.Insert(key, "T1");
        //log.Debug($"CacheItemVersion: {cacheItemVersion}");
        //cache.Insert("T1", "T1sdfsdfsdfs");
        cache.Remove(key);
        log.Debug($"{key} removed from cache");
    }

    /// <summary>
    /// Registers a callback for an existing cache item with <paramref name="key"/>
    /// Register Item Notifications for a Particular Item
    /// https://www.alachisoft.com/resources/docs/ncache/prog-guide/item-level-event-notifications.html?tabs=net%2Cnet1%2Cnet2%2Cnet3%2Cnet4%2Cnet5#register-item-notifications-for-a-particular-item
    /// </summary>
    /// <param name="key"></param>
    public void RegisterCallbackOnExistingCacheItem(string key)
    {
        // create CacheDataNotificationCallback object
        var dataNotificationCallback = new CacheDataNotificationCallback(OnUpdate);

        // Register notifications for a specific item being updated in cache
        // EventDataFilter as DataWithMetadata which returns keys along with their entire data
        cache.MessagingService.RegisterCacheNotification(key, dataNotificationCallback, EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
    }

    private void OnAdd(string key, CacheEventArg args)
    {
        // Handle cache data modification event
        log.Debug($"Cache item with key '{key}' has been added. Event Type: {args.EventType}");
    }

    private void OnUpdate(string key, CacheEventArg args)
    {
        // Handle cache data modification event
        log.Debug($"Cache item with key '{key}' has been updated. Event Type: {args.EventType}, cache: {args.CacheName}");
        // Item can be used if EventDataFilter is DataWithMetadata or Metadata
        if (args.Item != null)
        {
            Object objSub = args.Item.GetValue<Object>();
            if(objSub is Subscriber)
            {
                objSub = (Subscriber)objSub;
            }
            else
            {
                log.Debug($"");
            }
                //Console.WriteLine($"Updated Item is a Subscriber having name: '{updatedSub.Name}' with ID: '{updatedSub.Id}' and email: '{updatedSub.Email}'");
        }
    }

    private void OnRemoved(string key, CacheEventArg args)
    {
        // Handle cache data modification event
        log.Debug($"Cache item with key '{key}' has been removed. Event Type: {args.EventType}");
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

