using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public DateTime ParseDateTime(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return DateTime.ParseExact(it, Format, CultureInfo);
    }

    public string SerializeToString(DateTime value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public DateTime? ParseNullableDateTime(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return DateTime.TryParseExact(it, Format, CultureInfo, DateTimeStyles, out var output)
            ? output
            : null;
    }

    public string SerializeToString(DateTime? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public void SetValue(ref DateTime value, ReadOnlySpan<char> line) =>
        value = ParseDateTime(line);

    public void SetNullableValue(ref DateTime? value, ReadOnlySpan<char> line) =>
        value = ParseNullableDateTime(line);
}