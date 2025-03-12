namespace NCacheClient;

using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;

public class TagClient : NCache
{
    public TagClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }

    public TagClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        string tk1 = "tk1";
        string tv1 = "tv1";
        string tk2 = "tk2";
        string tv2 = "tv2";
        string tk3 = "tk3";
        string tv3 = "tv3";
        string tk4 = "tk4";
        string tv4 = "tv4";
        string tk5 = "tk5";
        string tv5 = "tv5";
        string tag1 = "T1";
        string tag2 = "T2";
        string[] tags1 = new string[] { "a", "b" };
        string[] tags2 = new string[] { "b", "c" };
        string[] tags3 = new string[] { "c", "a" };
        AddWithTags(tk1, tv1, tags1);
        AddWithTags(tk2, tv2, tags1);
        AddWithTags(tk3, tv3, tags2);
        AddWithTags(tk4, tv4, tags2);
        AddWithTags(tk5, tv5, tags3);
        GetAllTagData(tag1, true);
        GetAllTagData(tag2, true);
    }

    public bool AddWithTags(string key, object value, string[] tag)
    {
        try
        {
            Subscriber sub = new Subscriber() { Msisdn = value.ToString(), Id = 11111 };
            CacheItem cacheItem = new CacheItem(sub);
            Tag[] tags = new Tag[tag.Count()];
            foreach (string t in tag)
            {
                tags.Append(new Tag(t));
            }
            cacheItem.Tags = tags;
            cache.Add(key, cacheItem);
            log.Debug($"Item {key} added successfully with tags:  {String.Join(" ", tag)}");
            return true;
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item {key} with tag {tag}: {ex.Message}");
            return false;
        }
    }

    public void GetAllTagData(string tag, bool printData)
    {
        try
        {
            CacheItem[] cacheItems = cache.GetByTag(tag);
            if (cacheItems != null && cacheItems.Length > 0)
            {
                foreach (CacheItem cacheItem in cacheItems)
                {
                    log.Debug($"Key: {cacheItem.Key}, Value: {cacheItem.Value}");
                }
            }
            else
            {
                log.Debug($"No items found with tag {tag}");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error getting items with tag {tag}: {ex.Message}");
        }
    }
}