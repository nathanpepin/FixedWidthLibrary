using System.Text;

namespace FixedWidthLibraryCore.FixedWidth;

public abstract class FixedWidthElement<T>(int start, int length) : FixedWidthAttribute(start, length)
{
    public abstract T Parse(ReadOnlySpan<char> line);

    public abstract string SerializeToString(T? value);

    public StringBuilder WriteToStringBuilder(T value, StringBuilder? stringBuilder = null)
    {
        var output = SerializeToString(value);
        return stringBuilder is null
            ? new StringBuilder(output)
            : stringBuilder.Append(output);
    }

    public StreamWriter WriteToStream(T value, StreamWriter streamWriter)
    {
        var output = SerializeToString(value);
        streamWriter.WriteAsync(output);
        return streamWriter;
    }

    public async Task<StreamWriter> WriteToStreamAsync(T value, StreamWriter streamWriter)
    {
        var output = SerializeToString(value);
        await streamWriter.WriteAsync(output);
        return streamWriter;
    }
}