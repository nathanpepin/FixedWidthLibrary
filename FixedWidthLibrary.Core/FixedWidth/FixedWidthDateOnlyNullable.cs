#if NET6_0_OR_GREATER
namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthDateOnlyNullable(int start, int length) : FixedWidthElement<DateOnly?>(start, length)
{
    public override DateOnly? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseNullableString(line);

        return DateOnly.TryParseExact(it, Format, CultureInfo, DateTimeStyles, out var output)
            ? output
            : null;
    }

    public override string SerializeToString(DateOnly? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}
#endif