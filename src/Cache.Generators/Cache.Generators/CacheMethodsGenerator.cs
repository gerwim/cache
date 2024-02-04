using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GerwimFeiken.Cache.Generators;

[Generator]
public class CacheMethodsGenerator : IIncrementalGenerator
{
    private const string AttributeSourceCode = $@"// <auto-generated/>

namespace {Constants.Namespace}
{{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    internal sealed class {Constants.AttributeName} : System.Attribute
    {{
    }}
}}";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CacheKeyAttribute.g.cs",
            SourceText.From(AttributeSourceCode, Encoding.UTF8)));

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider((s, _) => s is ClassDeclarationSyntax,
                (ctx, _) => GetClassDeclarationForSourceGen(ctx))
            .Where(t => t.Properties.Any());

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }
    
    private static ClassDeclaration GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var properties = classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>();
        var result = new List<PropertyDeclarationSyntax>();

        foreach (var property in properties)
        {
            foreach (AttributeListSyntax attributeListSyntax in property.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax)
                            .Symbol is not IMethodSymbol attributeSymbol)
                        continue;

                    string attributeName = attributeSymbol.ContainingType.ToDisplayString();
                    
                    if (attributeName == $"{Constants.Namespace}.{Constants.AttributeName}")
                        result.Add(property);
                }
            }
        }

        return new ClassDeclaration(classDeclarationSyntax, result);
    }
    private void GenerateCode(SourceProductionContext context, Compilation compilation, ImmutableArray<ClassDeclaration> classDeclarations)
    {
        // Go through all filtered class declarations.
        foreach (var classDeclaration in classDeclarations)
        {
            // We need to get semantic model of the class to retrieve metadata.
            var semanticModel = compilation.GetSemanticModel(classDeclaration.ClassDeclarationSyntax.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(classDeclaration.ClassDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                continue;

            var props = new Dictionary<string, string>();
            foreach (var propertyDeclarationSyntax in classDeclaration.Properties)
            {
                var name = propertyDeclarationSyntax.Identifier.ToString();
                var typeSymbol = semanticModel.GetSymbolInfo(propertyDeclarationSyntax.Type).Symbol as INamedTypeSymbol;
                if (typeSymbol is null) continue;

                var type = typeSymbol.ToDisplayString(
                    new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle
                        .NameAndContainingTypesAndNamespaces));

                props.Add(type, name);
            }

            var className = $"{classDeclaration.ClassDeclarationSyntax.Identifier.Text}Cache";
            var interfaceName = $"I{className}";
            var propString = props.Aggregate(string.Empty, (acc, next) => $"{acc}, global::{next.Key} {next.Value}", res => res.Remove(0, 2));
            var cacheKeyFromProperties = $"{classSymbol}-{{{string.Join("}-{", props.Select(x => x.Value))}}}";
            var cacheKeyFromValue = $"{classSymbol}-{{{string.Join("}-{", props.Select(x => $"value.{x.Value}"))}}}";
            
            // TODO: write methods should not use the properties, but rather the object itself. It should get the properties from the object because we know the CacheKey attributes already
            
            // TODO: add ICache<User> solution
            // TODO: namespace should be the correct namespace (not GerwimFeiken.Cache)?
            
            var code = $@"// <auto-generated/>
#nullable enable
namespace GerwimFeiken.Cache;

public interface {interfaceName} {{
    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, int? expireInSeconds = null);
    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, System.TimeSpan expireIn);
    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, bool errorIfExists, int? expireInSeconds = null);
    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, bool errorIfExists, System.TimeSpan expireIn);
    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::{classSymbol}> func, int? expireInSeconds = null);
    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::System.Threading.Tasks.Task<global::{classSymbol}>> func, int? expireInSeconds = null);
    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::{classSymbol}> func, System.TimeSpan expireIn);
    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::System.Threading.Tasks.Task<global::{classSymbol}>> func, System.TimeSpan expireIn);
    public global::System.Threading.Tasks.Task<global::{classSymbol}?> Read({propString});
    public global::System.Threading.Tasks.Task Delete({propString});
}}

public class {className}(global::GerwimFeiken.Cache.ICache cache) : {interfaceName}
{{
    private readonly global::GerwimFeiken.Cache.ICache _cache = cache;

    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, int? expireInSeconds = null)
    {{
        return _cache.Write($""{cacheKeyFromValue}"", value, expireInSeconds);
    }}

    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, System.TimeSpan expireIn)
    {{
        return _cache.Write($""{cacheKeyFromValue}"", value, expireIn);
    }}

    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, bool errorIfExists, int? expireInSeconds = null)
    {{
        return _cache.Write($""{cacheKeyFromValue}"", value, errorIfExists, expireInSeconds);
    }}

    public global::System.Threading.Tasks.Task Write(global::{classSymbol} value, bool errorIfExists, System.TimeSpan expireIn)
    {{
        return _cache.Write($""{cacheKeyFromValue}"", value, errorIfExists, expireIn);
    }}

    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::{classSymbol}> func, int? expireInSeconds = null)
    {{
        return _cache.ReadOrWrite<global::{classSymbol}>($""{cacheKeyFromProperties}"", func, expireInSeconds);
    }}

    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::System.Threading.Tasks.Task<global::{classSymbol}>> func, int? expireInSeconds = null)
    {{
        return _cache.ReadOrWrite<global::{classSymbol}>($""{cacheKeyFromProperties}"", func, expireInSeconds);
    }}

    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::{classSymbol}> func, System.TimeSpan expireIn)
    {{
        return _cache.ReadOrWrite<global::{classSymbol}>($""{cacheKeyFromProperties}"", func, expireIn);
    }}

    public global::System.Threading.Tasks.Task<global::{classSymbol}?> ReadOrWrite({propString}, System.Func<global::System.Threading.Tasks.Task<global::{classSymbol}>> func, System.TimeSpan expireIn)
    {{
        return _cache.ReadOrWrite<global::{classSymbol}>($""{cacheKeyFromProperties}"", func, expireIn);
    }}

    public global::System.Threading.Tasks.Task<global::{classSymbol}?> Read({propString})
    {{
        return _cache.Read<global::{classSymbol}>($""{cacheKeyFromProperties}"");
    }}

    public global::System.Threading.Tasks.Task Delete({propString})
    {{
        return _cache.Delete($""{cacheKeyFromProperties}"");
    }}
}}
";

            context.AddSource($"{className}.g.cs", SourceText.From(code, Encoding.UTF8));
        }
    }

    private sealed record ClassDeclaration(ClassDeclarationSyntax ClassDeclarationSyntax, List<PropertyDeclarationSyntax> Properties)
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = ClassDeclarationSyntax;
        public List<PropertyDeclarationSyntax> Properties { get; } = Properties;
    }
}