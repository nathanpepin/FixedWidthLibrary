using System.Text;

namespace FixedWidthLibraryCore;

public partial class FixedWidthAttribute
{
    public string ParseString(ReadOnlySpan<char> line)
    {
        return ParseNullableString(line) ?? throw new NullReferenceException();
    }

    public string? ParseNullableString(ReadOnlySpan<char> line)
    {
        var it = Trim
            ? Pad == Direction.Left
                ? line.Slice(Start, Length).TrimStart(PadCharacter).ToString()
                : line.Slice(Start, Length).TrimEnd(PadCharacter).ToString()
            : line.Slice(Start, Length).ToString();

        if (WhiteSpaceToNull && string.IsNullOrWhiteSpace(it))
            return null;

        return RemoveChars.Length <= 0
            ? it
            : RemoveChars.Aggregate(it, (current, c) => current.Trim(c));
    }

    public string SerializeToString(string? it)
    {
        if (it is null)
            return new string(PadCharacter, Length);

        if (it.Length > Length && !AutoTrim)
            throw new IndexOutOfRangeException(
                $"String '{it}' is longer than the max length of {Length} and auto trim is turned off.");

        if (it.Length <= Length || !AutoTrim)
        {
            return Pad == Direction.Left
                ? it.PadLeft(Length, PadCharacter)
                : it.PadRight(Length, PadCharacter);
        }

        if (AutoTrimDirection == Direction.Left)
        {
            it = it.Substring(0, Length);
        }
        else
        {
            it = new string(
                it
                    .Where((x, i) => i + 1 > Length)
                    .ToArray()
            );
        }

        return Pad == Direction.Left
            ? it.PadLeft(Length, PadCharacter)
            : it.PadRight(Length, PadCharacter);
    }

    public void SetValue(ref string value, ReadOnlySpan<char> line) =>
        value = ParseString(line) ?? throw new NullReferenceException();

    public void SetNullableValue(ref string? value, ReadOnlySpan<char> line) =>
        value = ParseString(line);

    public string GetStringValue<T>(T value)
    {
        return value switch
        {
            string it => SerializeToString(it),
            bool it => SerializeToString(it),
            char it => SerializeToString(it),
            DateTime it => SerializeToString(it),
            decimal it => SerializeToString(it),
            double it => SerializeToString(it),
            float it => SerializeToString(it),
            int it => SerializeToString(it),
            long it => SerializeToString(it),
#if NET6_0_OR_GREATER
            DateOnly it => SerializeToString(it),
#endif
            _ => throw new NotSupportedException($"Type {value?.GetType().Name} is not supported")
        };
    }

    public StringBuilder WriteToStringBuilder<T>(T value, StringBuilder? stringBuilder = null)
    {
        var output = GetStringValue(value);

        return stringBuilder is null
            ? new StringBuilder(output)
            : stringBuilder.Append(output);
    }

    public StreamWriter WriteToStream<T>(T value, StreamWriter streamWriter)
    {
        var output = GetStringValue(value);

        streamWriter.WriteAsync(output);

        return streamWriter;
    }

    public async Task<StreamWriter> WriteToStreamAsync<T>(T value, StreamWriter streamWriter)
    {
        var output = GetStringValue(value);

        await streamWriter.WriteAsync(output);

        return streamWriter;
    }
}