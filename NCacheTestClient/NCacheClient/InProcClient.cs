using Alachisoft.NCache.Client;

namespace NCacheClient;

public class InProcClient : NCache
{

    public InProcClient(string cacheName) : base(cacheName)
    {
    }

    public override void Test()
    {
        Subscriber subscriber = Subscriber.GetRandomSubscriber();
        log.Debug($"Adding subscriber with key: {subscriber.Msisdn} to the cache: {subscriber}");
        cache.Add(subscriber.Msisdn, subscriber);
        log.Debug($"Now changing subscirber's name to {subscriber.Name} to {subscriber.Name}1");
        subscriber.Name = $"{subscriber.Name}1";
        log.Debug($"Now fetching from cache against key: {subscriber.Msisdn} to see if there is any update in the cache");
        var subscriberFromCache = cache.Get<Subscriber>(subscriber.Msisdn);
        if (subscriberFromCache != null)
        {
            log.Debug($"Subscriber fetched from cache: {subscriberFromCache}");
        }
        else
        {
            log.Error($"Subscriber with key {subscriber.Msisdn} not found in the cache");
        }
        // Compare if the subscriber fetched from cache is the same as the cache object
        if (subscriberFromCache.Equals(subscriber))
        {
            log.Debug("Subscriber fetched from cache is the SAME as the cache object");
        }
        else
        {
            log.Error("Subscriber fetched from cache is NOT SAME as the cache object");
        }
    }

    [Obsolete]
    public void StartStandAloneInProcCache()
    {
        try
        {
            string cacheName = "InProcCache";
            List<ServerInfo> serverInfoList = new List<ServerInfo>();
            ServerInfo serverInfo;
            if (_clusterIPs.Count > 0)
            {
                serverInfo = new ServerInfo(_clusterIPs[0], _port);
                serverInfoList.Add(serverInfo);

                var connectionOptions = new CacheConnectionOptions
                {
                    //ServerList = serverInfoList,
                    CommandRetries = 1,
                    ConnectionRetries = 1,
                    ConnectionTimeout = System.TimeSpan.FromMilliseconds(500),
                    LogLevel = Alachisoft.NCache.Client.LogLevel.Debug,
                    EnableClientLogs = true,
                    AppName = cacheName

                };
                CacheManager.StartCache("InProcCache");
                log.Debug($"Cache started successfully, with name: {cacheName}");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error starting cache: {ex.Message}");
        }
    }
}