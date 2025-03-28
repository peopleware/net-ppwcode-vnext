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

using PPWCode.Vernacular.Semantics.V;

namespace PPWCode.Vernacular.Persistence.V;

public abstract class PersistentObject<TId>
    : CivilizedObject,
      IPersistentObject<TId>,
      IEquatable<PersistentObject<TId>>
    where TId : IEquatable<TId>
{
    private int? _oldHashCode;

    public virtual TId? Id { get; init; }

    public virtual bool IdIsTransient
        => EqualityComparer<TId?>.Default.Equals(Id, default);

    public virtual TOther? As<TOther>()
        where TOther : class, IPersistentObject<TId>
        => this as TOther;

    public virtual bool IsSame(IIdentity<TId>? other)
        => IsSame(other as PersistentObject<TId>);

    public virtual bool IsSame(PersistentObject<TId>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (IdIsTransient || other.IdIsTransient)
        {
            return false;
        }

        if (!EqualityComparer<TId>.Default.Equals(Id, other.Id))
        {
            return false;
        }

        // Dependent on the lazy loading implementation of the used ORM mapper,
        // we need to be less restrictive and allow subtypes.
        Type otherType = other.GetType();
        Type thisType = GetType();
        return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
    }

    /// <inheritdoc />
    public bool Equals(PersistentObject<TId>? other)
        => IsSame(other);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => IsSame(obj as PersistentObject<TId>);

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Reviewed")]
    [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode", Justification = "Reviewed")]
    public override int GetHashCode()
    {
        // Once we have a hash code we'll never change it
        if (_oldHashCode != null)
        {
            return _oldHashCode.Value;
        }

        // When this instance is transient, we use the base GetHashCode()
        // and remember it, so an instance can NEVER change its hash code.
        if (IdIsTransient)
        {
            _oldHashCode = base.GetHashCode();
            return _oldHashCode.Value;
        }

        return Id is null ? 0 : Id.GetHashCode();
    }

    public static bool operator ==(PersistentObject<TId>? left, PersistentObject<TId>? right)
        => Equals(left, right);

    public static bool operator !=(PersistentObject<TId>? left, PersistentObject<TId>? right)
        => !Equals(left, right);
}
