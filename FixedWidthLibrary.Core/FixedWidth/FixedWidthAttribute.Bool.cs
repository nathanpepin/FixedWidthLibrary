using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public bool Assign(bool _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (string.IsNullOrWhiteSpace(it) && string.IsNullOrWhiteSpace(TrueValue))
            return true;

        if (string.IsNullOrWhiteSpace(it) && string.IsNullOrWhiteSpace(FalseValue))
            return false;

        if (it != null && it.Equals(TrueValue))
            return true;

        if (it != null && it.Equals(FalseValue))
            return false;

        throw new ArgumentOutOfRangeException(nameof(line));
    }

    public string SerializeToString(bool value)
    {
        var output = value ? TrueValue : FalseValue;
        return SerializeToString(output);
    }

    public bool? AssignNullable(bool? _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (string.IsNullOrWhiteSpace(it) && string.IsNullOrWhiteSpace(TrueValue))
            return true;

        if (string.IsNullOrWhiteSpace(it) && string.IsNullOrWhiteSpace(FalseValue))
            return false;

        if (it != null && it.Equals(TrueValue))
            return true;

        if (it != null && it.Equals(FalseValue))
            return false;

        return null;
    }

    public string SerializeToString(bool? value)
    {
        if (value is null)
            return SerializeToString(string.Empty);

        var output = value.Value ? TrueValue : FalseValue;
        return SerializeToString(output);
    }
}