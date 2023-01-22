using GerwimFeiken.Cache.Exceptions;
using Microsoft.Extensions.Configuration;

namespace GerwimFeiken.Cache.Utils;

public static class Configuration
{
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        return string.IsNullOrWhiteSpace(configuration[key]) 
                        ? throw new ConfigurationException($"Setting {key} is empty.")
                        : configuration[key];
    }
}