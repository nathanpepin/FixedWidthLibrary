namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthUInt(int start, int length) : FixedWidthElement<uint>(start, length)
{
    public override uint Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return uint.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(uint value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}