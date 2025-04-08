using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class BBANConverter : ValueConverter<BBAN, string>
{
    public BBANConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new BBAN(identification))
    {
    }
}
