using System;
using System.Reflection;
using GerwimFeiken.Cache.ContractResolvers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GerwimFeiken.Cache.ContractResolvers;

/// <summary>
/// Extends <see cref="DefaultContractResolver"/> with support for private setters and private constructors.
/// </summary>
public class PrivateSetterAndCtorContractResolver : DefaultContractResolver
{
    protected override JsonObjectContract CreateObjectContract(Type objectType)
        => base.CreateObjectContract(objectType).SupportPrivateCTors(objectType, CreateConstructorParameters);

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        => base.CreateProperty(member, memberSerialization).MakeWriteable(member);
}
