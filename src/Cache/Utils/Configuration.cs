using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Options;

namespace GerwimFeiken.Cache.Utils;

public static class Configuration
{
    [return: NotNull]
    public static TValue GetRequiredValue<TClass, TValue>(this TClass options, Expression<Func<TClass, TValue>> expression) 
        where TClass : IOptions
    {
        var propertyName = GetNameFromMemberExpression(expression.Body);
        var type = typeof(TClass);
        var properties = type.IsInterface
            ? new[] { type }
            .Concat(type.GetInterfaces())
            .SelectMany(i => i.GetProperties())
            : type.GetProperties();
        
        var value = properties.FirstOrDefault(x => x.Name == propertyName)?.GetValue(options, null);

        return value switch
        {
            null => throw new ConfigurationException($"Setting {propertyName} is empty."),
            TValue cast => cast,
            _ => throw new ConfigurationException($"Setting {propertyName} is not of type {typeof(TClass)}.")
        };
    }
    
    static string GetNameFromMemberExpression(Expression expression)
    {
        return expression switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            UnaryExpression unaryExpression => GetNameFromMemberExpression(unaryExpression.Operand),
            _ => throw new ConfigurationException("Invalid property type, this exception should never happen. Please create a bug report at https://github.com/gerwim/cache/issues")
        };
    }
}