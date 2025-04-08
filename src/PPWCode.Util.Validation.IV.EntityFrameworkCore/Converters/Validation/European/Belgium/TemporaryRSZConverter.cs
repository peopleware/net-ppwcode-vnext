using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class TemporaryRSZConverter : ValueConverter<TemporaryRSZ, string>
{
    public TemporaryRSZConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new TemporaryRSZ(identification))
    {
    }
}
