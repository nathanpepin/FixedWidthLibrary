namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthUIntNullable(int start, int length) : FixedWidthElement<uint?>(start, length)
{
    public override uint? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return uint.TryParse(it, NumberStyles, CultureInfo, out var value)
            ? value
            : null;
    }

    public override string SerializeToString(uint? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}