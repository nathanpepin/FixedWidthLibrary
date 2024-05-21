namespace FixedWidthLibraryCore;

public sealed class FixedWidthDoubleNullable(int start, int length) : FixedWidthElement<double?>(start, length)
{
    public override double? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseNullableString(line);
        return double.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(double? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
}