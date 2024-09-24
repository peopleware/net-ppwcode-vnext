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

using System.Text.Json;
using System.Text.Json.Serialization;

using NUnit.Framework;

namespace PPWCode.Util.Validation.IV.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public abstract class BaseTests
    {
        [SetUp]
        public void Setup()
        {
            OnSetup();
        }

        [TearDown]
        public void TearDown()
        {
            OnTearDown();
        }

        private static readonly Lazy<JsonSerializerOptions> _jsonSerializerOptions =
            new (() =>
                 {
                     JsonSerializerOptions options =
                         new ()
                         {
                             IncludeFields = false,
                             NumberHandling = JsonNumberHandling.Strict,
                             ReferenceHandler = ReferenceHandler.IgnoreCycles,
                             AllowTrailingCommas = false,
                             DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                             ReadCommentHandling = JsonCommentHandling.Disallow,
                             UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                             PropertyNameCaseInsensitive = false,
                             DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                         };
                     options.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
                     return options;
                 });

        public static JsonSerializerOptions JsonSerializerOptions
            => _jsonSerializerOptions.Value;

        protected virtual void OnSetup()
        {
        }

        protected virtual void OnTearDown()
        {
        }

        protected virtual T? DeepClone<T>(T obj)
            where T : class
            => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj, JsonSerializerOptions), JsonSerializerOptions);
    }
}
