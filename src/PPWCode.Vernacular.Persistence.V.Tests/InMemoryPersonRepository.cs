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

public class InMemoryPersonRepository
    : InMemoryRepository<Person, Person, long>,
      IPersonRepository
{
    private readonly ITimeProvider<DateTimeOffset> _timeProvider;
    private readonly IPrincipalProvider _principalProvider;
    private readonly IPersonQueryManager _personQueryManager;

    public InMemoryPersonRepository(
        ITimeProvider<DateTimeOffset> timeProvider,
        IPrincipalProvider principalProvider,
        IPersonQueryManager personQueryManager)
    {
        _timeProvider = timeProvider;
        _principalProvider = principalProvider;
        _personQueryManager = personQueryManager;
    }

    /// <inheritdoc />
    protected override long GetNextIdFor(Person model)
        => IdGenerator.NextId;

    /// <inheritdoc />
    protected override void SetIdAndCreateAuditProperties(Person model, long id)
        => model.SetIdAndCreateAuditProperties(id, _timeProvider.Now, _principalProvider.CurrentPrincipal.Identity?.Name ?? "Unknown");

    /// <inheritdoc />
    protected override void SetLastModifiedProperties(Person model)
        => model.SetLastModifiedProperties(_timeProvider.Now, _principalProvider.CurrentPrincipal.Identity?.Name ?? "Unknown");

    /// <inheritdoc />
    public Task<List<Person>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        => FindAsync(_personQueryManager.FindByName(name), cancellationToken);
}
