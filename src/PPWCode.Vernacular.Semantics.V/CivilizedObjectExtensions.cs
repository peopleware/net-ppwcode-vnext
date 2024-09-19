using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Vernacular.Semantics.V;

public static class CivilizedObjectExtensions
{
    public static void CheckForWildExceptions(this ICivilizedObject? civilizedObject, CompoundSemanticException cse)
    {
        if (civilizedObject is not null)
        {
            cse.AddElement(civilizedObject.WildExceptions());
        }
    }
}
