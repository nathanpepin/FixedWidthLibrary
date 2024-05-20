using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public float ParseFloat(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        
        return float.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(float value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public float? ParseNullableFloat(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return float.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public string SerializeToString(float? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
    
    public void SetValue(ref float value, ReadOnlySpan<char> line) =>
        value = ParseFloat(line);

    public void SetNullableValue(ref float? value, ReadOnlySpan<char> line) =>
        value = ParseNullableFloat(line);
}

public class FixedWidthAttributeInt : FixedWidthAttribute
{
    public FixedWidthAttributeInt(int start, int length) : base(start, length)
    {
    }
}