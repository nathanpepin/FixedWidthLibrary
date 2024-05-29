namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthULong(int start, int length) : FixedWidthElement<ulong>(start, length)
{
    public override ulong Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return ulong.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(ulong value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}