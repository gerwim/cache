using GerwimFeiken.Cache.ContractResolvers;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.SerializerSettings;

public class DefaultSerializerSettings : JsonSerializerSettings
{
    public DefaultSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.Objects;
        ContractResolver = new PrivateSetterAndCtorContractResolver();
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    }
}