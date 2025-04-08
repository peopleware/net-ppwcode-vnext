using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class VATConverter : ValueConverter<VAT, string>
{
    public VATConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new VAT(identification))
    {
    }
}
