namespace FixedWidthLibraryCore;

public enum StringComparerValue
{
    Ordinal,
    CurrentCulture,
    InvariantCulture,
    OrdinalIgnoreCase,
    CurrentCultureIgnoreCase,
    InvariantCultureIgnoreCase
}

public static class StringComparerValueExtensions
{
    public static StringComparer ToStringComparer(this StringComparerValue it)
    {
        return it switch
        {
            StringComparerValue.Ordinal => StringComparer.Ordinal,
            StringComparerValue.CurrentCulture => StringComparer.CurrentCulture,
            StringComparerValue.InvariantCulture => StringComparer.InvariantCulture,
            StringComparerValue.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
            StringComparerValue.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
            StringComparerValue.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
            _ => throw new ArgumentOutOfRangeException(nameof(it), it, null)
        };
    }
}