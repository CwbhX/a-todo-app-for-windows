using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Linework.Data.Converters;

public sealed class NullableDateTimeOffsetUnixMillisecondsConverter : ValueConverter<DateTimeOffset?, long?>
{
    public NullableDateTimeOffsetUnixMillisecondsConverter()
        : base(
            value => value.HasValue ? value.Value.ToUniversalTime().ToUnixTimeMilliseconds() : null,
            value => value.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(value.Value) : null)
    {
    }
}
