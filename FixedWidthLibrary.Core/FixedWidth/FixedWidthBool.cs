using System.Text;

namespace FixedWidthLibraryCore;

public sealed class FixedWidthBool(int start, int length) : FixedWidthElement<bool>(start, length)
{
    public override bool Parse(ReadOnlySpan<char> line)
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

        throw new ArgumentOutOfRangeException(nameof(line));
    }

    public override string SerializeToString(bool value)
    {
        var output = value ? TrueValue : FalseValue;
        return SerializeToString(output);
    }
}