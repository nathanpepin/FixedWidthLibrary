namespace FixedWidthLibraryCore;

public class FixedWidthBoolNullable(int start, int length) : FixedWidthElement<bool?>(start, length)
{
    public override bool? Parse(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (string.IsNullOrWhiteSpace(it) && string.IsNullOrWhiteSpace(TrueValue))
            return true;

        if (string.IsNullOrWhiteSpace(it) && string.IsNullOrWhiteSpace(FalseValue))
            return false;

        if (it.Equals(TrueValue))
            return true;

        if (it.Equals(FalseValue))
            return false;

        return null;
    }

    public override string SerializeToString(bool? value)
    {
        if (value is null)
            return SerializeToFixedWidthString(string.Empty);

        var output = value.Value ? TrueValue : FalseValue;
        return SerializeToFixedWidthString(output);
    }
}