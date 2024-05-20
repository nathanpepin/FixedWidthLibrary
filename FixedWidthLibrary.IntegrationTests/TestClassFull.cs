using System.Globalization;
using FixedWidthLibraryCore;

namespace FixedWidthLibrary.IntegrationTests;

[FixedWidthMarker]
public partial class TestClassFull
{
    [FixedWidth(1, 30, Format = "yyyyMMdd", PadCharacter = '*', Pad = Direction.Left, Trim = true, AutoTrim = false, DateTimeStyles = DateTimeStyles.None,
        AutoTrimDirection = Direction.Left, FalseValue = "ad", TrueValue = "DSF", IndexOffset = 1, NumberStyles = NumberStyles.Any,
        RemoveChars = "abcd", WhiteSpaceToNull = true)]
    public string FirstName { get; set; }

    [FixedWidth(31, 30, PadCharacter = '_', IndexOffset = 1)]
    public string? LastName { get; set; }

    [FixedWidth(61, 3, PadCharacter = '*', IndexOffset = 1)]
    public int Age { get; set; }

    [FixedWidth(64, 8, PadCharacter = '_', IndexOffset = 1, Format = "yyyyMMdd")]
    public DateTime DateOfBirth { get; set; }

    [FixedWidth(72, 1, PadCharacter = '*', IndexOffset = 1, TrueValue = "Y", FalseValue = "N")]
    public bool IsCool { get; set; }

    [FixedWidth(73, 1, PadCharacter = '_', IndexOffset = 1)]
    public char Gender { get; set; }

    [FixedWidth(74, 100, PadCharacter = '*', IndexOffset = 1)]
    public decimal Money { get; set; }

    [FixedWidth(174, 10, PadCharacter = '_', IndexOffset = 1)]
    public double Debt { get; set; }

    [FixedWidth(184, 10, PadCharacter = '*', IndexOffset = 1)]
    public float Wisdom { get; set; }

    [FixedWidth(194, 100, PadCharacter = '_', Pad = Direction.Right, IndexOffset = 1)]
    public long Height { get; set; }
    
    [FixedWidth(194, 100, PadCharacter = '_', Pad = Direction.Right, IndexOffset = 1)]
    public DateOnly D { get; set; }
}

[FixedWidthMarker]
public partial class FixedWidthDateOnly
{
    [FixedWidth(1, 10, PadCharacter = '*', IndexOffset = 1, Format = "yyyyMMdd")]
    public DateOnly Date { get; set; }
}