namespace NCacheClient;

public class PartitionClient : NCache
{

    public PartitionClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    long id = 0;//long.MinValue;
    string keyPrefix = "";
    int delayInMs = 0;
    int ttlInSecs = 5;

    public override void Test()
    {
        //cache.Clear();
        // AddWIthExpiry();
        LoadTest();
    }

    public void AddWIthExpiry()
    {
        Add(keyPrefix + ++id, keyPrefix + id, ttlInSecs);
    }

    public void LoadTest()
    {
        Thread t1 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, ttlInSecs);

            }
        });
        Thread t2 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, ttlInSecs);

            }
        });
        Thread t3 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, ttlInSecs);

            }
        });
        Thread t4 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, ttlInSecs);

            }
        });

        t1.Start();
        t2.Start();
        t3.Start();
        t4.Start();
    }

}