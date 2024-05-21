namespace FixedWidthLibraryCore;

public class FixedWidthIntNullable(int start, int length) : FixedWidthElement<int?>(start, length)
{
    public override int? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return int.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(int? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}