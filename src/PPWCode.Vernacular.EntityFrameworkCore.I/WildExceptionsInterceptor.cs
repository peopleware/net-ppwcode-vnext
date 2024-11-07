using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Semantics.V;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public class WildExceptionsInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            CompoundSemanticException cse = Validate(eventData.Context);
            if (!cse.IsEmpty)
            {
                throw cse;
            }
        }
        else
        {
            throw new ProgrammingError("Expected context to be initialized.");
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private CompoundSemanticException Validate(DbContext dbContext)
    {
        CompoundSemanticException cse = new ();

        IEnumerable<ICivilizedObject> civilizedObjects =
            dbContext
                .ChangeTracker
                .Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified)
                .Select(e => e.Entity)
                .OfType<ICivilizedObject>();
        foreach (ICivilizedObject civilizedObject in civilizedObjects)
        {
            cse.AddElement(civilizedObject.WildExceptions());
        }

        return cse;
    }
}
