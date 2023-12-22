using System.Collections.Generic;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.Cloudflare.Models;

public class Result
{
    [JsonProperty("expiration")]
    public long? Expiration { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;
}

public class ResultInfo
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("cursor")]
    public string? Cursor { get; set; }
}

public class CloudflareListKeysResponse
{
    [JsonProperty("errors")]
    public List<string> Errors { get; set; } = new();

    [JsonProperty("messages")]
    public List<string> Messages { get; set; } = new();

    [JsonProperty("result")]
    public List<Result>? Result { get; set; }

    [JsonProperty("success")]
    public bool? Success { get; set; }

    [JsonProperty("result_info")]
    public ResultInfo? ResultInfo { get; set; }
}