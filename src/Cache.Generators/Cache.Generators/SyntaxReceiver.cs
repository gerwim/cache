using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GerwimFeiken.Cache.Generators;

internal sealed class SyntaxReceiver : ISyntaxReceiver
{
    public List<InvocationExpressionSyntax> AddCacheCalls { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode context)
    {
        if (ShouldVisit(context, out var invocationSyntax))
            AddCacheCalls.Add(invocationSyntax!);
    }

    public static bool ShouldVisit(SyntaxNode context, out InvocationExpressionSyntax? invocation)
    {
        invocation = null;
        if (
            context
            is not InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax identifier } invocationSyntax
        )
            return false;
        if (identifier.Name.Identifier.ValueText != "AddCacheSourceGenerators")
            return false;

        invocation = invocationSyntax;
        return true;
    }
}