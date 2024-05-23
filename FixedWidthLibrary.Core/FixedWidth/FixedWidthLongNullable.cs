namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthLongNullable(int start, int length) : FixedWidthElement<long?>(start, length)
{
    public override long? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseNullableString(line);
        return long.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(long? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}