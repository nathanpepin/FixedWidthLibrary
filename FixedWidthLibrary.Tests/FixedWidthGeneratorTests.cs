using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace FixedWidthLibrary.Tests;

public class FixedWidthGeneratorTests
{
    private const string VectorClassText =
        """
        using System;
        using FixedWidthLibrary;
        using Generators;

        namespace FixedWidthLibrary.Sample;

        [FixedWidthMarker]
        public partial class MyClass
        {
            [FixedWidth(0, 30, Format = "yyyyMMdd", PadCharacter = 'I', Pad = Direction.Left, Trim = true, AutoTrim = false, DateTimeStyles = DateTimeStyles.None,
                AutoTrimDirection = Direction.Left, FalseValue = "ad", TrueValue = "DSF", IndexOffset = 1, NumberStyles = NumberStyles.Any,
                RemoveChars = "abcd", WhiteSpaceToNull = true)]
            public string FirstName { get; set; }
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