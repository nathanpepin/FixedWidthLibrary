using System.Globalization;

namespace FixedWidthLibraryCore.FixedWidth.Values;

public enum CultureInfoValue
{
    CurrentCulture,
    InvariantCulture,
    CurrentUICulture,
    InstalledUICulture,
    DefaultThreadCurrentCulture,
    DefaultThreadCurrentUICulture
}

public static class CultureInfoValueExtensions
{
    public static CultureInfo ToCultureInfo(this CultureInfoValue cultureValue)
    {
        return cultureValue switch
        {
            CultureInfoValue.CurrentCulture => CultureInfo.CurrentCulture,
            CultureInfoValue.InvariantCulture => CultureInfo.InvariantCulture,
            CultureInfoValue.CurrentUICulture => CultureInfo.CurrentUICulture,
            CultureInfoValue.InstalledUICulture => CultureInfo.InstalledUICulture,
            CultureInfoValue.DefaultThreadCurrentCulture => CultureInfo.DefaultThreadCurrentCulture,
            CultureInfoValue.DefaultThreadCurrentUICulture => CultureInfo.DefaultThreadCurrentUICulture,
            _ => throw new ArgumentException("Invalid CultureInfoValue", nameof(cultureValue))
        };
    }
}