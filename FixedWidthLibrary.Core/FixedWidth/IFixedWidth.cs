namespace FixedWidthLibraryCore.FixedWidth;

public interface IFixedWidth
{
    object Parse(ReadOnlySpan<char> line);
    string SerializeToString(object value);
}

public interface IFixedWidth<T> : IFixedWidth
{
    new T Parse(ReadOnlySpan<char> line);
    string SerializeToString(T value);
}