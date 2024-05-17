using System;
using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public int Assign(int _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (it is null) throw new NullReferenceException("Int cannot be null");

        return int.Parse(it, NumberStyles, CultureInfo);
    }

    public string SerializeToString(int value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }

    public int? AssignNullable(int? _, ReadOnlySpan<char> line)
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
}