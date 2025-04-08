using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class INSSConverter : ValueConverter<INSS, string>
{
    public INSSConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new INSS(identification))
    {
    }
}
