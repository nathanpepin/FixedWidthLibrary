using System.Linq;
using FixedWidthLibrary.Tests.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;

namespace FixedWidthLibrary.Tests;

public class EnumMapGeneratorTests
{
    [Fact]
    public void GenerateReportMethod()
    {
        const string source =
            """
            using System;
            using FixedWidthLibrary;
            using Generators;

            namespace Tests;
            
            [EnumMapMarker]
            enum TestEnum {
            	[EnumNotMapped] NotMapped,
            	[EnumMap("A")] A,
            	[EnumMap("B")] B,
            	[EnumMap("C", 1)] C
            }
            """;

        TestHelper.Verify<EnumGenerator>(source);
    }
}