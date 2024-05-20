using System;
using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public int ParseInt(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        
        return int.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(int value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public int? ParseNullableInt(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return int.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public string SerializeToString(int? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
    
    public void SetValue(ref int value, ReadOnlySpan<char> line) =>
        value = ParseInt(line);

    public void SetNullableValue(ref int? value, ReadOnlySpan<char> line) =>
        value = ParseNullableInt(line);
}