namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthFloatNullable(int start, int length) : FixedWidthElement<float?>(start, length)
{
    public override float? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return float.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(float? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}