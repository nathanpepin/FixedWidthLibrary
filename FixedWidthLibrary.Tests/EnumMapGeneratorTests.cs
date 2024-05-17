using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace FixedWidthLibrary.Tests;

public class EnumMapGeneratorTests
{
    private const string VectorClassText =
        """
        using System;
        using FixedWidthLibrary;
        using Generators;

        [EnumMapMarker]
        enum A {
        	[EnumNotMapped] NotMapped,
        	[EnumMap("BB")] B,
        	[EnumMap("CC", 1)] C
        }
        """;

    [Fact]
    public void GenerateReportMethod()
    {
        var generator = new EnumGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = CSharpCompilation.Create(nameof(FixedWidthGeneratorTests),
            new[] { CSharpSyntaxTree.ParseText(VectorClassText) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("Vector3.g.cs"));

        // Assert.Equal(ExpectedGeneratedClassText, generatedFileSyntax.GetText().ToString(),
        //     ignoreLineEndingDifferences: true);
    }
}