using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class RSZConverter : ValueConverter<RSZ, string>
{
    public RSZConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new RSZ(identification))
    {
    }
}
