using System.Collections.Concurrent;

namespace StockLedger.E2E.Tests.Helpers;

public static class TestDataStore
{
    private static readonly ConcurrentDictionary<string, object> Store = new();

    public static void Set(string key, object value)
    {
        Store.AddOrUpdate(key, value, (_, _) => value);
    }

    public static T Get<T>(string key)
    {
        if (!Store.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"TestDataStore does not contain key '{key}'.");

        if (value is T typed)
            return typed;

        // Handle JSON element conversions (Guid stored as string, etc.)
        return (T)Convert.ChangeType(value, typeof(T));
    }

    public static Guid GetGuid(string key)
    {
        if (!Store.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"TestDataStore does not contain key '{key}'.");

        return value switch
        {
            Guid g => g,
            string s => Guid.Parse(s),
            _ => Guid.Parse(value.ToString()!)
        };
    }
}
