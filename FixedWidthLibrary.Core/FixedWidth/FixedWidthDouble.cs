namespace FixedWidthLibraryCore.FixedWidth;

public sealed class FixedWidthDouble(int start, int length) : FixedWidthElement<double>(start, length)
{
    public override double Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return double.Parse(it, NumberStyles, CultureInfo);
    }

    public override string SerializeToString(double value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}