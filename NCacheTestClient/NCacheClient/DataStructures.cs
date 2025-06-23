namespace NCacheClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DataStructures : NCache
{

    public DataStructures(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }

    public DataStructures(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
    }

}
