public class WorkflowContext
{
    private readonly Dictionary<string, object> _contextData = new Dictionary<string, object>();

    // Method to add data to the context
    public void Set<T>(string key, T value)
    {
        _contextData[key] = value;
    }

    // Method to retrieve data from the context
    public T Get<T>(string key)
    {
        if (_contextData.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        throw new KeyNotFoundException($"Key '{key}' not found in the context.");
    }
}
