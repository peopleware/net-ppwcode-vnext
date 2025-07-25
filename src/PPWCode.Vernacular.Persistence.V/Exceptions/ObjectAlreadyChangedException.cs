// Copyright 2018 by PeopleWare n.v..
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
    public class ObjectAlreadyChangedException : SemanticException
    {
        public ObjectAlreadyChangedException(
            string entityName,
            object identifier,
            string? message = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            EntityName = entityName;
            Identifier = identifier;
        }

        public object EntityName
        {
            get => Data["EntityName"]!;
            private init => Data["EntityName"] = value;
        }

        public object Identifier
        {
            get => Data["Identifier"]!;
            private init => Data["Identifier"] = value;
        }

        public override bool Like(SemanticException? other)
            => base.Like(other)
               && other is ObjectAlreadyChangedException e
               && (EntityName == e.EntityName)
               && (Identifier == e.Identifier);

        public override string ToString()
            => $"Type: {GetType().Name}; Sender={Identifier}";
    }
}
