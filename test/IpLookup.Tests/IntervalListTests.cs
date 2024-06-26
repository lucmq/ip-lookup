using IpLookup.Api.Storage.InMemory.DataStructures;

namespace IpLookup.Api.Tests;

public class IntervalListTests
{
    [Fact]
    public void Add_Interval_AddsCorrectly()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        var start = 1;
        var end = 5;
        var value = "Test Value";

        // Act
        intervalList.Add(start, end, value);

        // Assert
        var get = intervalList.TryGetValue;
        Assert.Equal(1, intervalList.Count);
        Assert.Equal(value, get(start, out var result) ? result : null);
        Assert.Equal(value, get(end, out var result2) ? result2 : null);
        Assert.Equal(value, get(start + 1, out var result3) ? result3 : null);
        Assert.Equal(value, get(end - 1, out var result4) ? result4 : null);
    }

    [Fact]
    public void TryGetValue_ReturnsCorrectValue()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        var key = 3;
        var expectedValue = "Test Value";
        intervalList.Add(1, 5, expectedValue);

        // Act
        var result = intervalList.TryGetValue(key, out string value);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void Add_ThrowsException_WhenIntervalEndIsLessThanStart()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        var start = 5;
        var end = 1;
        var value = "Invalid Interval";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => intervalList.Add(start, end, value));
    }

    [Fact]
    public void Add_ThrowsException_WhenIntervalsOverlap()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        intervalList.Add(1, 5, "First Interval");
        var start = 3; // This start is within the interval [1, 5]
        var end = 6;
        var value = "Overlapping Interval";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => intervalList.Add(start, end, value));
    }

    [Fact]
    public void TryGetValue_ReturnsFalse_WhenKeyNotPresent()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        intervalList.Add(1, 5, "Test Value");
        var key = 6;

        // Act
        var result = intervalList.TryGetValue(key, out string value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void Count_ReturnsCorrectNumberOfIntervals()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        intervalList.Add(1, 2, "First");
        intervalList.Add(3, 4, "Second");
        intervalList.Add(5, 6, "Third");

        // Act
        var count = intervalList.Count;

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public void Add_EnforcesSortedOrder()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        intervalList.Add(5, 10, "Later");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => intervalList.Add(1, 4, "Earlier"));
    }

    [Fact]
    public void Add_ThrowsException_WhenAddingDuplicateInterval()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        intervalList.Add(1, 5, "Existing Interval");

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => intervalList.Add(1, 5, "Duplicate Interval"));
    }

    [Fact]
    public void Add_Interval_WithDateTimeKeys()
    {
        // Arrange
        var intervalList = new IntervalList<DateTime, string>(10);
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(1);

        // Act
        intervalList.Add(startTime, endTime, "Time Interval");

        // Assert
        var exists = intervalList
            .TryGetValue(startTime.AddMinutes(30), out var value);

        Assert.True(exists);
        Assert.Equal("Time Interval", value);
    }

    [Fact]
    public void Add_Interval_WithZeroLength()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        var start = 1;
        var end = 1;
        var value = "Zero-Length Interval";

        // Act
        intervalList.Add(start, end, value);

        // Assert
        Assert.Equal(1, intervalList.Count);
    }

    [Fact]
    public void Add_Interval_HandlesLargeIntervals()
    {
        // Arrange
        var intervalList = new IntervalList<int, string>(10);
        var start = int.MinValue;
        var end = int.MaxValue;

        // Act
        intervalList.Add(start, end, "Maximum Range");

        // Assert
        Assert.Equal(1, intervalList.Count);
    }
}