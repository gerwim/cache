using GerwimFeiken.Cache.ContractResolvers;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.SerializerSettings;

public class DefaultSerializerSettings : JsonSerializerSettings
{
    public DefaultSerializerSettings()
    {
        ContractResolver = new PrivateSetterAndCtorContractResolver();
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    }
}