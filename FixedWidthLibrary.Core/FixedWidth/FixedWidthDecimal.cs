using System.Text;

namespace FixedWidthLibraryCore;

public sealed class FixedWidthDecimal(int start, int length) : FixedWidthElement<decimal>(start, length)
{
    public override decimal Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return decimal.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(decimal value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
}