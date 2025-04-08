using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class OGMConverter : ValueConverter<OGM, string>
{
    public OGMConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new OGM(identification))
    {
    }
}
