namespace NCacheClient;
using System;
using Newtonsoft.Json;

//[Serializable]
public class Subscriber
{
    public string Name { get; set; }
    public string Category { get; set; }

    public Gender gender { get; set; }
    public string Email { get; set; }
    public string Msisdn { get; set; }
    //public ServiceType ServiceType { get; set; }
    public long Id { get; set; }

    /// <summary>
    /// Serialize the Subscriber object to JSON
    /// </summary>
    /// <param name="subscriber"></param>
    /// <returns></returns>
    public static string Serialize(Subscriber subscriber)
    {
        return JsonConvert.SerializeObject(subscriber);
    }

    /// <summary>
    /// Deserialize JSON to a Subscriber object
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Subscriber Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<Subscriber>(json);
    }
}

[Serializable]
public enum ServiceType
{
    Postpaid,
    Prepaid
}

public enum Gender{
    Male,
    Female
}