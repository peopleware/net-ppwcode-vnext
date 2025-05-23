﻿// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace PPWCode.Vernacular.Persistence.V;

public abstract class InsertAuditablePersistentObject<TId, TTimestamp>
    : PersistentObject<TId>,
      IInsertAuditable<TTimestamp>
    where TId : IEquatable<TId>
    where TTimestamp : struct, IComparable<TTimestamp>, IEquatable<TTimestamp>
{
    public virtual TTimestamp? CreatedAt { get; set; }

    public virtual string? CreatedBy { get; set; }
}
