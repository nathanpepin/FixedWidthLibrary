using System.Globalization;
using FixedWidthLibraryCore.FixedWidth.Values;

namespace FixedWidthLibraryCore.FixedWidth;

/// <summary>
/// When the containing class/struct/record has the FixedWidthMarker attribute, all properties with this attribute will be placed into a dictionary
/// and additional methods will be serialized and deserialize fixed width records.
///
/// Configure the properties as needed for the data type being serialized, otherwise leave default.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FixedWidthAttribute(int start, int length) : Attribute, IFixedWidth
{
    public int IndexOffset { get; set; }

    public int Start => start - IndexOffset;

    public int Length { get; } = length;

    public string Format { get; set; } = string.Empty;

    public char PadCharacter { get; set; } = ' ';

    public Direction Pad { get; set; } = Direction.Left;

    public bool Trim { get; set; } = true;

    public bool WhiteSpaceToNull { get; set; }

    public bool AutoTrim { get; set; }

    public Direction AutoTrimDirection = Direction.Right;

    public string RemoveChars { get; set; } = string.Empty;

    public NumberStyles NumberStyles { get; set; } = NumberStyles.Any;

    internal CultureInfo CultureInfo => CultureInfoValue.ToCultureInfo();

    public CultureInfoValue CultureInfoValue { get; set; } = CultureInfoValue.InvariantCulture;

    public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.AssumeLocal;

    public string? TrueValue { get; set; }

    public string? FalseValue { get; set; }

    internal StringComparer StringComparer => StringComparerValue.ToStringComparer();
    public StringComparerValue StringComparerValue { get; set; } = StringComparerValue.InvariantCulture;

    public Type? MapType { get; set; }

    protected string ParseString(ReadOnlySpan<char> line)
    {
        return ParseNullableString(line) ?? throw new NullReferenceException();
    }

    protected string? ParseNullableString(ReadOnlySpan<char> line)
    {
        var it = Trim
            ? Pad == Direction.Left
                ? line.Slice(Start, Length).TrimStart(PadCharacter).ToString()
                : line.Slice(Start, Length).TrimEnd(PadCharacter).ToString()
            : line.Slice(Start, Length).ToString();

        if (WhiteSpaceToNull && string.IsNullOrWhiteSpace(it))
            return null;

        return RemoveChars.Length <= 0
            ? it
            : RemoveChars.Aggregate(it, (current, c) => current.Trim(c));
    }

    protected string SerializeToFixedWidthString(string? it)
    {
        if (it is null)
            return new string(PadCharacter, Length);

        if (it.Length > Length && !AutoTrim)
            throw new IndexOutOfRangeException(
                $"String '{it}' is longer than the max length of {Length} and auto trim is turned off.");

        if (it.Length <= Length || !AutoTrim)
        {
            return Pad == Direction.Left
                ? it.PadLeft(Length, PadCharacter)
                : it.PadRight(Length, PadCharacter);
        }

        if (AutoTrimDirection == Direction.Left)
        {
            it = it.Substring(0, Length);
        }
        else
        {
            it = new string(
                it
                    .Where((_, i) => i + 1 > Length)
                    .ToArray()
            );
        }

        return Pad == Direction.Left
            ? it.PadLeft(Length, PadCharacter)
            : it.PadRight(Length, PadCharacter);
    }

    public object Parse(ReadOnlySpan<char> line)
    {
        return ParseString(line);
    }

    public string SerializeToString(object value)
    {
        if (value is string s)
            return SerializeToFixedWidthString(s);

        throw new InvalidCastException("Could not convert object to string");
    }
}