namespace NCacheClient;

using Alachisoft.NCache.Client;
using Alachisoft.NCache.Common.DataPersistence;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;

public class TagClient : NCache
{
    public TagClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }

    public TagClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public enum SubscriberTags{
        ISB,
        KHI,
        LHR,
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
        string[] tags1 = new string[] { SubscriberTags.ISB.ToString(), SubscriberTags.KHI.ToString() };
        string[] tags2 = new string[] { SubscriberTags.KHI.ToString(), SubscriberTags.LHR.ToString() };
        string[] tags3 = new string[] { SubscriberTags.LHR.ToString(), SubscriberTags.ISB.ToString() };
        AddWithTags(tk1, tv1, tags1);
        AddWithTags(tk2, tv2, tags1);
        AddWithTags(tk3, tv3, tags2);
        AddWithTags(tk4, tv4, tags2);
        AddWithTags(tk5, tv5, tags3);
        GetAllTagData(SubscriberTags.ISB.ToString(), true);
        GetAllTagData(SubscriberTags.LHR.ToString(), true);

        AddWIthNamedTags();
        AddWIthNamedTags();
        AddWIthNamedTags();
        SearchOneTagWithSQL(SubscriberTags.ISB.ToString());
        SearchNamedTagsWithOQL("Age", 10);

    }

    public bool AddWithTags(string key, object value, string[] tag)
    {
        try
        {
            Subscriber sub = Subscriber.GetRandomSubscriber();
            CacheItem cacheItem = new CacheItem(Subscriber.Serialize(sub));
            Tag[] tags = new Tag[tag.Count()];
            for (int i = 0; i < tags.Count(); i++)
            {
                tags[i] = new Tag(tag[i]);
            }
            cacheItem.Tags = tags;
            key = sub.Msisdn;
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


    public bool AddWIthNamedTags()
    {
        string key = "";
        try
        {
            Subscriber sub = Subscriber.GetRandomSubscriber();
            key = sub.Id.ToString();
            CacheItem cacheItem = new CacheItem(Subscriber.Serialize(sub));

            // Creating a Named Tags Dictionary
            var namedTags = new NamedTagsDictionary();

            // Adding Named Tags to the Dictionary
            // Where keys are the names of the tags as string type and Values are of primitive type
            namedTags.Add("Age", sub.GetAge());

            // Setting the named tag property of the cacheItem
            cacheItem.NamedTags = namedTags;

            cache.Add(key, cacheItem);
            log.Debug($"Item {key} added successfully with namedTags, Age: {sub.GetAge()}");
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

    public IDictionary<string, string> GetAllTagData(string tag, bool printData)
    {
        try
        {
            Tag[] tags = new Tag[1];
            tags[0] = new Tag(tag);
            IDictionary<string, string> cacheData = cache.SearchService.GetByTags<string>(tags, TagSearchOptions.ByAllTags);
            if (printData)
                foreach (var (k, v) in cacheData)
                {
                    Subscriber sub = Subscriber.Parse(v);
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

    public void SearchOneTagWithSQL(string tagName)
    {
        try
        {
            // Define the SQL query with a parameter placeholder
            string tagPlaceHolder = "$Tag$";
            string query = $"SELECT Email FROM NCacheClient.Subscriber WHERE {tagPlaceHolder} = ?";
            log.Debug($"Executing Query: {query}");

            // Create a QueryCommand and add the parameter value
            var queryCommand = new QueryCommand(query);
             queryCommand.Parameters.Add(tagPlaceHolder, tagName);

            // Execute the query
            ICacheReader reader = cache.SearchService.ExecuteReader(queryCommand);
            int fieldCount = reader.FieldCount;
            if (fieldCount == 0)
            {
                log.Error($"No items found with Tag = {tagName}");
                return;
            }

            // Iterate through the results
            while (reader.Read())
            {
                Subscriber item = reader.GetValue<Subscriber>(0);
                // Process the retrieved item
            }
        }
        catch (OperationFailedException ex)
        {
            // Handle exceptions
            log.Error($"Error: {ex.Message}");
        }
    }
    public void SearchNamedTagsWithOQL(string tagName, int value)
    {
        try
        {
            // Define the SQL query with a parameter placeholder
            string query = $"SELECT Email FROM NCacheClient.Subscriber WHERE {tagName} > {value}";
            log.Debug($"Executing Query: {query}");

            // Create a QueryCommand and add the parameter value
            var queryCommand = new QueryCommand(query);
            // queryCommand.Parameters.Add(tagName, value);

            // Execute the query
            ICacheReader reader = cache.SearchService.ExecuteReader(queryCommand);
            int fieldCount = reader.FieldCount;
            if (fieldCount == 0)
            {
                log.Error($"No items found with {tagName} > {value}");
                return;
            }

            // Iterate through the results
            while (reader.Read())
            {
                Subscriber item = reader.GetValue<Subscriber>(0);
                // Process the retrieved item
            }
        }
        catch (OperationFailedException ex)
        {
            // Handle exceptions
            log.Error($"Error: {ex.Message}");
        }

    }

}