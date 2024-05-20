using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public long ParseLong(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        
        return long.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(long value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public long? ParseNullableLong(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return long.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public string SerializeToString(long? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
    
    public void SetValue(ref long value, ReadOnlySpan<char> line) =>
        value = ParseLong(line);

    public void SetNullableValue(ref long? value, ReadOnlySpan<char> line) =>
        value = ParseNullableLong(line);
}

