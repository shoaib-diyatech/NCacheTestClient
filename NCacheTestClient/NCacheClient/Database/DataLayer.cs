namespace NCacheTestClient.NCacheClient.Database;

using global::NCacheClient;
using NCacheClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using log4net;

public class DataLayer
{
    private readonly string _connectionString;

    protected static readonly ILog log = LogManager.GetLogger(typeof(NCache));

    public DataLayer()
    {
        // Load connection string from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        _connectionString = config["ConnectionString"];
    }

    public bool IsConnected { get; private set; }

    private SqlConnection sqlConnection;

    public bool Connect()
    {
        try
        {
            // Query the database for a subscriber with the given msisdn
            sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            IsConnected = true;
            return IsConnected;
        }
        catch (Exception ex)
        {
            log.Error($"Error connecting to database: { ex.Message}, ConnectionString: {_connectionString}");
            return IsConnected;
        }
    }

    public void Dispose()
    {

    }

    public void AddData(string data)
    {
        // Logic to add data to the database
        Console.WriteLine($"Data added: {data}");
    }

    public bool TestConnection()
    {
        try
        {
            if (!IsConnected)
            {
                log.Error("Database connection is not established.");
                return false;
            }
            using var command = new SqlCommand(
               "SELECT top 1 FROM Subscriber", sqlConnection);
            using var reader = command.ExecuteReader();
            if(reader.HasRows)
                return true;
            else
                return false;

        }
        catch { return false; }
        }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msisdn"></param>
    /// <returns>null if no subscriber found or if database connection is not established</returns>
    public Subscriber GetData(string msisdn)
    {
        try
        {
            if (!IsConnected)
            {
                log.Error("Database connection is not established.");
                return null;
            }

            using var command = new SqlCommand(
                "SELECT Id, Msisdn, Name, Email, IsActive, DateOfBirth FROM Subscriber WHERE Msisdn = @msisdn",
                sqlConnection);
            command.Parameters.AddWithValue("@msisdn", msisdn);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var subscriber = new Subscriber
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Msisdn = reader.GetString(reader.GetOrdinal("Msisdn")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")))
                };
                return subscriber;
            }
            return null;
        }
        catch (Exception ex)
        {
            log.Error($"Error retrieving data: {ex.Message} against msisdn: {msisdn}");
            Console.WriteLine($"Error retrieving data: {ex.Message}");
            return null;
        }
    }
}

