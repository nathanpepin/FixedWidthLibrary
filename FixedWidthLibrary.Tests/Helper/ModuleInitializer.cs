using System.Runtime.CompilerServices;
using VerifyTests;

namespace FixedWidthLibrary.Tests.Helper;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() =>
        VerifySourceGenerators.Initialize();
}