namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthDecimalNullable(int start, int length) : FixedWidthElement<decimal?>(start, length)
{
    public override decimal? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseNullableString(line);
        return decimal.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(decimal? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}