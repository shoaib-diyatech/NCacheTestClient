namespace NCacheClient;

public class PartitionClient : NCache
{

    public PartitionClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    long id = long.MinValue;
    string keyPrefix = "P1";

    public override void Test()
    {
        LoadTest();
    }

    public void LoadTest()
    {
        Thread t1 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, 5);

            }
        });
        Thread t2 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, 5);

            }
        });
        Thread t3 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, 5);

            }
        });
        Thread t4 = new Thread(() =>
        {
            while (true)
            {
                Add(keyPrefix + ++id, keyPrefix + id, 5);

            }
        });

        t1.Start();
        t2.Start();
        t3.Start();
        t4.Start();
    }

}