using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.France;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.France;

public class NIRConverter : ValueConverter<NIR, string>
{
    public NIRConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new NIR(identification))
    {
    }
}
