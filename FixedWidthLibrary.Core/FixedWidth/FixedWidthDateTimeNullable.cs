namespace FixedWidthLibraryCore;

public sealed class FixedWidthDateTimeNullable(int start, int length) : FixedWidthElement<DateTime?>(start, length)
{
    public override DateTime? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return DateTime.TryParseExact(it, Format, CultureInfo, DateTimeStyles, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(DateTime? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
}