namespace NCacheClient;
using Alachisoft.NCache.Client;

[Serializable]
public class Car
{
    public string Name { get; set; }
    public string Category { get; set; }
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
        string group1 = "G1";
        string group2 = "G2";
        AddWithGroup(gk1, vg1, group1);
        AddWithGroup(gk2, vg2, group1);
        AddWithGroup(gk3, vg3, group1);
        AddWithGroup(gk4, vg4, group2);
        AddWithGroup(gk5, vg5, group2);
        GetAllGroupData(group1, true);
        GetAllGroupData(group2, true);
    }

    public bool AddWithGroup(string key, object value, string group)
    {
        try
        {
            // Car sub = new Car(){ Name = "+123456789"  };
            Subscriber sub = new Subscriber() { Msisdn = "+123456789", Id = 11111 };
            CacheItem cacheItem = new CacheItem(Subscriber.Serialize(sub));

            //CacheItem cacheItem = new CacheItem(value);
            cacheItem.Group = group;
            cache.Add(key, cacheItem);
            log.Debug($"Item {key} added successfully with group {group}");
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
