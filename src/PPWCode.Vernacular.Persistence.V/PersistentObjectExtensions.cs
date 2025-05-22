namespace PPWCode.Vernacular.Persistence.V;

public static class PersistentObjectExtensions
{
    public static TParent? GetParentWithId<TId, TChild, TParent>(this TChild? child, Func<TChild, TParent?> getParent, TId parentId)
        where TChild : IPersistentObject<TId>
        where TParent : IPersistentObject<TId>
        where TId : IEquatable<TId>
    {
        if (child == null)
        {
            return default;
        }

        TParent? parent = getParent(child);
        if ((parent == null) || parent.Id is null)
        {
            return default;
        }

        return parent.Id.Equals(parentId) ? parent : default;
    }
}
