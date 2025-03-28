namespace NCacheClient;

public class AsyncClient : NCache
{
    public AsyncClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {
    }

    public AsyncClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        string addKey = "addKey";
        string addValue = "addValue";
        log.Debug($"Adding key: {addKey} and value: {addValue} through AddAsync");
        TestAddAsync(addKey, addValue);

        string insertKey = "insertKey";
        string insertValue = "insertValue";
        log.Debug($"Inserting key: {insertKey} and value: {insertValue} through InsertAsync");
        TestInsertAsync(insertKey, insertValue);

    }

    public void TestAddAsync(string key, object value)
    {
        Task awaitable = cache.AddAsync(key, value);
        log.Debug($"Async Add call sent for key: {key} and value: {value}");

        // Attaching a continuation to handle the completion of the task
        awaitable.ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                log.Debug($"Async Add call completed for key: {key} and value: {value}");
                OnAddCompleted(key, value);
            }
            else if (task.IsFaulted)
            {
                log.Error($"Async Add call failed for key: {key} and value: {value}");
            }
        });
        log.Debug($"Exiting TestAddAsync after attaching callback for key: {key} and value: {value}");
    }

    public void TestInsertAsync(string key, object value)
    {
        Task awaitable = cache.InsertAsync(key, value);
        log.Debug($"Async Insert call sent for key: {key} and value: {value}");

        // Attaching a continuation to handle the completion of the task
        awaitable.ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                log.Debug($"Async Insert call completed for key: {key} and value: {value}");
                OnInsertCompleted(key, value);
            }
            else if (task.IsFaulted)
            {
                log.Error($"Async Insert call failed for key: {key} and value: {value}");
            }
        });
    }

    public void OnAddCompleted(string key, object value)
    {
        log.Debug($"OnAddCompleted called for key: {key} and value: {value}");
    }

    public void OnInsertCompleted(string key, object value)
    {
        log.Debug($"OnInsertCompleted called for key: {key} and value: {value}");
    }
}