using System.Text;

namespace FixedWidthLibraryCore;

public sealed class FixedWidthDateTime(int start, int length) : FixedWidthElement<DateTime>(start, length)
{
    public override DateTime Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return DateTime.ParseExact(it, Format, CultureInfo);
    }

    public override string SerializeToString(DateTime value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
}