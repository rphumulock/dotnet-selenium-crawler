
public class WorkflowContext
{
    private readonly Dictionary<string, object> _contextData = new Dictionary<string, object>();

    public void Set<T>(string key, T value)
    {
        _contextData[key] = value;
    }

    public T Get<T>(string key)
    {
        if (_contextData.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        throw new KeyNotFoundException($"Key '{key}' not found in the context.");
    }

    public T GetOrDefault<T>(string key, T defaultValue = default)
    {
        if (_contextData.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return defaultValue;
    }
}
