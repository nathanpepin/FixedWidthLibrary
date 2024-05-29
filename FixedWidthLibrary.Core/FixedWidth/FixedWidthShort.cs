namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthShort(int start, int length) : FixedWidthElement<short>(start, length)
{
    public override short Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return short.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(short value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}