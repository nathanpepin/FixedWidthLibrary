using System.Text;

namespace FixedWidthLibraryCore;
public sealed class FixedWidthFloat(int start, int length) : FixedWidthElement<float>(start, length)
{
    public override float Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return float.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(float value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToString(output);
    }
}