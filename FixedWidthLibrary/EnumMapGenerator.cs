using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;


namespace FixedWidthLibrary;

[Generator]
public class EnumGenerator : IIncrementalGenerator
{
    private const string Namespace = "Generators";
    private const string EnumMapGeneratorMarkerAttributeName = "EnumMapGeneratorMarkerAttribute";
    private const string EnumMapGeneratorAttributeName = "EnumMapGeneratorAttribute";
    private const string EnumNotMappedGeneratorAttributeName = "EnumNotMappedAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => Predicate(s),
                (ctx, _) => GetEnumDeclarationForSourceGen(ctx));

        // Generate the source code.
        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }

    private static bool Predicate(SyntaxNode s)
    {
        if (s is not EnumDeclarationSyntax e) return false;
        if (!e.AttributeLists.Any()) return false;
        if (!e.AttributeLists
                .Any(a => a.Attributes.Any(attr => attr.Name.ToString() == "EnumMapMarker"))) return false;

        return s is EnumDeclarationSyntax;
    }

    private class EnumMapInfo(string enumMemberName, string? value, int version, bool isDefault = false)
    {
        public string EnumMemberName { get; } = enumMemberName;
        public string? Value { get; } = value;
        public int Version { get; } = version;
        public bool IsDefault { get; } = isDefault;
    }

    private static EnumDeclarationSyntax GetEnumDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        return (EnumDeclarationSyntax)context.Node;
    }

    private void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<EnumDeclarationSyntax> enumDeclarations)
    {
        foreach (var enumDeclarationSyntax in enumDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);

            if (ModelExtensions.GetDeclaredSymbol(semanticModel, enumDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                continue;

            var namespaceName = namedTypeSymbol.ContainingNamespace.ToDisplayString();

            var enumName = enumDeclarationSyntax.Identifier.Text;

            EnumMapInfo? defaultEnumMap = null;
            List<EnumMapInfo> enumMapInfos = [];

            foreach (var em in enumDeclarationSyntax
                         .Members)
            {
                var enumMemberName = em.Identifier.Text;

                foreach (var a in em
                             .AttributeLists
                             .Select(x => x.Attributes
                                 .First()))
                {
                    switch (a.Name.ToString())
                    {
                        case "EnumNotMapped":
                            defaultEnumMap = new EnumMapInfo(enumMemberName, "", 0, true);
                            break;
                        case "EnumMap":
                            var attributeArguments = a.DescendantNodes().OfType<AttributeArgumentSyntax>().ToImmutableArray();

                            var value = attributeArguments[0].Expression.ToString();

                            if (attributeArguments.Length == 1)
                            {
                                enumMapInfos.Add(new EnumMapInfo(enumMemberName, value, 0));
                            }
                            else
                            {
                                var version = int.Parse(attributeArguments[1].Expression.ToString());
                                enumMapInfos.Add(new EnumMapInfo(enumMemberName, value, version));
                            }

                            break;
                    }
                }
            }

            var enumMapGroups = enumMapInfos
                .GroupBy(x => x.Version)
                .Select(x =>
                {
                    var stringToEnums = defaultEnumMap is null
                        ? x.Select(enumMapInfo => $"{enumName}.{enumMapInfo.EnumMemberName} => {enumMapInfo.Value}")
                        : x.Select(enumMapInfo => $"{enumName}.{enumMapInfo.EnumMemberName} => {enumMapInfo.Value}")
                            .Append($"_ => string.Empty");

                    var stringToEnum = $$"""
                                         if (version == {{x.Key}})
                                             return value switch
                                             {
                                                 {{string.Join(",\n", stringToEnums)}}
                                             }
                                         """;

                    var enumToStrings = defaultEnumMap is null
                        ? x.Select(enumMapInfo => $"{enumMapInfo.Value} => {enumName}.{enumMapInfo.EnumMemberName}")
                        : x.Select(enumMapInfo => $"{enumMapInfo.Value} => {enumName}.{enumMapInfo.EnumMemberName}")
                            .Append($"_ => {enumName}.{defaultEnumMap.EnumMemberName}");

                    var enumToString = $$"""
                                         if (version == {{x.Key}})
                                             return value switch
                                             {
                                                 {{string.Join(",\n", enumToStrings)}}
                                             };
                                         """;

                    return new { StringToEnum = stringToEnum, EnumToString = enumToString };
                })
                .ToImmutableArray();

            var code =
                $$"""
                  // <auto-generated/>

                  using System;
                  using System.Collections.Generic;

                  namespace {{namespaceName}};

                  public static partial class {{enumName}}Extensions
                  {
                        public MapToString(this {{enumName}} value, int version = 0)
                        {
                            {{string.Join("\n", enumMapGroups.Select(x => x.EnumToString))}}
                            
                            throw new ArgumentOutOfRangeException(nameof(version));
                        }
                        
                        public MapToEnum(this string value, int version = 0)
                        {
                           {{string.Join("\n", enumMapGroups.Select(x => x.StringToEnum))}}
                           
                           throw new ArgumentOutOfRangeException(nameof(version));
                        }
                  }
                  """;
            
            var node = SyntaxFactory.ParseSyntaxTree(code).GetRoot();
            node = node.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
            var formattedNode = Formatter.Format(node, new AdhocWorkspace());
            var formattedCode = formattedNode.ToFullString();

            context.AddSource($"{enumName}.Maps.g.cs", SourceText.From(formattedCode, Encoding.UTF8));
        }
    }
}