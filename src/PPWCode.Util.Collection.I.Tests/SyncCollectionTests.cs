using NUnit.Framework;

namespace PPWCode.Util.Collection.I.Tests;

[TestFixture]
public class SyncCollectionTests
{
    [Test]
    public void add_from_child_site()
    {
        // Arrange
        CollectionOwner owner = new ();
        CollectionItem item = new ();

        // Act
        item.Owner = owner;

        // Assert
        Assert.That(owner.Items, Has.Count.EqualTo(1), "The collection should contain one item.");
        Assert.That(owner.Items, Has.Member(item), "The collection should contain the item.");
        AssertBiDirectionality(owner, [item]);
    }

    [Test]
    public void add_from_owner_site()
    {
        // Arrange
        CollectionOwner owner = new ();
        CollectionItem item = new ();

        // Act
        owner.Items.Add(item);

        // Assert
        Assert.That(item.Owner, Is.EqualTo(owner), "The item should reference the owner.");
        AssertBiDirectionality(owner, [item]);
    }

    [Test]
    public void remove_from_child_site()
    {
        // Arrange
        CollectionOwner owner = new ();
        CollectionItem item =
            new ()
            {
                Owner = owner
            };

        // Act
        item.Owner = null;

        // Assert
        Assert.That(owner.Items, Is.Empty, "The collection should be empty.");
    }

    [Test]
    public void remove_from_owner_site()
    {
        // Arrange
        CollectionOwner owner = new ();
        CollectionItem item = new ();

        // Act
        owner.Items.Add(item);
        owner.Items.Remove(item);

        // Assert
        Assert.That(item.Owner, Is.Null, "The item should not reference the owner.");
        AssertBiDirectionality(owner);
    }

    [Test]
    public void adding_same_child_from_owner_site()
    {
        // Arrange
        CollectionOwner owner = new ();
        CollectionItem item = new ();

        // Act
        owner.Items.Add(item);
        owner.Items.Add(item);

        // Assert
        Assert.That(owner.Items, Has.Count.EqualTo(1), "The collection should contain one item.");
        AssertBiDirectionality(owner, [item]);
    }

    [Test]
    public void adding_same_child_from_child_site()
    {
        // Arrange
        CollectionOwner owner = new ();
        CollectionItem item = new ();

        // Act
        item.Owner = owner;
        item.Owner = owner;

        // Assert
        Assert.That(owner.Items, Has.Count.EqualTo(1), "The collection should contain one item.");
        AssertBiDirectionality(owner, [item]);
    }

    [Test]
    public void object_should_be_garbage_collected()
    {
        // Arrange
        WeakReference weakRef = CreateWeakReference();

        // Act
        ForceGarbageCollection();

        // Assert
        Assert.That(weakRef.IsAlive, Is.False, "The object should have been garbage collected.");
    }

    private WeakReference CreateWeakReference()
    {
        // Arrange
        CollectionOwner owner = new ();
        WeakReference ownerRef = new (owner);
        CollectionItem item = new ();
        owner.Items.Add(item);

        // Act
        item.Owner = new CollectionOwner();
        return ownerRef;
    }

    private void ForceGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private void AssertBiDirectionality(
        CollectionOwner owner,
        IEnumerable<CollectionItem>? items = null)
    {
        // Each Child should point to himself as a parent
        foreach (CollectionItem item in owner.Items)
        {
            Assert.That(owner, Is.EqualTo(item.Owner));
        }

        if (items is not null)
        {
            // * each of these items should point to the owner
            // * the owner should have all these items in his collection
            foreach (CollectionItem item in items)
            {
                Assert.That(owner, Is.EqualTo(item.Owner));
                Assert.That(owner.Items, Has.Member(item));
            }
        }
    }

    private class CollectionOwner
    {
        public CollectionOwner()
        {
            Items = new SyncCollection<CollectionItem, CollectionOwner>(this, (item, owner) => item.Owner = owner);
        }

        public SyncCollection<CollectionItem, CollectionOwner> Items { get; }
    }

    private class CollectionItem
    {
        private CollectionOwner? _owner;

        public CollectionOwner? Owner
        {
            get => _owner;
            set => SyncCollection<CollectionItem, CollectionOwner>.Set(this, ref _owner, value, x => x.Items);
        }
    }
}
