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
public class Persistent_IsSame_Tests : BaseFixture
{
    [Test]
    public void persistent_is_equal_to_himself()
    {
        PersistentFoo x = new () { Id = 1 };

        Assert.That(x.IsSame(x), Is.True);
    }

    [Test]
    public void persistent_is_equal_to_persistent_with_same_identity()
    {
        PersistentFoo x = new () { Id = 1 };
        PersistentFoo y = new () { Id = 1 };

        Assert.That(x.IsSame(y), Is.True);
    }

    [Test]
    public void persistent_is_equal_to_persistent_with_same_identity_subclass()
    {
        PersistentFoo x = new () { Id = 1 };
        PersistentSpecializedFoo y = new () { Id = 1 };

        Assert.That(x.IsSame(y), Is.True);
    }

    [Test]
    public void persistent_is_not_equal_to_null()
    {
        PersistentFoo x = new () { Id = 1 };

        Assert.That(x.IsSame(null), Is.False);
    }

    [Test]
    public void persistent_is_not_equal_to_persistent_with_differrent_identity()
    {
        PersistentFoo x = new () { Id = 1 };
        PersistentFoo y = new () { Id = 2 };

        Assert.That(x.IsSame(y), Is.False);
    }

    [Test]
    public void transient_is_equal_to_himself()
    {
        PersistentFoo x = new ();

        Assert.That(x.IsSame(x), Is.True);
    }

    [Test]
    public void transient_is_not_equal_to_null()
    {
        PersistentFoo x = new ();

        Assert.That(x.IsSame(null), Is.False);
    }

    [Test]
    public void transient_is_not_equal_to_transient()
    {
        PersistentFoo x = new ();
        PersistentFoo y = new ();

        Assert.That(x.IsSame(y), Is.False);
    }
}
