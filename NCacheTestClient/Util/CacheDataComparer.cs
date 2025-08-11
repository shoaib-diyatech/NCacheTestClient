namespace NCacheTestClient.Util;

using Alachisoft.NCache.Client;
using System.Collections;
using log4net;

internal class CacheDataComparer
{

    protected static readonly ILog log = LogManager.GetLogger(typeof(CacheDataComparer));

    public static bool CompareCacheData(ICache cache1, ICache cache2)
    {
        if (cache1 == null || cache2 == null)
        {
            throw new ArgumentNullException("Caches cannot be null.");
        }

        IEnumerator cache1Enumerator = cache1.GetEnumerator();
        return CompareCacheData(cache1Enumerator, cache2);
    }

    public static bool CompareCacheData(IEnumerator cache1Enumerator, ICache cache2)
    {
        if (cache1Enumerator == null || cache2 == null)
        {
            throw new ArgumentNullException("Cache enumerators cannot be null.");
        }
        while (cache1Enumerator.MoveNext())
        {
            DictionaryEntry id = (DictionaryEntry)cache1Enumerator.Current;

            string cache2value = cache2.Get<string>(id.Key.ToString());

            log.Info($"Key: {id.Key}, Cache1Value: {id.Value}, Cache2Value: {cache2value}");
            if (id.Value == null && cache2value == null)
            {
                log.Info($"Key: {id.Key} has no value in both caches.");
                continue; // Both values are null, skip to next entry
            }
            if (id.Value as string == cache2value)
            {
                log.Debug($"Key: {id.Key} matches with value: {id.Value}, Value: {id.Value}");
            }
            else
            {
                log.Error($"Key: {id.Key} does not match. Cache1 Value: {id.Value}, Cache2 Value: {cache2value}");
                return false; // Data mismatch found
            }
        }
        log.Info("All cache entries match between the two caches.");
        return true;
    }

    /// <summary>
    /// Compares two caches and logs:
    /// - The count of items in cache1 not present in cache2.
    /// - The count and keys of items that match (same key and value).
    /// - The count and keys of items in cache2 not present in cache1 or with different values.
    /// </summary>
    public static void CompareAndReportCacheDifferences(ICache cache1, ICache cache2)
    {
        long totalEntriesInCache1 = cache1.Count;
        long totalEntriesInCache2 = cache2.Count;
        if (cache1 == null || cache2 == null)
            throw new ArgumentNullException("Caches cannot be null.");

        var cache1Enumerator = cache1.GetEnumerator();
        var cache1Keys = new HashSet<string>();
        var cache2Keys = new HashSet<string>();
        var matchingKeys = new List<string>();
        var cache1NotInCache2 = new List<string>();
        var cache2NotInCache1OrDifferent = new List<string>();

        // Build cache2 key set
        var cache2Enumerator = cache2.GetEnumerator();
        while (cache2Enumerator.MoveNext())
        {
            var entry = (DictionaryEntry)cache2Enumerator.Current;
            cache2Keys.Add(entry.Key.ToString());
        }

        // Compare cache1 against cache2
        while (cache1Enumerator.MoveNext())
        {
            var entry = (DictionaryEntry)cache1Enumerator.Current;
            string key = entry.Key.ToString();
            cache1Keys.Add(key);

            var cache2Value = cache2.Get<object>(key);

            if (!cache2Keys.Contains(key))
            {
                cache1NotInCache2.Add(key);
            }
            else
            {
                // Compare values (using .Equals for generality)
                if ((entry.Value == null && cache2Value == null) ||
                    (entry.Value != null && entry.Value.Equals(cache2Value)))
                {
                    matchingKeys.Add(key);
                }
                else
                {
                    cache2NotInCache1OrDifferent.Add(key);
                }
            }
        }

        // Find keys in cache2 not in cache1
        foreach (var key in cache2Keys)
        {
            if (!cache1Keys.Contains(key))
            {
                cache2NotInCache1OrDifferent.Add(key);
            }
        }

        log.Info($"Total items in cache1: {totalEntriesInCache1}");
        log.Info($"Total items in cache2: {totalEntriesInCache2}");

        log.Info($"Items in cache1 not present in cache2: {cache1NotInCache2.Count}");
        if (cache1NotInCache2.Count > 0)
            log.Debug($"Keys: {string.Join(", ", cache1NotInCache2)}");

        log.Info($"Items that match in both caches: {matchingKeys.Count}");
        if (matchingKeys.Count > 0)
            log.Debug($"Keys: {string.Join(", ", matchingKeys)}");

        log.Info($"Items in cache2 not present in cache1 or with different values: {cache2NotInCache1OrDifferent.Count}");
        if (cache2NotInCache1OrDifferent.Count > 0)
            log.Debug($"Keys: {string.Join(", ", cache2NotInCache1OrDifferent)}");
    }
}