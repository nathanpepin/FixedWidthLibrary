using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public char ParseChar(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);
        
        return it[0];
    }

    public string SerializeToString(char value)
    {
        return SerializeToString(value.ToString());
    }

    public char? ParseNullableChar(ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return it?[0];
    }

    public string SerializeToString(char? value)
    {
        return SerializeToString(value?.ToString());
    }
    
    public void SetValue(ref char value, ReadOnlySpan<char> line) =>
        value = ParseChar(line);

    public void SetNullableValue(ref char? value, ReadOnlySpan<char> line) =>
        value = ParseNullableChar(line);
}