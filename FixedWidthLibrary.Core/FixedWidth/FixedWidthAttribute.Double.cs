using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public double Assign(double _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (it is null) throw new NullReferenceException("Double cannot be null");

        return double.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(double value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public double? AssignNullable(double? _, ReadOnlySpan<char> line)
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
}