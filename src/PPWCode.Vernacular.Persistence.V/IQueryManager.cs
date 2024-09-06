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

namespace PPWCode.Vernacular.Persistence.V;

/// <summary>
///     An interface that represents a helper for the repositories.
///     Interfaces based on this will always represent helper methods that transform
///     an <see cref="T:System.Linq.IQueryable`1" /> by applying filters (or transformations).
/// </summary>
/// <typeparam name="TIdentity">The identity type</typeparam>
public interface IQueryManager<in TIdentity>
    where TIdentity : struct, IEquatable<TIdentity>
{
}