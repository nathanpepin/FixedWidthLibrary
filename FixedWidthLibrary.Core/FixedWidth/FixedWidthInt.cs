namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthInt(int start, int length) : FixedWidthElement<int>(start, length)
{
    public override int Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return int.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(int value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}