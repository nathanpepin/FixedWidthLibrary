namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthByte(int start, int length) : FixedWidthElement<byte>(start, length)
{
    public override byte Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return byte.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(byte value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}