namespace FixedWidthLibraryCore;

public sealed class FixedWidthCharNullable(int start, int length) : FixedWidthElement<char?>(start, length)
{
    public override char? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseNullableString(line);
        return it?[0];
    }

    public override string SerializeToString(char? value)
    {
        return SerializeToFixedWidthString(value.ToString());
    }
}