using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation;

public class IBANConverter : ValueConverter<IBAN, string>
{
    public IBANConverter()
        : base(
            identification => identification.CleanedVersion,
            identification => new IBAN(identification))
    {
    }
}
