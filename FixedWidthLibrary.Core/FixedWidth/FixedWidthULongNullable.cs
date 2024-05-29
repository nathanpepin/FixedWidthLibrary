namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthULongNullable(int start, int length) : FixedWidthElement<ulong?>(start, length)
{
    public override ulong? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return ulong.TryParse(it, NumberStyles, CultureInfo, out var value)
            ? value
            : null;
    }

    public override string SerializeToString(ulong? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}