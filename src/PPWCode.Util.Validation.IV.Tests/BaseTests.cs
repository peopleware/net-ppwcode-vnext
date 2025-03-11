using System.Text.Json;
using System.Text.Json.Serialization;

using NUnit.Framework;

using PPWCode.Util.Validation.IV.European.Belgium;

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
            new (
                () =>
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
                    options.Converters.Add(new INSSConverter());
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
        {
            string json = JsonSerializer.Serialize(obj, JsonSerializerOptions);
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }
    }
}
