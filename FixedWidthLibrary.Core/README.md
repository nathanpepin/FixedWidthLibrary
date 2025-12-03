### FixedWidthLibrary.Core

Core primitives for working with fixed-width flat-file records. This package provides the attributes, types, and helpers used by the `FixedWidthLibrary` source generator/analyzer to generate fast readers and writers for your models.

#### Install

```
dotnet add package FixedWidthLibrary.Core
```

For source generation, also add the analyzer/generator package to your project:

```
dotnet add package FixedWidthLibrary
```

#### Quick start

1) Define a partial model and mark it with `FixedWidthMarker`.

2) Annotate properties with `FixedWidth` to specify the start index, length, and formatting options. The `FixedWidthLibrary` generator will create parsing and writing helpers at build time.

```csharp
using FixedWidthLibraryCore.FixedWidthMarker;
using FixedWidthLibraryCore.FixedWidth;

[FixedWidthMarker]
public partial class CustomerRecord
{
    // Start at 0, length 10
    [FixedWidth(0, 10)]
    public string Id { get; set; }

    // Start at 10, length 20
    [FixedWidth(10, 20)]
    public string Name { get; set; }

    // Start at 30, length 1, treat Y/N as boolean
    [FixedWidth(30, 1, TrueValue = "Y", FalseValue = "N")]
    public bool IsActive { get; set; }
}

// Generated members (by FixedWidthLibrary) typically include:
// - A constructor that accepts a single fixed-width line
// - WriteToStringBuilder()/WriteToSpan() to serialize back to fixed width
```

Usage example (assuming the analyzer is referenced):

```csharp
// Parse
var record = new CustomerRecord("000012345John Smith          Y\r\n");
Console.WriteLine(record.Id);       // 000012345
Console.WriteLine(record.Name);     // John Smith
Console.WriteLine(record.IsActive); // True

// Serialize
var line = record.WriteToStringBuilder().ToString();
```

#### Why two packages?

- `FixedWidthLibrary.Core`: runtime types and attributes. Reference this from your application and from any project that declares fixed-width models.
- `FixedWidthLibrary`: Roslyn analyzer and source generator that produces the parsing/serialization code during build.

#### License

MIT

---

### Attributes reference

This package exposes the attributes you use to describe your fixed-width models. Below is a complete reference of all available attributes and options.

#### FixedWidthMarkerAttribute

Namespace: `FixedWidthLibraryCore.FixedWidthMarker`

Applied to: `class`, `struct` (including `record class`/`record struct`)

Purpose: Marks a type as a fixed-width record. When present, the source generator will discover and process any properties annotated with `FixedWidth` (and the typed variants below) and generate parsing/serialization helpers for the containing type.

Usage:

```csharp
using FixedWidthLibraryCore.FixedWidthMarker;

[FixedWidthMarker]
public partial class CustomerRecord { /* properties with FixedWidth* attributes */ }
```

#### FixedWidthAttribute (base property attribute)

Namespace: `FixedWidthLibraryCore.FixedWidth`

Applied to: properties on a type annotated with `FixedWidthMarker`

Ctor parameters:
- `FixedWidth(int start, int length)`
  - `start`: zero-based start index of the field within the fixed-width line (you can offset via `IndexOffset`).
  - `length`: the field's fixed width.

Common options (properties on the attribute, with defaults):
- `int IndexOffset { get; set; }` — default `0`. When non-zero, the effective start becomes `start - IndexOffset` (useful for 1-based specs).
- `int Start` — computed from ctor `start` and `IndexOffset` (read-only).
- `int Length` — from ctor `length` (read-only).
- `string Format { get; set; } = string.Empty` — format string used when parsing/serializing numeric/date types.
- `char PadCharacter { get; set; } = ' '` — padding character used during serialization and for trimming.
- `Direction Pad { get; set; } = Direction.Left` — side on which padding is applied when writing (`Left` or `Right`).
- `bool Trim { get; set; } = true` — if true, trims padding from the parsed slice (left or right depending on `Pad`).
- `bool WhiteSpaceToNull { get; set; } = false` — if true, whitespace-only input becomes `null` for nullable targets.
- `bool AutoTrim { get; set; } = false` — if true, values longer than `Length` are auto-trimmed instead of throwing.
- `Direction AutoTrimDirection = Direction.Right` — which side is trimmed when `AutoTrim` is enabled.
- `string RemoveChars { get; set; } = string.Empty` — if set, each character in this string will be trimmed from both ends of the parsed value (after `Trim`).
- `NumberStyles NumberStyles { get; set; } = NumberStyles.Any` — numeric parse styles for integral/floating types.
- `CultureInfoValue CultureInfoValue { get; set; } = CultureInfoValue.InvariantCulture` — culture used for parse/format (see `CultureInfoValue` enum below).
- `DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.AssumeLocal` — styles for `DateTime` parsing.
- `string? TrueValue { get; set; }` — literal used to represent `true` for boolean fields.
- `string? FalseValue { get; set; }` — literal used to represent `false` for boolean fields.
- `StringComparerValue StringComparerValue { get; set; } = StringComparerValue.InvariantCulture` — comparer option for string comparisons where applicable.
- `Type? MapType { get; set; }` — optional custom mapping type (advanced scenarios).

Supporting enums (namespace `FixedWidthLibraryCore.FixedWidth.Values`):
- `Direction { Left, Right }`
- `CultureInfoValue { Default, CurrentCulture, InvariantCulture, CurrentUICulture, InstalledUICulture, DefaultThreadCurrentCulture, DefaultThreadCurrentUICulture }`
- `StringComparerValue { CurrentCulture, InvariantCulture, Ordinal, CurrentCultureIgnoreCase, InvariantCultureIgnoreCase, OrdinalIgnoreCase }` (see enum in source for exact members)

Notes:
- Trimming behavior during parse is guided by `Trim` and `Pad`: when `Pad` is `Left`, parse trims from the left using `PadCharacter`; when `Pad` is `Right`, parse trims from the right.
- During serialization, values are padded to `Length` on the side indicated by `Pad`. If `AutoTrim` is `false` and the value exceeds `Length`, an exception is thrown.

#### Typed FixedWidth attributes

These concrete attributes target specific CLR types and inherit all options from `FixedWidthAttribute`. They vary only in their parse/format behavior and target CLR type. Each has the same constructor shape: `(int start, int length)`.

Unless noted otherwise, each type listed below also has a nullable counterpart with the `Nullable` suffix that maps to the nullable CLR type and returns `null` if parsing fails or the input is blank (subject to `WhiteSpaceToNull`).

Strings:
- `FixedWidthString` → `string`
- `FixedWidthStringNullable` → `string?`

Booleans:
- `FixedWidthBool` → `bool` (uses `TrueValue`/`FalseValue` to map literals; if both are null/empty and the field is blank, defaults to `false`/`true` per implementation rules)
- `FixedWidthBoolNullable` → `bool?`

Characters:
- `FixedWidthChar` → `char`
- `FixedWidthCharNullable` → `char?`

Integral numbers:
- `FixedWidthByte` / `FixedWidthByteNullable` → `byte` / `byte?`
- `FixedWidthSByte` / `FixedWidthSByteNullable` → `sbyte` / `sbyte?`
- `FixedWidthShort` / `FixedWidthShortNullable` → `short` / `short?`
- `FixedWidthUShort` / `FixedWidthUShortNullable` → `ushort` / `ushort?`
- `FixedWidthInt` / `FixedWidthIntNullable` → `int` / `int?`
- `FixedWidthUInt` / `FixedWidthUIntNullable` → `uint` / `uint?`
- `FixedWidthLong` / `FixedWidthLongNullable` → `long` / `long?`
- `FixedWidthULong` / `FixedWidthULongNullable` → `ulong` / `ulong?`

Floating-point and decimal:
- `FixedWidthFloat` / `FixedWidthFloatNullable` → `float` / `float?`
- `FixedWidthDouble` / `FixedWidthDoubleNullable` → `double` / `double?`
- `FixedWidthDecimal` / `FixedWidthDecimalNullable` → `decimal` / `decimal?`

Dates and time:
- `FixedWidthDateTime` / `FixedWidthDateTimeNullable` → `DateTime` / `DateTime?` (uses `Format` and `DateTimeStyles`)
- `FixedWidthDateOnly` / `FixedWidthDateOnlyNullable` → `DateOnly` / `DateOnly?` (available on .NET 6+; uses `Format`)

Example usages:

```csharp
using FixedWidthLibraryCore.FixedWidth;

// Numeric with culture and format
[FixedWidth(0, 8, Format = "00000000", Pad = Direction.Left, PadCharacter = '0')]
public int Id { get; set; }

// Date with explicit format
[FixedWidth(8, 8, Format = "yyyyMMdd")]
public DateTime OrderDate { get; set; }

// Boolean using Y/N mapping
[FixedWidth(16, 1, TrueValue = "Y", FalseValue = "N")]
public bool Active { get; set; }

// Optional number; blanks become null
[FixedWidth(17, 5, WhiteSpaceToNull = true)]
public int? OptionalCount { get; set; }
```

Tips:
- Set `IndexOffset = 1` when your field specification is 1-based.
- Use `RemoveChars` to strip separators like `,` or `$` before numeric parsing.
- For right-aligned text fields, set `Pad = Direction.Right` so parsing trims from the right and writing pads on the right.
