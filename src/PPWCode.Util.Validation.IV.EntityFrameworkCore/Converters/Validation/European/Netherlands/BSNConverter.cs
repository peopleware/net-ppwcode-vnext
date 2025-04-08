using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Netherlands;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Netherlands;

public class BSNConverter : ValueConverter<BSN, string>
{
    public BSNConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new BSN(identification))
    {
    }
}
