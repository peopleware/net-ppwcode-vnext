﻿// Copyright 2018 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Vernacular.Persistence.V.Exceptions
{
    [Serializable]
    public abstract class DbConstraintException : RepositorySqlException
    {
        private const string EntityIdKey = "DbConstraintException.EntityId";
        private const string EntityNameKey = "DbConstraintException.EntityName";
        private const string ConstraintNameKey = "DbConstraintException.ConstraintName";
        private const string ConstraintTypeKey = "DbConstraintException.ConstraintType";
        private const string ConstraintColumnNamesKey = "BaseColumnNameConstraintException.ExtraInfo";

        protected DbConstraintException(
            object entityId,
            string entityName,
            string sql,
            DbConstraintTypeEnum constraintType,
            string constraintName,
            string? extraInfo = null,
            string? message = null,
            Exception? innerException = null)
            : base(sql, message, innerException)
        {
            EntityId = entityId;
            EntityName = entityName;
            ConstraintType = constraintType;
            ConstraintName = constraintName;
            ExtraInfo = extraInfo;
        }

        public object EntityId
        {
            get => Data[EntityIdKey]!;
            private init => Data[EntityIdKey] = value;
        }

        public string EntityName
        {
            get => (string)Data[EntityNameKey]!;
            private init => Data[EntityNameKey] = value;
        }

        public DbConstraintTypeEnum ConstraintType
        {
            get => (DbConstraintTypeEnum)Data[ConstraintTypeKey]!;
            private init => Data[ConstraintTypeKey] = value;
        }

        public string ConstraintName
        {
            get => (string)Data[ConstraintNameKey]!;
            private init => Data[ConstraintNameKey] = value;
        }

        public string? ExtraInfo
        {
            get => (string?)Data[ConstraintColumnNamesKey];
            private init => Data[ConstraintColumnNamesKey] = value;
        }

        public override bool Like(SemanticException? other)
            => base.Like(other)
               && other is DbConstraintException e
               && (EntityId == e.EntityId)
               && (ConstraintType == e.ConstraintType)
               && string.Equals(EntityName, e.EntityName, StringComparison.InvariantCulture)
               && string.Equals(Sql, e.Sql, StringComparison.InvariantCulture)
               && string.Equals(ExtraInfo, e.ExtraInfo, StringComparison.InvariantCulture);

        public override string ToString()
        {
            StringBuilder sb = new ();

            sb.AppendFormat("EntityId: {0}", EntityId);
            sb.AppendLine();
            sb.AppendFormat("EntityName: {0}", EntityName);
            sb.AppendLine();
            sb.AppendFormat("Sql: {0}", Sql);
            sb.AppendLine();
            sb.AppendFormat("ConstraintType: {0}", ConstraintType);
            sb.AppendLine();
            sb.AppendFormat("ConstraintName: {0}", ConstraintName);
            sb.AppendLine();
            sb.AppendFormat("Extra Info: {0}", ExtraInfo);
            sb.AppendLine();
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}
