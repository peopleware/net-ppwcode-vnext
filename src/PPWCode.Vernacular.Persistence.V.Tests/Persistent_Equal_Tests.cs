// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace PPWCode.Vernacular.Persistence.V.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Tests")]
public class Persistent_Equal_Tests : BaseFixture
{
    private void equal_asserts<T>(PersistentObject<T> x, PersistentObject<T>? y)
        where T : IEquatable<T>
    {
        Assert.That(x.Equals(y), Is.True);
        Assert.That(x == y, Is.True);
        Assert.That(x != y, Is.False);
    }

    private void not_equal_asserts<T>(PersistentObject<T> x, PersistentObject<T>? y)
        where T : IEquatable<T>
    {
        Assert.That(x.Equals(y), Is.False);
        Assert.That(x == y, Is.False);
        Assert.That(x != y, Is.True);
    }

    [Test]
    public void persistent_is_equal_to_himself()
    {
        PersistentFoo x = new () { Id = 1 };

        equal_asserts(x, x);
    }

    [Test]
    public void persistent_is_equal_to_persistent_with_same_identity()
    {
        PersistentFoo x = new () { Id = 1 };
        PersistentFoo y = new () { Id = 1 };

        equal_asserts(x, y);
    }

    [Test]
    public void persistent_is_equal_to_persistent_with_same_identity_subclass()
    {
        PersistentFoo x = new () { Id = 1 };
        PersistentSpecializedFoo y = new () { Id = 1 };

        equal_asserts(x, y);
    }

    [Test]
    public void persistent_is_not_equal_to_null()
    {
        PersistentFoo x = new () { Id = 1 };

        not_equal_asserts(x, null);
    }

    [Test]
    public void persistent_is_not_equal_to_persistent_with_differrent_identity()
    {
        PersistentFoo x = new () { Id = 1 };
        PersistentFoo y = new () { Id = 2 };

        not_equal_asserts(x, y);
    }

    [Test]
    public void transient_is_equal_to_himself()
    {
        PersistentFoo x = new ();

        equal_asserts(x, x);
    }

    [Test]
    public void transient_is_not_equal_to_null()
    {
        PersistentFoo x = new ();

        not_equal_asserts(x, null);
    }

    [Test]
    public void transient_is_not_equal_to_transient()
    {
        PersistentFoo x = new ();
        PersistentFoo y = new ();

        not_equal_asserts(x, y);
    }
}
