namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthChar(int start, int length) : FixedWidthElement<char>(start, length)
{
    public override char Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return it[0];
    }

    public override string SerializeToString(char value)
    {
        return SerializeToFixedWidthString(value.ToString());
    }
}