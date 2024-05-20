using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace FixedWidthLibrary;

[Generator]
public class FixedWidthGenerator : IIncrementalGenerator
{
    private const string Namespace = "FixedWidthLibraryCore";
    private const string FixedWidthMarkerAttributeName = "FixedWidthMarkerAttribute";
    private const string FixWidthPropertyAttributeName = "FixedWidthAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => Predicate(s),
                (ctx, _) => GetClassDeclarationForSourceGen(ctx));

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            ((ctx, t) => GenerateCode(ctx, t.Left, t.Right)));
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

                    var dictionaryValue =
                        $$"""
                          { "{{propertyName}}", new {{Namespace}}.FixedWidthAttribute({{start}}, {{length}}){{{string.Join(",", fixedWidthProperties)}}} }
                          """;

                    return (dictionaryValue, start, length);
                })
                .OrderBy(x => x.start)
                .ToImmutableArray();

            var assignments = properties
                .Select(x =>
                {
                    var t = x.Type.Name;

                    var type = x.Type.Name switch
                    {
                        "String" => "String",
                        "Boolean" => "Bool",
                        "Int32" => "Int",
                        "Int64" => "Long",
                        "Decimal" => "Decimal",
                        "Double" => "Double",
                        "Single" => "Float",
                        "Char" => "Char",
                        "DateTime" => "DateTime",
                        "DateOnly" => "DateOnly",
                        _ => "No primitive type matched"
                    };

                    return x.NullableAnnotation == NullableAnnotation.NotAnnotated
                        ? $"""{x.Name} = FixedWidthAttributes["{x.Name}"].Parse{type}(line);"""
                        : $"""{x.Name} = FixedWidthAttributes["{x.Name}"].ParseNullable{type}(line);""";
                });

            var stringBuilderWrites = properties
                .Select(x => $"""FixedWidthAttributes["{x.Name}"].WriteToStringBuilder({x.Name}, stringBuilder);""")
                .ToImmutableArray();

            var streamWrites = properties
                .Select(x => $"""FixedWidthAttributes["{x.Name}"].WriteToStream({x.Name}, streamWriter);""")
                .ToImmutableArray();

            var asyncStreamWrites = properties
                .Select(x => $"""await FixedWidthAttributes["{x.Name}"].WriteToStreamAsync({x.Name}, streamWriter);""")
                .ToImmutableArray();

            var code =
                $$"""
                  // <auto-generated/>

                  using System;
                  using System.Collections.Generic;
                  using System.Text;
                  using System.IO;

                  namespace {{namespaceName}};

                  partial class {{className}}
                  {
                        public static Dictionary<string, {{Namespace}}.FixedWidthAttribute> FixedWidthAttributes = new Dictionary<string, {{Namespace}}.FixedWidthAttribute>
                      {
                            {{string.Join(",\n", dictionaryBody.Select(x => x.dictionaryValue))}}
                      };
                      
                      public const int TotalFixedWidthLength = {{dictionaryBody.Sum(x => x.length)}};
                  
                        public {{className}}(ReadOnlySpan<char> line) {
                            {{string.Join("\n", assignments)}}
                        }
                        
                        public {{className}}() {
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