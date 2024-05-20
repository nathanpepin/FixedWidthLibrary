using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public bool ParseBool(ReadOnlySpan<char> line)
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

    public string SerializeToString(bool value)
    {
        var output = value ? TrueValue : FalseValue;
        return SerializeToString(output);
    }

    public bool? ParseNullableBool(ReadOnlySpan<char> line)
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

    public string SerializeToString(bool? value)
    {
        if (value is null)
            return SerializeToString(string.Empty);

        var output = value.Value ? TrueValue : FalseValue;
        return SerializeToString(output);
    }

    public void SetValue(ref bool value, ReadOnlySpan<char> line) =>
        value = ParseBool(line);

    public void SetNullableValue(ref bool? value, ReadOnlySpan<char> line) =>
        value = ParseNullableBool(line);
}