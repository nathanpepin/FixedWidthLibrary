namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthByteNullable(int start, int length) : FixedWidthElement<byte?>(start, length)
{
    public override byte? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return byte.TryParse(it, NumberStyles, CultureInfo, out var value)
            ? value
            : null;
    }

    public override string SerializeToString(byte? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}