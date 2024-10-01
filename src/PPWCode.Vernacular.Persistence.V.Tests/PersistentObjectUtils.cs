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

using System.Reflection;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Vernacular.Persistence.V.Tests;

public static class PersistentObjectUtils
{
    private static void SetProperty(object target, string fieldName, object value)
    {
        Type? type = target.GetType();
        PropertyInfo? propertyInfo = null;

        while (type != null)
        {
            propertyInfo = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (propertyInfo != null)
            {
                break;
            }

            type = type.BaseType;
        }

        if (propertyInfo != null)
        {
            propertyInfo.SetValue(target, value);
        }
    }

    public static void SetPersistentId<TId>(this IPersistentObject<TId> persistentObject, TId id)
        where TId : struct, IEquatable<TId>
    {
        if (persistentObject == null)
        {
            throw new InternalProgrammingError("Cannot set persistence id on null object.");
        }

        SetProperty(persistentObject, nameof(IPersistentObject<TId>.Id), id);
    }

    public static void SetIdAndCreateAuditProperties<TId, T>(
        this IPersistentObject<TId>? persistentObject,
        TId id,
        T now,
        string userName)
        where TId : struct, IEquatable<TId>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        if (persistentObject != null)
        {
            if (persistentObject.IsTransient)
            {
                persistentObject.SetPersistentId(id);
            }

            if (persistentObject is IInsertAuditable<T> insertAuditable)
            {
                insertAuditable.CreatedAt = now;
                insertAuditable.CreatedBy = userName;
            }
        }
    }

    public static void SetLastModifiedProperties<TId, T>(
        this IPersistentObject<TId>? persistentObject,
        T now,
        string userName)
        where TId : struct, IEquatable<TId>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        if (persistentObject is IUpdateAuditable<T> updateAuditable)
        {
            updateAuditable.LastModifiedAt = now;
            updateAuditable.LastModifiedBy = userName;
        }
    }
}
