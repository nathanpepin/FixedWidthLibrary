namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthUShort(int start, int length) : FixedWidthElement<ushort>(start, length)
{
    public override ushort Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return ushort.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(ushort value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}