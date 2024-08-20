using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public static class ComplexTypeExtensions
{
    public static void CheckForWildExceptions(this ComplexType? complexType, CompoundSemanticException cse)
    {
        if (complexType is not null)
        {
            cse.AddElement(complexType.WildExceptions());
        }
    }
}
