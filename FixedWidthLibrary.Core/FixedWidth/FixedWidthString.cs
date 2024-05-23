namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthString(int start, int length) : FixedWidthElement<string>(start, length)
{
    public override string Parse(ReadOnlySpan<char> line)
    {
        return ParseString(line);
    }

    public override string SerializeToString(string value)
    {
        return SerializeToFixedWidthString(value);
    }
}