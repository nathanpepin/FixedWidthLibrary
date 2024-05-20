using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FixedWidthLibrary;

public static class Helper
{
    public static string FormatCode(string code) =>
        CSharpSyntaxTree.ParseText(code).GetRoot().NormalizeWhitespace().ToFullString();
}