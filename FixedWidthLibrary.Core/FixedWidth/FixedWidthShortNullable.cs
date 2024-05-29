namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthShortNullable(int start, int length) : FixedWidthElement<short?>(start, length)
{
    public override short? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return short.TryParse(it, NumberStyles, CultureInfo, out var value)
            ? value
            : null;
    }

    public override string SerializeToString(short? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}