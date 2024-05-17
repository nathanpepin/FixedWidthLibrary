using System;
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
public class FixedWidthGenerator : IIncrementalGenerator
{
    private const string Namespace = "FixedWidthLibraryCore";
    private const string FixedWidthFileAttributeName = "FixedWidthMarkerAttribute";
    private const string FixWidthPropertyAttributeName = "FixedWidthAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => s is ClassDeclarationSyntax,
                (ctx, _) => GetClassDeclarationForSourceGen(ctx))
            .Where(t => t.fixWidthFileAttribute)
            .Select((t, _) => t.Item1);

        // Generate the source code.
        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            ((ctx, t) => GenerateCode(ctx, t.Left, t.Right)));
    }

    private static (ClassDeclarationSyntax, bool fixWidthFileAttribute) GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeSyntax in classDeclarationSyntax
                     .AttributeLists
                     .SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
        {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                continue;

            var attributeName = attributeSymbol.ContainingType.ToDisplayString();

            if (attributeName == $"{Namespace}.{FixedWidthFileAttributeName}")
                return (classDeclarationSyntax, true);
        }

        return (classDeclarationSyntax, false);
    }

    private void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        foreach (var classDeclarationSyntax in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                continue;

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            var className = classDeclarationSyntax.Identifier.Text;

            var properties = classSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.GetAttributes().Any(ad => ad.AttributeClass?.Name == FixWidthPropertyAttributeName))
                .ToImmutableArray();

            var dictionaryBody = properties
                .Select(x =>
                {
                    var propertyName = x.Name;

                    var fixedWidthPropertyAttribute = x.GetAttributes().First(ad => ad.AttributeClass?.Name == FixWidthPropertyAttributeName);
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

                    return (dictionaryValue, length);
                })
                .ToImmutableArray();

            var assignments = properties
                .Select(x => x.NullableAnnotation == NullableAnnotation.NotAnnotated
                    ? $"""{x.Name} = FixedWidthAttributes["{x.Name}"].Assign({x.Name}, line);"""
                    : $"""{x.Name} = FixedWidthAttributes["{x.Name}"].AssignNullable({x.Name}, line);""");

            var stringBuidlerWrites = properties
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
                            
                            {{string.Join("\n", stringBuidlerWrites)}}
                            
                            stringBuilder.AppendLine();
                            
                            return stringBuilder;
                        }
                        
                        public StreamWriter WriteToStream<T>(StreamWriter streamWriter)
                        {
                            {{string.Join("\n", streamWrites)}}
                            
                            return streamWriter;
                        }
                        
                        public async Task<StreamWriter> WriteToStreamAsync<T>(T value, StreamWriter streamWriter)
                        {
                            {{string.Join("\n", asyncStreamWrites)}}
                            
                            return streamWriter;
                        }
                  }

                  """;

            var node = SyntaxFactory.ParseSyntaxTree(code).GetRoot();
            node = node.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
            var formattedNode = Formatter.Format(node, new AdhocWorkspace());
            var formattedCode = formattedNode.ToFullString();
            
            context.AddSource($"{className}.FixedWidth.g.cs", SourceText.From(formattedCode, Encoding.UTF8));
        }
    }
}