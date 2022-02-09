using System.Text.Json.Serialization;
using Guide.Shared.Models;

namespace Guide.Client6.Json;

// See https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-source-generator/
[JsonSerializable(typeof(EquivalencyData))]
[JsonSerializable(typeof(DaleelData))]
internal partial class GuideJsonSerializerContext: JsonSerializerContext
{
    
}