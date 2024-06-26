namespace IpLookup.Api.Storage.InMemory.DataStructures;

internal sealed class IntervalList<TKey, TValue>(int capacity)
    where TKey : IComparable
{
    private readonly List<TKey> _intervalStarts = new(capacity);
    private readonly List<TKey> _intervalEnds = new(capacity);
    private readonly List<TValue> _values = new(capacity);

    private TKey _lastKey = default!;

    public int Count => _intervalStarts.Count;

    public void Add(TKey start, TKey end, TValue value)
    {
        ValidateAddRequest(start, end);

        _intervalStarts.Add(start);
        _intervalEnds.Add(end);
        _values.Add(value);

        _lastKey = end;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var p = FindIntervalIndex(key);
        if (p == -1)
        {
            value = default!;
            return false;
        }

        value = _values[p];
        return true;
    }

    private void ValidateAddRequest(TKey start, TKey end)
    {
        if (end.CompareTo(start) < 0)
            throw new ArgumentException("Interval end must be greater than or " +
                                        "equal to the start");

        if (Count == 0)
            // Note (06-2024): The _lastKey field is started to default(TKey) and not
            // to null. Thus, it is necessary to check if the list is empty.
            return;

        if (_lastKey.CompareTo(start) > 0)
            throw new ArgumentException("Overlapping intervals are not allowed");
    }

    private int FindIntervalIndex(TKey key)
    {
        var p = FindLowerThanOrEqualIndex(_intervalStarts, key);
        if (p < 0)
            return -1;

        var rangeEnd = _intervalEnds[p];
        if (rangeEnd.CompareTo(key) < 0)
            // The key being searched falls between two indexed intervals.
            return -1;

        return p;
    }

    private static int FindLowerThanOrEqualIndex<T>(List<T> values, T value)
    {
        var index = values.BinarySearch(value);
        if (index >= 0)
            return index;

        // If the input value is not found, BinarySearch returns
        // the bitwise complement of the index of the next
        // larger element.
        index = ~index;

        // Index out of range.
        if (index == 0)
            return -1;

        // Input value found, return the previous element.
        return index - 1;
    }
}