using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation;

public class BICConverter : ValueConverter<BIC, string>
{
    public BICConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new BIC(identification))
    {
    }
}
