using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace FixedWidthLibrary;

public class FixedWidthAbstract(string typeName, string nullableTypeName)
{
    public string TypeName { get; } = typeName;
    public string NullableTypeName { get; } = nullableTypeName;
};

[Generator]
public class FixedWidthGenerator : IIncrementalGenerator
{
    private const string Namespace = "FixedWidthLibraryCore.FixedWidth";
    private const string FixedWidthMarkerAttributeName = "FixedWidthMarkerAttribute";
    private const string FixWidthPropertyAttributeName = "FixedWidthAttribute";

    private static FixedWidthAbstract GetFixedWidthAbstract(ISymbol type)
    {
        return type.Name switch
        {
            "String" => new FixedWidthAbstract("FixedWidthString", "FixedWidthStringNullable"),
            "Boolean" => new FixedWidthAbstract("FixedWidthBool", "FixedWidthBoolNullable"),
            "Int32" => new FixedWidthAbstract("FixedWidthInt", "FixedWidthIntNullable"),
            "Int64" => new FixedWidthAbstract("FixedWidthLong", "FixedWidthLongNullable"),
            "Decimal" => new FixedWidthAbstract("FixedWidthDecimal", "FixedWidthDecimalNullable"),
            "Double" => new FixedWidthAbstract("FixedWidthDouble", "FixedWidthDoubleNullable"),
            "Single" => new FixedWidthAbstract("FixedWidthFloat", "FixedWidthFloatNullable"),
            "Char" => new FixedWidthAbstract("FixedWidthChar", "FixedWidthCharNullable"),
            "DateTime" => new FixedWidthAbstract("FixedWidthDateTime", "FixedWidthDateTimeNullable"),
            "DateOnly" => new FixedWidthAbstract("FixedWidthDateOnly", "FixedWidthDateOnlyNullable"),
        };
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            var provider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (s, _) => Predicate(s),
                    (ctx, _) => GetClassDeclarationForSourceGen(ctx));

            context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
                (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
        }
        catch (Exception e)
        {
            var message =
                $"""
                 /*

                 {e}

                 */
                 """;

            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "FixedWidthGeneratorError.g.cs",
                SourceText.From(message, Encoding.UTF8)));
        }
    }

    private static bool Predicate(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax c &&
               c.AttributeLists.Any() &&
               c.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == "FixedWidthMarker"));
    }

    private static ClassDeclarationSyntax GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        return (ClassDeclarationSyntax)context.Node;
    }

    private void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        foreach (var classDeclarationSyntax in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclarationSyntax) is not INamedTypeSymbol
                namedTypeSymbol)
                continue;

            var namespaceName = namedTypeSymbol.ContainingNamespace.ToDisplayString();

            var classAccessibility = compilation
                    .GetSemanticModel(classDeclarationSyntax.SyntaxTree)
                    .GetDeclaredSymbol(classDeclarationSyntax)
                    ?.DeclaredAccessibility switch
                {
                    Accessibility.Public => "public",
                    Accessibility.Private => "private",
                    Accessibility.Internal => "internal",
                    _ or null => ""
                };

            var className = classDeclarationSyntax.Identifier.Text;

            var properties = namedTypeSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(GetFixedWidthProperties)
                .ToImmutableArray();

            var dictionaryBody = properties
                .Select(x =>
                {
                    var propertyName = x.Name;

                    var fixedWidthPropertyAttribute = x.GetAttributes()
                        .First(ad => ad.AttributeClass?.Name == FixWidthPropertyAttributeName);
                    var start = (int)(fixedWidthPropertyAttribute.ConstructorArguments[0].Value ?? 0);
                    var length = (int)(fixedWidthPropertyAttribute.ConstructorArguments[1].Value ?? 0);

                    var fixedWidthProperties = fixedWidthPropertyAttribute
                        .NamedArguments
                        .Select(kv => $"{kv.Key} = {kv.Value.ToCSharpString()}")
                        .ToImmutableArray();

                    var typeName = x.Type.NullableAnnotation == NullableAnnotation.Annotated
                        ? GetFixedWidthAbstract(x.Type).NullableTypeName
                        : GetFixedWidthAbstract(x.Type).TypeName;

                    var mapType =
                        fixedWidthPropertyAttribute
                            .NamedArguments
                            .FirstOrDefault(kv => kv.Key == "MapType")
                            .Value
                            .Value
                            ?.ToString() ?? $"{Namespace}.{typeName}";

                    var dictionaryValue =
                        $$"""
                          public static readonly {{mapType}} {{propertyName}} = new {{mapType}}({{start}}, {{length}}){{{string.Join(",", fixedWidthProperties)}}};
                          """;

                    return (dictionaryValue, start, length, mapType);
                })
                .OrderBy(x => x.start)
                .ToImmutableArray();

            var assignments = properties
                .Select(x => $"{x.Name} = FixedWidthMetaData.{x.Name}.Parse(line);")
                .ToImmutableArray();

            var stringBuilderWrites = properties
                .Select(x => $"FixedWidthMetaData.{x.Name}.WriteToStringBuilder({x.Name}, stringBuilder);")
                .ToImmutableArray();

            var streamWrites = properties
                .Select(x => $"FixedWidthMetaData.{x.Name}.WriteToStream({x.Name}, streamWriter);")
                .ToImmutableArray();

            var asyncStreamWrites = properties
                .Select(x => $"await FixedWidthMetaData.{x.Name}.WriteToStreamAsync({x.Name}, streamWriter);")
                .ToImmutableArray();

            var code =
                $$"""
                  // <auto-generated/>

                  using System;
                  using System.Collections.Generic;
                  using System.Text;
                  using System.IO;
                  using FixedWidthLibraryCore.FixedWidth;

                  namespace {{namespaceName}};

                  {{classAccessibility}} partial class {{className}}
                  {
                        public static class FixedWidthMetaData
                        {
                            {{string.Join("\n", dictionaryBody.Select(x => x.dictionaryValue))}}
                        }
                  
                      public const int TotalFixedWidthLength = {{dictionaryBody.Sum(x => x.length)}};
                  
                        public {{className}}(ReadOnlySpan<char> line) {
                            {{string.Join("\n", assignments)}}
                        }
                        
                        public StringBuilder WriteToStringBuilder(StringBuilder? stringBuilder = null)
                        {
                            stringBuilder ??= new StringBuilder();
                            
                            {{string.Join("\n", stringBuilderWrites)}}
                            
                            stringBuilder.AppendLine();
                            
                            return stringBuilder;
                        }
                        
                        public Stream WriteToStream(Stream stream)
                        {
                            using var streamWriter = new StreamWriter(stream, leaveOpen: true);
                            
                            {{string.Join("\n", streamWrites)}}
                            
                            return stream;
                        }
                        
                        public async Task<Stream> WriteToStreamAsync(Stream stream)
                        {
                            using var streamWriter = new StreamWriter(stream, leaveOpen: true);
                        
                            {{string.Join("\n", asyncStreamWrites)}}
                            
                            return stream;
                        }
                  }
                  """;

            var formattedCode = Helper.FormatCode(code);

            context.AddSource($"{className}.FixedWidth.g.cs", SourceText.From(formattedCode, Encoding.UTF8));
        }
    }

    private static bool GetFixedWidthProperties(IPropertySymbol x)
    {
        var attributes = x.GetAttributes();
        return attributes.Any(ad => ad.AttributeClass?.Name == "FixedWidthAttribute");
    }
}