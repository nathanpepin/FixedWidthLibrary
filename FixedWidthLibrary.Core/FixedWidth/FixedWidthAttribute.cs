using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FixedWidthLibraryCore;

/// <summary>
/// When the containing class/struct/record has the FixedWidthMarker attribute, all properties with this attribute will be placed into a dictionary
/// and additional methods will be serialized and deserialize fixed width records.
///
/// Configure the properties as needed for the data type being serialized, otherwise leave default.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public partial class FixedWidthAttribute : Attribute
{
    public FixedWidthAttribute(int start, int length)
    {
        _start = start;
        Length = length;
    }

    public int IndexOffset { get; set; }

    private int _start;

    public int Start => _start - IndexOffset;

    public int Length { get; }

    public string? Format { get; set; }

    public char PadCharacter { get; set; } = ' ';

    public Direction Pad { get; set; } = Direction.Left;

    public bool Trim { get; set; } = true;

    public bool WhiteSpaceToNull { get; set; }

    public bool AutoTrim { get; set; }

    public Direction AutoTrimDirection = Direction.Right;

    public string RemoveChars { get; set; } = string.Empty;

    public NumberStyles NumberStyles { get; set; } = System.Globalization.NumberStyles.Any;

    internal CultureInfo CultureInfo => CultureInfoValue.ToCultureInfo();

    public CultureInfoValue CultureInfoValue { get; set; } = CultureInfoValue.InvariantCulture;

    public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.AssumeLocal;

    public string? TrueValue { get; set; }

    public string? FalseValue { get; set; }

    internal StringComparer StringComparer => StringComparerValue.ToStringComparer();
    public StringComparerValue StringComparerValue { get; set; } = StringComparerValue.InvariantCulture;
}