namespace NCacheClient;
using Alachisoft.NCache.Client;

[Serializable]
public class Car
{
    public string Name { get; set; }
    public string Category { get; set; }
}

public enum Group
{
    HighRPU,
    LowRPU,
    VIP
}
public class GroupClient : NCache
{
    public GroupClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }
    public GroupClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        string HighRPU = Group.HighRPU.ToString();
        string VIP = Group.VIP.ToString();
        string LowRPU = Group.LowRPU.ToString();
        cache.Clear();

        Subscriber sub = Subscriber.GetRandomSubscriber();
        AddWithGroup(sub.Msisdn, sub, HighRPU);
        UpdateGroup(sub.Msisdn, VIP);

        GetAllGroupData(HighRPU, true);
        GetAllGroupData(VIP, true);

        SearchWithOQL(HighRPU, true);
        SearchWithOQL(VIP, true);
    }

    public void SearchWithOQL(string groupName, bool logAllData = false)
    {
        string groupPlaceHodler = "$Group$";
        //string queryWithoutGroup = $"SELECT Name, Email, Msisdn FROM NCacheClient.Subscriber";
        string query = $"SELECT $VALUE$ FROM NCacheClient.Subscriber WHERE $Group$ = ?";
        // Use QueryCommand for query execution
        var queryCommand = new QueryCommand(query);
        queryCommand.Parameters.Add("$Group$", groupName);
        log.Debug($"{groupName}: {query}");
        // Executing the Query
        ICacheReader reader = cache.SearchService.ExecuteReader(queryCommand);
        // Read results if the result set is not empty
        if (reader.FieldCount > 0)
        {
            // Iterate through the results
            while (reader.Read())
            {
                //Loop through all the fields and print the field name and value
                // int totalFiled = reader.FieldCount;
                // for (int i = 0; i < totalFiled; i++)
                // {
                //     string fieldName = reader.GetName(i);
                //     string fieldValue = reader.GetValue<string>(i);
                //     log.Debug($"Field: {fieldName}, Value: {fieldValue}");
                // }

                string name = "";// reader.GetValue<string>("Name");
                string email = "";//reader.GetValue<string>("Email");
                string msisdn = "";//reader.GetValue<string>("Msisdn");
                Subscriber sub = reader.GetValue<Subscriber>("$VALUE$");
                if (logAllData)
                    log.Debug($"{sub}");
            }
        }
        else
        {
            log.Error($"No items found with GROUP: {groupName}");
        }
    }

    public bool AddWithGroup(string key, object value, string group)
    {
        try
        {
            CacheItem cacheItem = new CacheItem(value);
            cacheItem.Group = group;
            cache.Add(key, cacheItem);
            log.Debug($"Item {key} added successfully with group {group}, {value}");
            return true;
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item {key} with group {group}: {ex.Message}");
            return false;
        }
    }

    public bool UpdateGroup(string key, string newGroup)
    {
        try
        {
            CacheItem cacheItem = cache.GetCacheItem(key);
            string oldGroup = cacheItem.Group;
            if(oldGroup == newGroup)
            {
                log.Debug($"Item {key} already has group {newGroup}");
                return true;
            }
            cacheItem.Group = newGroup;
            cache.Insert(key, cacheItem);
            log.Debug($"Item {key} updated with new group {newGroup}, was: {oldGroup}");
            return true;
        }
        catch (Exception ex)
        {
            log.Error($"Error updating group for item {key}: {ex.Message}");
            return false;
        }
    }

    public void GetGroupKeys(string groupName){
        log.Debug($"Retreving keys from cache: {_cacheName} of group: {groupName}");
        try
        {
            ICollection<string> groupKeys = cache.SearchService.GetGroupKeys(groupName);
            foreach (var key in groupKeys)
            {
                log.Debug($"group: {groupName}, key: {key}");
            }
        }
        catch (Exception exp)
        {
            log.Error("GroupClient: Error in GetGroupKeys", exp);
        }
    }

    public IDictionary<string, Subscriber> GetAllGroupData(string groupName, bool logAllData = false)
    {
        log.Debug($"Retreving items from cache: {_cacheName} of group: {groupName}");
        try
        {
            IDictionary<string, Subscriber> groupAllDataDict = cache.SearchService.GetGroupData<Subscriber>(groupName);
            if (logAllData)
            {
                foreach (var (k, v) in groupAllDataDict)
                {
                    log.Debug($"group: {groupName}, key: {k}, value: {v}");
                }
            }
            return groupAllDataDict;
        }
        catch (Exception exp)
        {
            log.Error("GroupClient: Error in GetAllGroupData", exp);
            return null;
        }
    }

    public void RemoveGroupData(string groupName)
    {
        try
        {
            log.Debug($"Removing all items from  cache: {_cacheName} of group: {groupName}");
            cache.SearchService.RemoveGroupData(groupName);
        }
        catch (Exception exp)
        {
            log.Error("GroupClient: Error in RemoveGroupData", exp);
        }
    }
}
