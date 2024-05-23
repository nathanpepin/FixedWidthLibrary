namespace FixedWidthLibraryCore.FixedWidth;

#if NET6_0_OR_GREATER
public sealed class FixedWidthDateOnly(int start, int length) : FixedWidthElement<DateOnly>(start, length)
{
    public override DateOnly Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        return DateOnly.ParseExact(it ?? throw new NullReferenceException(), Format, CultureInfo);
    }

    public override string SerializeToString(DateOnly value)
    {
        var output = value.ToString(Format, CultureInfo);
        return SerializeToFixedWidthString(output);
    }
}

#endif