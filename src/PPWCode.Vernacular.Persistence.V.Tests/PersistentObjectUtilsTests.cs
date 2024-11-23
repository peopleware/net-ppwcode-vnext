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

using NUnit.Framework;

namespace PPWCode.Vernacular.Persistence.V.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class PersistentObjectUtilsTests
{
    [Test]
    public void can_set_id()
    {
        // Arrange
        Person person = new ();
        Assert.That(person.IdIsTransient, Is.True);

        // Act
        long expectedId = 1L;
        person.SetPersistentId(expectedId);

        // Assert
        Assert.That(person.IdIsTransient, Is.False);
        Assert.That(person.Id, Is.EqualTo(expectedId));
    }

    [Test]
    public void can_set_id_and_create_audit_fields()
    {
        // Arrange
        long expectedId = 1L;
        DateTimeOffset expectedNow = DateTimeOffset.Now;
        string expectedBy = "User";

        Person person = new ();
        Assert.That(person.IdIsTransient, Is.True);
        Assert.That(person.CreatedAt, Is.Null);
        Assert.That(person.CreatedBy, Is.Null);
        Assert.That(person.LastModifiedAt, Is.Null);
        Assert.That(person.LastModifiedBy, Is.Null);

        // Act
        person.SetIdAndCreateAuditProperties(expectedId, expectedNow, expectedBy);

        // Assert
        Assert.That(person.IdIsTransient, Is.False);
        Assert.That(person.Id, Is.EqualTo(expectedId));
        Assert.That(person.CreatedAt, Is.EqualTo(expectedNow));
        Assert.That(person.CreatedBy, Is.EqualTo(expectedBy));
        Assert.That(person.LastModifiedAt, Is.Null);
        Assert.That(person.LastModifiedBy, Is.Null);
    }

    [Test]
    public void can_set_last_modified_audit_fields()
    {
        // Arrange
        DateTimeOffset expectedNow = DateTimeOffset.Now;
        string expectedBy = "User";

        Person person = new ();
        Assert.That(person.IdIsTransient, Is.True);
        Assert.That(person.CreatedAt, Is.Null);
        Assert.That(person.CreatedBy, Is.Null);
        Assert.That(person.LastModifiedAt, Is.Null);
        Assert.That(person.LastModifiedBy, Is.Null);

        // Act
        person.SetLastModifiedProperties(expectedNow, expectedBy);

        // Assert
        Assert.That(person.IdIsTransient, Is.True);
        Assert.That(person.CreatedAt, Is.Null);
        Assert.That(person.CreatedBy, Is.Null);
        Assert.That(person.LastModifiedAt, Is.EqualTo(expectedNow));
        Assert.That(person.LastModifiedBy, Is.EqualTo(expectedBy));
    }
}
