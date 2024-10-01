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

using PPWCode.Util.Authorisation.I;
using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.Persistence.V.Tests;

public class PersonRepository
    : InMemoryRepository<Person, Person, long>,
      IPersonRepository
{
    public PersonRepository(
        ITimeProvider<DateTimeOffset> timeProvider,
        IPrincipalProvider principalProvider,
        IPersonQueryManager personQueryManager)
        : base(personQueryManager)
    {
        TimeProvider = timeProvider;
        PrincipalProvider = principalProvider;
        PersonQueryManager = personQueryManager;
    }

    public ITimeProvider<DateTimeOffset> TimeProvider { get; }
    public IPrincipalProvider PrincipalProvider { get; }
    public IPersonQueryManager PersonQueryManager { get; }

    /// <inheritdoc />
    protected override long GetNextIdFor(Person model)
        => IdGenerator.NextId;

    /// <inheritdoc />
    protected override void SetIdAndCreateAuditProperties(Person model, long id)
        => model.SetIdAndCreateAuditProperties(id, TimeProvider.Now, PrincipalProvider.CurrentPrincipal.Identity?.Name ?? "Unknown");

    /// <inheritdoc />
    protected override void SetLastModifiedProperties(Person model)
        => model.SetLastModifiedProperties(TimeProvider.Now, PrincipalProvider.CurrentPrincipal.Identity?.Name ?? "Unknown");

    /// <inheritdoc />
    public Task<List<Person>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        => FindAsync(PersonQueryManager.FindByName(name), cancellationToken);
}
