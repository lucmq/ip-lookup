namespace IpLookup.Api.Storage.InMemory.DataStructures;

internal class MemoryColumn<T>(int capacity) where T : notnull
{
    private readonly List<T> _items = new(capacity);
    private readonly Dictionary<T, int> _itemsKey = new(capacity);
    private int _key;

    public long Count => _items.Count;

    public T Get(int id)
    {
        return _items[id];
    }

    public int Add(T item)
    {
        if (_itemsKey.TryGetValue(item, out var key))
            return key;

        _items.Add(item);
        _itemsKey.Add(item, _key);

        return _key++;
    }
}