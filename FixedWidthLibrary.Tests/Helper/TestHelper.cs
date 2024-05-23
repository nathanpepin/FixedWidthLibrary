using System;
using System.Linq;
using System.Threading.Tasks;
using FixedWidthLibraryCore.FixedWidth;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;

namespace FixedWidthLibrary.Tests.Helper;

public static class TestHelper
{
    public static Task Verify<T>(string source) where T : IIncrementalGenerator, new()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.Location)
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => !s.Contains("xunit"))
            .Select(s => MetadataReference.CreateFromFile(s))
            .Append(MetadataReference.CreateFromFile(
                typeof(FixedWidthAttribute).Assembly.Location))
            .ToList();


        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        var generator = new T();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);
        
        return Verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }
}