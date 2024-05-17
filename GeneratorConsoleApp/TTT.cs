using System.Globalization;
using System.Text;
using FixedWidthLibraryCore;

namespace GeneratorConsoleApp;

public class TTT
{
    [FixedWidth(1, 30, Format = "yyyyMMdd", PadCharacter = '*', Pad = Direction.Left, Trim = true, AutoTrim = false, DateTimeStyles = DateTimeStyles.None,
        AutoTrimDirection = Direction.Left, FalseValue = "ad", TrueValue = "DSF", IndexOffset = 1, NumberStyles = NumberStyles.Any,
        RemoveChars = "abcd", WhiteSpaceToNull = true)]
    public string FirstName { get; set; }

    [FixedWidth(31, 30, PadCharacter = '_', IndexOffset = 1)]
    public string? LastName { get; set; }

    [FixedWidth(61, 3, PadCharacter = '*', IndexOffset = 1)]
    public int Age { get; set; }

    [FixedWidth(64, 8, PadCharacter = '_', IndexOffset = 1, Format = "yyyyMMdd")]
    public DateTime DateOfBirth { get; set; }

    [FixedWidth(72, 1, PadCharacter = '*', IndexOffset = 1, TrueValue = "Y", FalseValue = "N")]
    public bool IsCool { get; set; }

    [FixedWidth(73, 1, PadCharacter = '_', IndexOffset = 1)]
    public char Gender { get; set; }

    [FixedWidth(74, 100, PadCharacter = '*', IndexOffset = 1)]
    public decimal Money { get; set; }

    [FixedWidth(174, 10, PadCharacter = '_', IndexOffset = 1)]
    public double Debt { get; set; }

    [FixedWidth(184, 10, PadCharacter = '*', IndexOffset = 1)]
    public float Wisdom { get; set; }

    [FixedWidth(194, 100, PadCharacter = '_', Pad = Direction.Right, IndexOffset = 1)]
    public long Height { get; set; }
    
    public static Dictionary<string, FixedWidthLibraryCore.FixedWidthAttribute> FixedWidthAttributes = new Dictionary<string, FixedWidthLibraryCore.FixedWidthAttribute>
    {
        {
            "FirstName",
            new FixedWidthLibraryCore.FixedWidthAttribute(1, 30)
            {
                Format = "yyyyMMdd",
                PadCharacter = '*',
                Pad = FixedWidthLibraryCore.Direction.Left,
                Trim = true,
                AutoTrim = false,
                DateTimeStyles = System.Globalization.DateTimeStyles.None,
                AutoTrimDirection = FixedWidthLibraryCore.Direction.Left,
                FalseValue = "ad",
                TrueValue = "DSF",
                IndexOffset = 1,
                NumberStyles = System.Globalization.NumberStyles.Any,
                RemoveChars = "abcd",
                WhiteSpaceToNull = true
            }
        },
        {
            "LastName",
            new FixedWidthLibraryCore.FixedWidthAttribute(31, 30)
            {
                PadCharacter = '_',
                IndexOffset = 1
            }
        },
        {
            "Age",
            new FixedWidthLibraryCore.FixedWidthAttribute(61, 3)
            {
                PadCharacter = '*',
                IndexOffset = 1
            }
        },
        {
            "DateOfBirth",
            new FixedWidthLibraryCore.FixedWidthAttribute(64, 8)
            {
                PadCharacter = '_',
                IndexOffset = 1,
                Format = "yyyyMMdd"
            }
        },
        {
            "IsCool",
            new FixedWidthLibraryCore.FixedWidthAttribute(72, 1)
            {
                PadCharacter = '*',
                IndexOffset = 1,
                TrueValue = "Y",
                FalseValue = "N"
            }
        },
        {
            "Gender",
            new FixedWidthLibraryCore.FixedWidthAttribute(73, 1)
            {
                PadCharacter = '_',
                IndexOffset = 1
            }
        },
        {
            "Money",
            new FixedWidthLibraryCore.FixedWidthAttribute(74, 100)
            {
                PadCharacter = '*',
                IndexOffset = 1
            }
        },
        {
            "Debt",
            new FixedWidthLibraryCore.FixedWidthAttribute(174, 10)
            {
                PadCharacter = '_',
                IndexOffset = 1
            }
        },
        {
            "Wisdom",
            new FixedWidthLibraryCore.FixedWidthAttribute(184, 10)
            {
                PadCharacter = '*',
                IndexOffset = 1
            }
        },
        {
            "Height",
            new FixedWidthLibraryCore.FixedWidthAttribute(194, 100)
            {
                PadCharacter = '_',
                Pad = FixedWidthLibraryCore.Direction.Right,
                IndexOffset = 1
            }
        }
    };
    public const int TotalFixedWidthLength = 293;
    public TTT(ReadOnlySpan<char> line)
    {
        FirstName = FixedWidthAttributes["FirstName"].Assign(FirstName, line);
        LastName = FixedWidthAttributes["LastName"].AssignNullable(LastName, line);
        Age = FixedWidthAttributes["Age"].Assign(Age, line);
        DateOfBirth = FixedWidthAttributes["DateOfBirth"].Assign(DateOfBirth, line);
        IsCool = FixedWidthAttributes["IsCool"].Assign(IsCool, line);
        Gender = FixedWidthAttributes["Gender"].Assign(Gender, line);
        Money = FixedWidthAttributes["Money"].Assign(Money, line);
        Debt = FixedWidthAttributes["Debt"].Assign(Debt, line);
        Wisdom = FixedWidthAttributes["Wisdom"].Assign(Wisdom, line);
        Height = FixedWidthAttributes["Height"].Assign(Height, line);
    }

    public TTT()
    {
    }

    public StringBuilder WriteToStringBuilder(StringBuilder? stringBuilder = null)
    {
        stringBuilder ??= new StringBuilder();
        FixedWidthAttributes["FirstName"].WriteToStringBuilder(FirstName, stringBuilder);
        FixedWidthAttributes["LastName"].WriteToStringBuilder(LastName, stringBuilder);
        FixedWidthAttributes["Age"].WriteToStringBuilder(Age, stringBuilder);
        FixedWidthAttributes["DateOfBirth"].WriteToStringBuilder(DateOfBirth, stringBuilder);
        FixedWidthAttributes["IsCool"].WriteToStringBuilder(IsCool, stringBuilder);
        FixedWidthAttributes["Gender"].WriteToStringBuilder(Gender, stringBuilder);
        FixedWidthAttributes["Money"].WriteToStringBuilder(Money, stringBuilder);
        FixedWidthAttributes["Debt"].WriteToStringBuilder(Debt, stringBuilder);
        FixedWidthAttributes["Wisdom"].WriteToStringBuilder(Wisdom, stringBuilder);
        FixedWidthAttributes["Height"].WriteToStringBuilder(Height, stringBuilder);
        stringBuilder.AppendLine();
        return stringBuilder;
    }

    public StreamWriter WriteToStream<T>(StreamWriter streamWriter)
    {
        FixedWidthAttributes["FirstName"].WriteToStream(FirstName, streamWriter);
        FixedWidthAttributes["LastName"].WriteToStream(LastName, streamWriter);
        FixedWidthAttributes["Age"].WriteToStream(Age, streamWriter);
        FixedWidthAttributes["DateOfBirth"].WriteToStream(DateOfBirth, streamWriter);
        FixedWidthAttributes["IsCool"].WriteToStream(IsCool, streamWriter);
        FixedWidthAttributes["Gender"].WriteToStream(Gender, streamWriter);
        FixedWidthAttributes["Money"].WriteToStream(Money, streamWriter);
        FixedWidthAttributes["Debt"].WriteToStream(Debt, streamWriter);
        FixedWidthAttributes["Wisdom"].WriteToStream(Wisdom, streamWriter);
        FixedWidthAttributes["Height"].WriteToStream(Height, streamWriter);
        return streamWriter;
    }

    public async Task<StreamWriter> WriteToStreamAsync<T>(T value, StreamWriter streamWriter)
    {
        await FixedWidthAttributes["FirstName"].WriteToStreamAsync(FirstName, streamWriter);
        await FixedWidthAttributes["LastName"].WriteToStreamAsync(LastName, streamWriter);
        await FixedWidthAttributes["Age"].WriteToStreamAsync(Age, streamWriter);
        await FixedWidthAttributes["DateOfBirth"].WriteToStreamAsync(DateOfBirth, streamWriter);
        await FixedWidthAttributes["IsCool"].WriteToStreamAsync(IsCool, streamWriter);
        await FixedWidthAttributes["Gender"].WriteToStreamAsync(Gender, streamWriter);
        await FixedWidthAttributes["Money"].WriteToStreamAsync(Money, streamWriter);
        await FixedWidthAttributes["Debt"].WriteToStreamAsync(Debt, streamWriter);
        await FixedWidthAttributes["Wisdom"].WriteToStreamAsync(Wisdom, streamWriter);
        await FixedWidthAttributes["Height"].WriteToStreamAsync(Height, streamWriter);
        return streamWriter;
    }
}