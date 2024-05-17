using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public char Assign(char _, ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        if (it is null) throw new NullReferenceException("Char cannot be null");

        return it[0];
    }

    public string SerializeToString(char value)
    {
        return SerializeToString(value.ToString());
    }

    public char? AssignNullable(char? _,  ReadOnlySpan<char> line)
    {
        var it = ParseString(line);

        return it?[0];
    }

    public string SerializeToString(char? value)
    {
        return SerializeToString(value?.ToString());
    }
}