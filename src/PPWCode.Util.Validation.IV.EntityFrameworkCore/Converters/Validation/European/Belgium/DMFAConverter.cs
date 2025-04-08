using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class DMFAConverter : ValueConverter<DMFA, string>
{
    public DMFAConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new DMFA(identification))
    {
    }
}
