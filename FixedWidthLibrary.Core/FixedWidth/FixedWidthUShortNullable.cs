namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthUShortNullable(int start, int length) : FixedWidthElement<ushort?>(start, length)
{
    public override ushort? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return ushort.TryParse(it, NumberStyles, CultureInfo, out var value)
            ? value
            : null;
    }

    public override string SerializeToString(ushort? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}