namespace NCacheClient;

using Alachisoft.NCache.Client;
using Alachisoft.NCache.Common.DataPersistence;
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


    public bool AddWIthNamedTags(string key, object value)
    {
        try
        {
            Subscriber sub = new Subscriber() { Msisdn = value.ToString(), Id = 11111 };
            CacheItem cacheItem = new CacheItem(sub);

            // Creating a Named Tags Dictionary
            var namedTags = new NamedTagsDictionary();

            // Adding Named Tags to the Dictionary
            // Where keys are the names of the tags as string type and Values are of primitive type
            namedTags.Add("Age", 25);

            // Setting the named tag property of the cacheItem
            cacheItem.NamedTags = namedTags;

            cache.Add(key, cacheItem);
            log.Debug($"Item {key} added successfully with namedTags");
            return true;
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item {key} with namedTaga {ex.Message}");
            return false;
        }
    }
    public ICollection<string> GetAllKeysByTag(string tagName, bool printData)
    {
        try
        {
            log.Debug($"Getting keys with tag {tagName} through wildcard search");
            ICollection<string> keys = cache.SearchService.GetKeysByTag(tagName);
            if (printData)
                foreach (var key in keys)
                {
                    log.Debug($"Key: {key}");
                }

            log.Debug($"Getting keys with tag {tagName} through Tag search");
            Tag[] tag = new Tag[1];
            tag.Append(new Tag(tagName));
            keys = cache.SearchService.GetKeysByTag(tag[0]);
            if (printData)
                foreach (var key in keys)
                {
                    log.Debug($"Key: {key}");
                }
            return keys;
        }
        catch (Exception ex)
        {
            log.Error($"Error getting keys with tag {tagName}: {ex.Message}");
            return null;
        }
    }

    public ICollection<string> GetAllKeysByTags(string[] tagsName, bool printData)
    {
        Tag[] tags = new Tag[tagsName.Count()];
        foreach (string t in tagsName)
        {
            tags.Append(new Tag(t));
        }
        TagSearchOptions tagSearchOptions = new TagSearchOptions();
        //tagSearchOptions.
        ICollection<string> keys = cache.SearchService.GetKeysByTags(tags, tagSearchOptions);
        foreach (var key in keys)
        {
            log.Debug($"Key: {key}");
        }
        return keys;
    }

    public IDictionary<string, Subscriber> GetAllTagData(string tag, bool printData)
    {
        try
        {
            TagSearchOptions tagSearchOptions = new TagSearchOptions();
            tagSearchOptions.CompareTo(tag);
            Tag[] tags = new Tag[1];
            tags.Append(new Tag(tag));
            IDictionary<string, Subscriber> cacheData = cache.SearchService.GetByTags<Subscriber>(tags, tagSearchOptions);
            if (printData)
                foreach (var (k, v) in cacheData)
                {
                    log.Debug($"Key: {k}, Value: {v}");
                }
            return cacheData;
        }
        catch (Exception ex)
        {
            log.Error($"Error getting items with tag {tag}: {ex.Message}");
            return null;
        }
    }
}