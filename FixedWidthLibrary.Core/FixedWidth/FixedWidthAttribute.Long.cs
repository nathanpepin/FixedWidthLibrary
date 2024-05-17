using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public long Assign(long l, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (it is null) throw new NullReferenceException("Long cannot be null");

        return long.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(long value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public long? AssignNullable(long? _, ReadOnlySpan<char> line)
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
}