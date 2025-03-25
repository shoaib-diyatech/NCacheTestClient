namespace NCacheClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLoader.ServerSide;



public class CacheLoaderTest
{
    public void Test()
    {
        SubscriberLoader subscriberLoader = new SubscriberLoader();
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("connectionString", "TestConnectionString");
        parameters.Add("LogFilePath", @"D:\logs\ClassLoader");
        subscriberLoader.Init(parameters, "SKOnly");

    }
}
