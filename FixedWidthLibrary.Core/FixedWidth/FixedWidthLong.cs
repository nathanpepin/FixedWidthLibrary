using System.Text;

namespace FixedWidthLibraryCore;

public sealed class FixedWidthLong(int start, int length) : FixedWidthElement<long>(start, length)
{
    public override long Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return long.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(long value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}