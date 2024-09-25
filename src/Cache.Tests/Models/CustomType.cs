using Newtonsoft.Json;

namespace Cache.Tests.Models;

public record CustomType
{
    [JsonProperty("$type")]
    public string? Type { get; set; }
}