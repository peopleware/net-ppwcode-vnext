using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Semantics.V;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public abstract class ComplexType : CivilizedObject
{
    public abstract bool IsEmpty { get; }

    /// <inheritdoc />
    public sealed override CompoundSemanticException WildExceptions()
    {
        if (IsEmpty)
        {
            return new CompoundSemanticException();
        }

        CompoundSemanticException cse = base.WildExceptions();
        OnComplexTypeWildExceptions(cse);

        return cse;
    }

    protected abstract void OnComplexTypeWildExceptions(CompoundSemanticException cse);
}
