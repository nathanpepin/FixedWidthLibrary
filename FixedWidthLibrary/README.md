### FixedWidthLibrary

Roslyn analyzer and source generator that builds high-performance readers and writers for fixed-width flat-file records from simple attributes. Pair this with `FixedWidthLibrary.Core` which provides the runtime attributes and types.

#### Install

Reference both packages in your project:

```
dotnet add package FixedWidthLibrary.Core
dotnet add package FixedWidthLibrary
```

The analyzer/source generator runs at build time; no code changes are required beyond adding attributes.

#### Define a record

```csharp
using FixedWidthLibraryCore.FixedWidthMarker;
using FixedWidthLibraryCore.FixedWidth;

[FixedWidthMarker]
public partial class OrderLine
{
    [FixedWidth(0, 8)]
    public string Sku { get; set; }

    [FixedWidth(8, 5)]
    public int Quantity { get; set; }

    [FixedWidth(13, 1, TrueValue = "Y", FalseValue = "N")]
    public bool Backordered { get; set; }
}
```

During build, the generator produces parsing and serialization members so you can do:

```csharp
var line = "ABC12345" + "00010" + "N" + "\r\n"; // width: 8 + 5 + 1
var record = new OrderLine(line);
// record.Sku == ABC12345; record.Quantity == 10; record.Backordered == false

var output = record.WriteToStringBuilder().ToString(); // Serialized fixed-width line
```

#### Notes

- Start positions are zero-based and lengths are fixed. The generator uses the provided offsets to slice input and to write padded output.
- Boolean fields can be mapped using `TrueValue`/`FalseValue` settings on the `FixedWidth` attribute.
- Use `WriteToStringBuilder()` or stream helpers generated for efficient output.

#### Packages

- `FixedWidthLibrary` (this package): analyzer + source generator.
- `FixedWidthLibrary.Core`: attributes and runtime helpers consumed by your app and used by the generator.

#### Attributes reference

For a complete list of all attributes and options you can use on your models (including constructor parameters, defaults, and typed variants like `FixedWidthInt`, `FixedWidthDateTime`, `FixedWidthBool`, etc.), see the Attributes reference in the Core package README:

- FixedWidthLibrary.Core → "Attributes reference" section

#### License

MIT
