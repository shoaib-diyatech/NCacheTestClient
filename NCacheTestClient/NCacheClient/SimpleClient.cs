namespace NCacheClient;
using Alachisoft.NCache.Client;
using Microsoft.Identity.Client;
using System.Collections;
using System.Net.NetworkInformation;

public class SimpleClient: NCache{
    public SimpleClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }

    public SimpleClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        cache.Clear();

        PopulateCache(100);

        PrintCacheEntriesAsync();
        log.Debug("Printing Cache Entries... ");

        String key = "abc";
        String value = "abcVALUE";
        Add(key, value);

        Remove("5");

        // Subscriber sub = Subscriber.GetRandomSubscriber();
        // AddCacheItem(sub.Msisdn, sub);
        // Get(sub.Msisdn);
        // InsertCacheItem(sub.Msisdn, sub);
        // Remove(sub.Msisdn);
        // Get(sub.Msisdn);
    }

    public IEnumerator GetCacheEnumerator()
    {
        return cache.GetEnumerator();
    }

    public ICache GetCacheObj()
    {
        return cache;
    }

    private async Task PrintCacheEntriesAsync()
    {
        await Task.Run(() =>
            {
                IEnumerator enumerator = cache.GetEnumerator();
                try
                {
                    int totalCount = 0;
                    while (enumerator.MoveNext())
                    {
                        DictionaryEntry id = (DictionaryEntry)enumerator.Current;
                        Console.WriteLine("Key: " + id.Key + ", Value: " + id.Value);
                        Task.Delay(500).Wait();
                        totalCount++;
                    }
                    log.Info("Total Entries retreived: " + totalCount);
                }
                finally
                {
                    //enumerator.Dispose();
                }
            });
    }

    public void PopulateCache(int count)
    {
        for (int i = 0; i < count; i++)
        {
            String key = i.ToString();
            String value = i.ToString();
            Insert(key, value);
        }
        log.Info($"Cache populated with {count} entries.");
    }

    public void PopulateRandom(int count)
    {
        cache.Clear();
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            String key = random.Next(1000, 9999).ToString();
            String value = random.Next(1000, 9999).ToString();
            log.Debug($"Inserting key: {key}, value: {value} into cache.");
            Insert(key, value);
        }
        log.Info($"Cache populated with {count} random entries.");
    }
}