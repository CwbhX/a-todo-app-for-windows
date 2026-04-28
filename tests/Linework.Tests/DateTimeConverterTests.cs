using Linework.Data.Converters;
using Xunit;

namespace Linework.Tests;

public sealed class DateTimeConverterTests
{
    [Fact]
    public void DateTimeOffset_converter_stores_utc_unix_milliseconds()
    {
        var converter = new DateTimeOffsetUnixMillisecondsConverter();
        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();
        var localValue = new DateTimeOffset(2026, 4, 27, 8, 30, 0, TimeSpan.FromHours(-7));

        var stored = toProvider(localValue);
        var restored = fromProvider(stored);

        Assert.Equal(localValue.ToUniversalTime().ToUnixTimeMilliseconds(), stored);
        Assert.Equal(localValue.ToUniversalTime(), restored);
    }

    [Fact]
    public void Nullable_DateTimeOffset_converter_round_trips_values_and_nulls()
    {
        var converter = new NullableDateTimeOffsetUnixMillisecondsConverter();
        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();
        var localValue = new DateTimeOffset(2026, 4, 27, 8, 30, 0, TimeSpan.FromHours(-7));

        var stored = toProvider(localValue);
        var restored = fromProvider(stored);

        Assert.Equal(localValue.ToUniversalTime().ToUnixTimeMilliseconds(), stored);
        Assert.Equal(localValue.ToUniversalTime(), restored);
        Assert.Null(toProvider(null));
        Assert.Null(fromProvider(null));
    }

    [Fact]
    public void Nullable_DateOnly_converter_round_trips_iso_dates_and_nulls()
    {
        var converter = new NullableDateOnlyIsoStringConverter();
        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();
        var date = new DateOnly(2026, 4, 27);

        var stored = toProvider(date);
        var restored = fromProvider(stored);

        Assert.Equal("2026-04-27", stored);
        Assert.Equal(date, restored);
        Assert.Null(toProvider(null));
        Assert.Null(fromProvider(null));
    }
}
