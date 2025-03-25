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
        string gk1 = "gk333333333333333333331";
        string vg1 = "vk1";
        string gk2 = "gk2";
        string vg2 = "vg2";
        string gk3 = "gk3";
        string vg3 = "vg3";
        string gk4 = "gk4";
        string vg4 = "vg4";
        string gk5 = "gk5";
        string vg5 = "vg5";
        string HighRPU = Group.HighRPU.ToString();
        string VIP = Group.VIP.ToString();
        string LowRPU = Group.LowRPU.ToString();
        cache.Clear();
        AddWithGroup(gk1, vg1, HighRPU);
        AddWithGroup(gk2, vg2, HighRPU);
        AddWithGroup(gk3, vg3, HighRPU);
        AddWithGroup(gk4, vg4, LowRPU);
        AddWithGroup(gk5, vg5, LowRPU);
        AddWithGroup(gk5, vg5, LowRPU);
        AddWithGroup(gk5, vg5, VIP);
        AddWithGroup(gk5, vg5, VIP);
        AddWithGroup(gk5, vg5, VIP);
        //GetAllGroupData(HighRPU, true);
        //GetAllGroupData(VIP, true);
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
        key = "";
        try
        {
            //Car sub = new Car(){ Name = "+123456789" };
            Subscriber sub = Subscriber.GetRandomSubscriber();
            //CacheItem cacheItem = new CacheItem(Subscriber.Serialize(sub));
            CacheItem cacheItem = new CacheItem(sub);

            //CacheItem cacheItem = new CacheItem(sub);
            cacheItem.Group = group;
            key = sub.Msisdn;
            cache.Add(key, cacheItem);
            log.Debug($"Item {key} added successfully with group {group}, {sub}");
            return true;
        }
        catch (Exception ex)
        {
            log.Error($"Error inserting item {key} with group {group}: {ex.Message}");
            return false;
        }
    }

    public IDictionary<string, string> GetAllGroupData(string groupName, bool logAllData = false)
    {
        log.Debug($"Retreving items from cache: {_cacheName} of group: {groupName}");
        try
        {
            IDictionary<string, string> groupAllDataDict = cache.SearchService.GetGroupData<string>(groupName);
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

        }
    }
}
