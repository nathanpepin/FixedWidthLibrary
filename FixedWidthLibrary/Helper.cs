using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FixedWidthLibrary;

public static class Helper
{
    public static string FormatCode(string code) =>
        CSharpSyntaxTree.ParseText(code).GetRoot().NormalizeWhitespace().ToFullString();

    public static bool HasAttributeWithName(ISymbol it, string attributeName)
    {
        var attributes = it.GetAttributes();
        return attributes.Any(ad => ad.AttributeClass?.Name == attributeName);
    }

    public static bool HasFixedWidthAttribute(ISymbol it)
    {
        var attributes = it.GetAttributes();
        return attributes.Any(ad => IsFixedWidthAttribute(ad.AttributeClass));
    }

    public static bool IsFixedWidthAttribute(INamedTypeSymbol? attributeClass)
    {
        if (attributeClass == null)
            return false;

        // Check if this is the FixedWidthAttribute directly
        if (attributeClass.Name == "FixedWidthAttribute")
            return true;

        // Check if it inherits from FixedWidthElement<T>
        var baseType = attributeClass.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == "FixedWidthElement" || 
                baseType.OriginalDefinition?.Name == "FixedWidthElement")
                return true;
            baseType = baseType.BaseType;
        }

        return false;
    }

    public static ImmutableArray<IPropertySymbol> GetPropertiesWithAttribute(ITypeSymbol namedTypeSymbol, string attributeName)
    {
        return
        [
            ..namedTypeSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => HasAttributeWithName(x, attributeName))
        ];
    }

    public static ImmutableArray<IPropertySymbol> GetPropertiesWithFixedWidthAttribute(ITypeSymbol namedTypeSymbol)
    {
        return
        [
            ..namedTypeSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(HasFixedWidthAttribute)
        ];
    }

    public static string GetClassAccessibility(Compilation compilation, BaseTypeDeclarationSyntax classDeclarationSyntax)
    {
        return compilation
                .GetSemanticModel(classDeclarationSyntax.SyntaxTree)
                .GetDeclaredSymbol(classDeclarationSyntax)
                ?.DeclaredAccessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Private => "private",
                Accessibility.Internal => "internal",
                _ or null => ""
            };
    }

    public static bool ClassHasAttribute(SyntaxNode syntaxNode, string attributeName)
    {
        return syntaxNode is ClassDeclarationSyntax c &&
               c.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == attributeName));
    }

    public static AttributeData GetAttributeWithName(ISymbol it, string attributeName)
    {
        return it.GetAttributes()
            .First(ad => ad.AttributeClass?.Name == attributeName);
    }

    public static AttributeData GetFixedWidthAttribute(ISymbol it)
    {
        return it.GetAttributes()
            .First(ad => IsFixedWidthAttribute(ad.AttributeClass));
    }
}