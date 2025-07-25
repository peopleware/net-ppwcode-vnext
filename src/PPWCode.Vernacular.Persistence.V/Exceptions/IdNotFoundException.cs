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

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Vernacular.Persistence.V.Exceptions
{
    public class IdNotFoundException<T, TId> : NotFoundException
        where T : class, IIdentity<TId>
        where TId : IEquatable<TId>
    {
        public const string PersistentObjectTypeKey = "IdNotFoundException.PersistentObjectType";
        public const string PersistenceIdKey = "IdNotFoundException.PersistenceId";

        public IdNotFoundException(TId id, Exception? innerException = null)
            : base(null, innerException)
        {
            PersistentObjectType = typeof(T);
            Id = id;
        }

        public Type PersistentObjectType
        {
            get => (Type)Data[PersistentObjectTypeKey]!;
            private init => Data[PersistentObjectTypeKey] = value;
        }

        public TId Id
        {
            get => (TId)Data[PersistenceIdKey]!;
            private init => Data[PersistenceIdKey] = value;
        }

        public override bool Like(SemanticException? other)
            => base.Like(other)
               && other is IdNotFoundException<T, TId> otherIdNotFoundException
               && (PersistentObjectType == otherIdNotFoundException.PersistentObjectType)
               && EqualityComparer<TId>.Default.Equals(Id, otherIdNotFoundException.Id);

        public override string ToString()
            => $"Type: {GetType().Name}; PersistentObjectType={PersistentObjectType}; Id={Id}";
    }
}
