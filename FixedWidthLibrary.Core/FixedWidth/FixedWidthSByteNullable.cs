namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthSByteNullable(int start, int length) : FixedWidthElement<sbyte?>(start, length)
{
    public override sbyte? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return sbyte.TryParse(it, NumberStyles, CultureInfo, out var value)
            ? value
            : null;
    }

    public override string SerializeToString(sbyte? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}