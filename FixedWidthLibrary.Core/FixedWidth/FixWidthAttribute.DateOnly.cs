namespace FixedWidthLibraryCore;

#if NET6_0_OR_GREATER
public partial class FixedWidthAttribute
{
    public DateOnly ParseDateOnly(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return DateOnly.ParseExact(it ?? throw new NullReferenceException(), Format, CultureInfo);
    }

    public string SerializeToString(DateOnly value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public DateOnly? ParseNullableDateOnly(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return DateOnly.TryParseExact(it, Format, CultureInfo, DateTimeStyles, out var output)
            ? output
            : null;
    }

    public string SerializeToString(DateOnly? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
    
    public void SetValue(ref DateOnly value, ReadOnlySpan<char> line) =>
        value = ParseDateOnly(line);

    public void SetNullableValue(ref DateOnly? value, ReadOnlySpan<char> line) =>
        value = ParseNullableDateOnly(line);
}
#endif