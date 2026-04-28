using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Linework.Data.Converters;

public sealed class DateTimeOffsetUnixMillisecondsConverter : ValueConverter<DateTimeOffset, long>
{
    public DateTimeOffsetUnixMillisecondsConverter()
        : base(
            value => value.ToUniversalTime().ToUnixTimeMilliseconds(),
            value => DateTimeOffset.FromUnixTimeMilliseconds(value))
    {
    }
}
