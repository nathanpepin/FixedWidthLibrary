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
    public string TypeName { get; } = $"{FixedWidthGenerator.CoreNamespace}.{typeName}";
    public string NullableTypeName { get; } = $"{FixedWidthGenerator.CoreNamespace}.{nullableTypeName}";
};

[Generator]
public class FixedWidthGenerator : IIncrementalGenerator
{
    public const string CoreNamespace = "FixedWidthLibraryCore.FixedWidth";

    private const string FixWidthAttributeName = "FixedWidthAttribute";
    private const string FixWidthName = "FixedWidth";

    private const string FixedWidthMarkerAttributeName = "FixedWidthMarkerAttribute";
    private const string FixedWidthMarkerName = "FixedWidthMarker";

    private static readonly Dictionary<string, FixedWidthAbstract> StandardTypeMappings = new()
    {
        ["String"] = new FixedWidthAbstract("FixedWidthString", "FixedWidthStringNullable"),
        ["Boolean"] = new FixedWidthAbstract("FixedWidthBool", "FixedWidthBoolNullable"),
        ["Int32"] = new FixedWidthAbstract("FixedWidthInt", "FixedWidthIntNullable"),
        ["Int64"] = new FixedWidthAbstract("FixedWidthLong", "FixedWidthLongNullable"),
        ["Decimal"] = new FixedWidthAbstract("FixedWidthDecimal", "FixedWidthDecimalNullable"),
        ["Double"] = new FixedWidthAbstract("FixedWidthDouble", "FixedWidthDoubleNullable"),
        ["Single"] = new FixedWidthAbstract("FixedWidthFloat", "FixedWidthFloatNullable"),
        ["Char"] = new FixedWidthAbstract("FixedWidthChar", "FixedWidthCharNullable"),
        ["DateTime"] = new FixedWidthAbstract("FixedWidthDateTime", "FixedWidthDateTimeNullable"),
        ["DateOnly"] = new FixedWidthAbstract("FixedWidthDateOnly", "FixedWidthDateOnlyNullable"),
        ["Byte"] = new FixedWidthAbstract("FixedWidthByte", "FixedWidthByteNullable"),
        ["SByte"] = new FixedWidthAbstract("FixedWidthSByte", "FixedWidthSByteNullable"),
        ["UInt32"] = new FixedWidthAbstract("FixedWidthUInt", "FixedWidthUIntNullable"),
        ["UInt64"] = new FixedWidthAbstract("FixedWidthULong", "FixedWidthULongNullable"),
        ["Int16"] = new FixedWidthAbstract("FixedWidthShort", "FixedWidthShortNullable"),
        ["UInt16"] = new FixedWidthAbstract("FixedWidthUShort", "FixedWidthUShortNullable"),
    };


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => Predicate(s),
                (ctx, _) => GetClassDeclarationForSourceGen(ctx));

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }

    private static bool Predicate(SyntaxNode syntaxNode)
    {
        return Helper.ClassHasAttribute(syntaxNode, FixedWidthMarkerName);
    }

    private static ClassDeclarationSyntax GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        return (ClassDeclarationSyntax)context.Node;
    }

    private static void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        foreach (var classDeclarationSyntax in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                continue;

            var properties = Helper.GetPropertiesWithFixedWidthAttribute(namedTypeSymbol);

            var (metaDataVales, fixedWidthLength) = GetMetaDataInformation(properties);

            var code = GenerateSourceText(compilation, namedTypeSymbol, classDeclarationSyntax, metaDataVales, fixedWidthLength, properties);

            var formattedCode = Helper.FormatCode(code);

            context.AddSource($"{classDeclarationSyntax.Identifier.Text}.FixedWidth.g.cs", SourceText.From(formattedCode, Encoding.UTF8));
        }
    }

    private static string GenerateSourceText(Compilation compilation, ISymbol namedTypeSymbol, BaseTypeDeclarationSyntax classDeclarationSyntax,
        ImmutableArray<string> metaDataVales, int fixedWidthLength, ImmutableArray<IPropertySymbol> properties)
    {
        var ns = namedTypeSymbol.ContainingNamespace.ToDisplayString();
        var accessibility = Helper.GetClassAccessibility(compilation, classDeclarationSyntax);
        var className = classDeclarationSyntax.Identifier.Text;
        var metaData = string.Join("\n", metaDataVales);
        var ctorAssignments = string.Join("\n", GetConstructorAssignments(properties));
        var sbWrites = string.Join("\n", GetStringBuilderWrites(properties));
        var streamWrites = string.Join("\n", GetStreamWrites(properties));
        var asyncStreamWrites = string.Join("\n", GetAsyncStreamWrites(properties));

        return $@"// <auto-generated/>

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using {CoreNamespace};

namespace {ns};

{accessibility} partial class {className}
{{
    public static class FixedWidthMetaData
    {{
        {metaData}
    }}

    public const int TotalFixedWidthLength = {fixedWidthLength};

    public {className}(ReadOnlySpan<char> line) {{
        {ctorAssignments}
    }}

    public StringBuilder WriteToStringBuilder(StringBuilder? stringBuilder = null)
    {{
        stringBuilder ??= new StringBuilder();

        {sbWrites}

        stringBuilder.AppendLine();

        return stringBuilder;
    }}

    public Stream WriteToStream(Stream stream)
    {{
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);

        {streamWrites}

        return stream;
    }}

    public async Task<Stream> WriteToStreamAsync(Stream stream)
    {{
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);

        {asyncStreamWrites}

        return stream;
    }}
}}
";
    }


    private static ImmutableArray<string> GetAsyncStreamWrites(ImmutableArray<IPropertySymbol> properties)
    {
        return
        [
            ..properties
                .Select(x => $"await FixedWidthMetaData.{x.Name}.WriteToStreamAsync({x.Name}, streamWriter);")
        ];
    }

    private static ImmutableArray<string> GetStreamWrites(ImmutableArray<IPropertySymbol> properties)
    {
        return
        [
            ..properties
                .Select(x => $"FixedWidthMetaData.{x.Name}.WriteToStream({x.Name}, streamWriter);")
        ];
    }

    private static ImmutableArray<string> GetStringBuilderWrites(ImmutableArray<IPropertySymbol> properties)
    {
        return
        [
            ..properties
                .Select(x => $"FixedWidthMetaData.{x.Name}.WriteToStringBuilder({x.Name}, stringBuilder);")
        ];
    }

    private static ImmutableArray<string> GetConstructorAssignments(ImmutableArray<IPropertySymbol> properties)
    {
        return
        [
            ..properties
                .Select(x => $"{x.Name} = FixedWidthMetaData.{x.Name}.Parse(line);")
        ];
    }

    private static (ImmutableArray<string> MetaDataVales, int Length) GetMetaDataInformation(
        ImmutableArray<IPropertySymbol> properties)
    {
        ImmutableArray<(string metaDataValue, int endPosition)> metaDataValues =
        [
            ..properties
                .Select(x =>
                {
                    var propertyName = x.Name;

                    var fixedWidthPropertyAttribute = Helper.GetFixedWidthAttribute(x);

                    var start = (int)(fixedWidthPropertyAttribute.ConstructorArguments[0].Value ?? 0);
                    var end = (int)(fixedWidthPropertyAttribute.ConstructorArguments[1].Value ?? 0);
                    var length = end - start; // Calculate actual length from start and end positions

                    var fixedWidthPropertiesAssignments = fixedWidthPropertyAttribute
                        .NamedArguments
                        .Select(kv => $"{kv.Key} = {kv.Value.ToCSharpString()}")
                        .ToImmutableArray();

                    var mapType = GetMapType(fixedWidthPropertyAttribute, x);

                    var metaDataValue = $"public static readonly {mapType} {propertyName} = new {mapType}({start}, {length}){{{string.Join(",", fixedWidthPropertiesAssignments)}}};";

                    return (metaDataValue, start, end);
                })
                .OrderBy(x => x.start)
                .Select(x => (x.metaDataValue, x.end))
        ];

        // TotalFixedWidthLength should be the max end position
        return ([..metaDataValues.Select(x => x.metaDataValue)],
            metaDataValues.Max(x => x.endPosition));
    }

    /// <summary>
    /// Gets either the type specified in the attribute property labeled "MapType",
    /// or uses the custom attribute class if it's not the standard FixedWidthAttribute,
    /// or gets a built-in type mapper. 
    /// </summary>
    /// <param name="fixedWidthPropertyAttribute"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    private static string GetMapType(AttributeData fixedWidthPropertyAttribute, IPropertySymbol x)
    {
        // First check if MapType is explicitly specified
        var mapTypeArg = fixedWidthPropertyAttribute
            .NamedArguments
            .FirstOrDefault(kv => kv.Key == "MapType")
            .Value
            .Value
            ?.ToString();

        if (mapTypeArg != null)
            return mapTypeArg;

        // If this is a custom attribute (not FixedWidthAttribute), use the attribute class name
        var attributeClassName = fixedWidthPropertyAttribute.AttributeClass?.Name;
        if (attributeClassName != null && attributeClassName != FixWidthAttributeName)
        {
            // Return the fully qualified name of the custom attribute
            return fixedWidthPropertyAttribute.AttributeClass!.ToDisplayString();
        }

        // Fall back to standard type mapping
        return GetSupportedTypeName(x);
    }

    private static string GetSupportedTypeName(IPropertySymbol x)
    {
        return x.Type.NullableAnnotation == NullableAnnotation.Annotated
            ? StandardTypeMappings[x.Type.Name].NullableTypeName
            : StandardTypeMappings[x.Type.Name].TypeName;
    }
}
