namespace FixedWidthLibraryCore.EnumMapper;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class EnumMapAttribute : Attribute
{
    public EnumMapAttribute(string value, int version = 0)
    {
        Value = value;
        Version = version == 0 ? 0 : version;
    }

    public string Value { get; }

    public int? Version { get; }
}