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

using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public interface IRepository<TModel, in TId>
    where TModel : IPersistentObject<TId>, IIdentity<TId>
    where TId : IEquatable<TId>
{
    Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IList<TModel>> FindAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(TModel model, CancellationToken cancellationToken = default);
    void Delete(TModel model);
}