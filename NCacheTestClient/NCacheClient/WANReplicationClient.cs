namespace NCacheClient;
internal class WANReplicationClient : NCache
{
    public WANReplicationClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }
    public WANReplicationClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }
    public override void Test()
    {
        // Implement WAN replication test logic here
        // For example, you can create a cache and replicate data across WAN
        Console.WriteLine("WAN Replication Test is not implemented yet.");
    }
}