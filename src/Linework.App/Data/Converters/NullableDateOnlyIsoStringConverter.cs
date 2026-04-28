using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Linework.Data.Converters;

public sealed class NullableDateOnlyIsoStringConverter : ValueConverter<DateOnly?, string?>
{
    public NullableDateOnlyIsoStringConverter()
        : base(
            value => value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
            value => value == null ? null : DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture))
    {
    }
}
