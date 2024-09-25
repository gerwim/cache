using System;
using System.Reflection;
using GerwimFeiken.Cache.ContractResolvers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GerwimFeiken.Cache.ContractResolvers;

/// <summary>
/// Extends <see cref="DefaultContractResolver"/> with support for private setters and private constructors.
/// </summary>
public class CustomContractResolver : DefaultContractResolver
{
    protected override JsonObjectContract CreateObjectContract(Type objectType)
        => base.CreateObjectContract(objectType).SupportPrivateCTors(objectType, CreateConstructorParameters);
    
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        // Reset JsonProperty settings that may have been set by attributes
        property.PropertyName = member.Name;
        property.Ignored = false;
        property.Converter = null;
        property.DefaultValueHandling = null;
        property.NullValueHandling = null;
        property.ObjectCreationHandling = null;
        property.ReferenceLoopHandling = null;
        property.Required = Required.Default;
        property.ShouldSerialize = null;
        property.TypeNameHandling = null;

        return property.MakeWriteable(member);
    }
}
