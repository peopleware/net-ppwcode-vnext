using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class KBOConverter : ValueConverter<KBO, string>
{
    public KBOConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new KBO(identification))
    {
    }
}
