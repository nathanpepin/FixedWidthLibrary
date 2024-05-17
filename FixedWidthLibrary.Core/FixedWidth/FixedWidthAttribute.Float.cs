using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public float Assign(float _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (it is null) throw new NullReferenceException("Float cannot be null");

        return float.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(float value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public float? AssignNullable(float? _, ReadOnlySpan<char> line)
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
}