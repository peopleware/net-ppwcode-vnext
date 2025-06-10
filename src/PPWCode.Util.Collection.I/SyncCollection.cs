using System.Collections;

namespace PPWCode.Util.Collection.I;

/// <summary>
///     Collection that keeps the back reference to the owner synchronized.
/// </summary>
/// <typeparam name="TCollectionItem">Items in the collection</typeparam>
/// <typeparam name="TCollectionOwner">Owner of the collection</typeparam>
public class SyncCollection<TCollectionItem, TCollectionOwner> : ICollection<TCollectionItem>
    where TCollectionItem : class
    where TCollectionOwner : class
{
    private readonly HashSet<TCollectionItem> _items;
    private readonly TCollectionOwner _owner;
    private readonly Action<TCollectionItem, TCollectionOwner?> _setAction;

    public SyncCollection(
        TCollectionOwner owner,
        Action<TCollectionItem, TCollectionOwner?> setAction,
        IEqualityComparer<TCollectionItem>? comparer = null)
    {
        _items = new HashSet<TCollectionItem>(comparer);
        _owner = owner;
        _setAction = setAction;
    }

    /// <inheritdoc />
    public IEnumerator<TCollectionItem> GetEnumerator()
        => _items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc />
    public void Add(TCollectionItem item)
    {
        if (_items.Add(item))
        {
            _setAction(item, _owner);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        foreach (TCollectionItem item in _items)
        {
            _setAction(item, null);
        }

        _items.Clear();
    }

    /// <inheritdoc />
    public bool Contains(TCollectionItem item)
        => _items.Contains(item);

    /// <inheritdoc />
    public void CopyTo(TCollectionItem[] array, int arrayIndex)
        => _items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(TCollectionItem item)
    {
        if (_items.Remove(item))
        {
            _setAction(item, null);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public int Count
        => _items.Count;

    /// <inheritdoc />
    public bool IsReadOnly
        => false;

    /// <summary>
    ///     Set the backing field, and update the collections of the previous and next owner.
    /// </summary>
    /// <param name="current">Current class instance (this)</param>
    /// <param name="previousValue">Reference to the backing field</param>
    /// <param name="newValue">New value for the backing field</param>
    /// <param name="getCollection">Reference to the collection for a certain owner</param>
    public static void Set(
        TCollectionItem current,
        ref TCollectionOwner? previousValue,
        TCollectionOwner? newValue,
        Func<TCollectionOwner, SyncCollection<TCollectionItem, TCollectionOwner>> getCollection)
    {
        if (previousValue == newValue)
        {
            return;
        }

        if (previousValue != null)
        {
            TCollectionOwner prev = previousValue;
            previousValue = null;
            getCollection(prev)._items.Remove(current);
        }

        if (newValue != null)
        {
            getCollection(newValue)._items.Add(current);
        }

        previousValue = newValue;
    }
}
