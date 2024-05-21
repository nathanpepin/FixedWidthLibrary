namespace FixedWidthLibraryCore;

public sealed class FixedWidthStringNullable(int start, int length) : FixedWidthElement<string?>(start, length)
{
    public override string? Parse(ReadOnlySpan<char> line)
    {
        return ParseNullableString(line);
    }

    public override string SerializeToString(string? value)
    {
        return SerializeToFixedWidthString(value);
    }
}