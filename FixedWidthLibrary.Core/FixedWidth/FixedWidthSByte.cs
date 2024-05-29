namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthSByte(int start, int length) : FixedWidthElement<sbyte>(start, length)
{
    public override sbyte Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return sbyte.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(sbyte value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}