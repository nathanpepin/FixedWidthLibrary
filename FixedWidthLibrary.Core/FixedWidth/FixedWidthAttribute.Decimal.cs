using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public decimal Assign(decimal _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (it is null) throw new NullReferenceException("Decimal cannot be null");

        return decimal.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(decimal value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public decimal? AssignNullable(decimal? _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return decimal.TryParse(it, NumberStyles, CultureInfo, out var output)
            ? output
            : null;
    }

    public string SerializeToString(decimal? value)
    {
        var output = value?.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
}