using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;

public class CompanyLocalUnitNumberConverter : ValueConverter<CompanyLocalUnitNumber, string>
{
    public CompanyLocalUnitNumberConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new CompanyLocalUnitNumber(identification))
    {
    }
}
