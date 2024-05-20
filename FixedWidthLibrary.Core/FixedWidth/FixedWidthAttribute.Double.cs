using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public double ParseDouble(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        
        return double.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(double value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public double? ParseNullableDouble(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return double.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public string SerializeToString(double? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
    
    public void SetValue(ref double value, ReadOnlySpan<char> line) =>
        value = ParseDouble(line);

    public void SetNullableValue(ref double? value, ReadOnlySpan<char> line) =>
        value = ParseNullableDouble(line);
}