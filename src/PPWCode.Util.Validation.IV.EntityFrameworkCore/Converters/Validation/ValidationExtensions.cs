using Microsoft.EntityFrameworkCore;

using PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium;
using PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.France;
using PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Netherlands;
using PPWCode.Util.Validation.IV.European.Belgium;
using PPWCode.Util.Validation.IV.European.France;
using PPWCode.Util.Validation.IV.European.Netherlands;

using INSSConverter = PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation.European.Belgium.INSSConverter;

namespace PPWCode.Util.Validation.IV.EntityFrameworkCore.Converters.Validation;

public static class ValidationExtensions
{
    public static ModelConfigurationBuilder INSSConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<INSS>()
            .HaveConversion<INSSConverter>()
            .HaveMaxLength(new INSS(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder IBANConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<IBAN>()
            .HaveConversion<IBANConverter>()
            .HaveMaxLength(new IBAN(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder BICConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<BIC>()
            .HaveConversion<BICConverter>()
            .HaveMaxLength(new BIC(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder BBANConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<BBAN>()
            .HaveConversion<BBANConverter>()
            .HaveMaxLength(new BBAN(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder CompanyLocalUnitNumberConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<CompanyLocalUnitNumber>()
            .HaveConversion<CompanyLocalUnitNumberConverter>()
            .HaveMaxLength(new CompanyLocalUnitNumber(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder DMFAConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DMFA>()
            .HaveConversion<DMFAConverter>()
            .HaveMaxLength(new DMFA(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder KBOConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<INSS>()
            .HaveConversion<INSSConverter>()
            .HaveMaxLength(new INSS(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder OGMConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<OGM>()
            .HaveConversion<OGMConverter>()
            .HaveMaxLength(new OGM(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder RSZConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<RSZ>()
            .HaveConversion<RSZConverter>()
            .HaveMaxLength(new RSZ(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder TemporaryRSZConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<TemporaryRSZ>()
            .HaveConversion<TemporaryRSZConverter>()
            .HaveMaxLength(new TemporaryRSZ(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder VATConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<VAT>()
            .HaveConversion<VATConverter>()
            .HaveMaxLength(new VAT(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder NIRConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<NIR>()
            .HaveConversion<NIRConverter>()
            .HaveMaxLength(new NIR(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }

    public static ModelConfigurationBuilder BSNConvention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<BSN>()
            .HaveConversion<BSNConverter>()
            .HaveMaxLength(new BSN(null).StandardMaxLength)
            .AreUnicode(false);

        return configurationBuilder;
    }
}
