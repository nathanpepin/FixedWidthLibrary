using System;
using JetBrains.Annotations;
using Xunit;
using FixedWidthLibraryCore;
using FixedWidthLibraryCore.FixedWidth;
using FixedWidthLibraryCore.FixedWidth.Values;
using FixedWidthLibraryCore.FixedWidthMarker;
using FluentAssertions;

namespace FixedWidthLibrary.IntegrationTests.FixedWidth;

[TestSubject(typeof(FixedWidthAttribute))]
public class FixedWidthAttributeTest
{
    [Fact]
    public void AssignNullable_ReturnsExpectedValue_ForPaddedLeftValue()
    {
        //Arrange
        var fixedWidthAttribute = new FixedWidthInt(0, 6) { Pad = Direction.Left };
        const string testValue = " 12345";

        //Act
        var deserialized = fixedWidthAttribute.Parse(" 12345");
        var serialized = fixedWidthAttribute.SerializeToString(deserialized);

        //Assert
        deserialized.Should().BeInRange(12345, 12345);
        serialized.Should().Be(testValue);
    }

}

public sealed class CustomMap(int start, int length) : FixedWidthElement<bool>(start, length)
{
    public override bool Parse(ReadOnlySpan<char> line)
    {
        var value = ParseString(line);
        return value == "Indigo";
    }

    public override string SerializeToString(bool value)
    {
        return value ? "Indigo" : "BAAA";
    }
}
//
// [FixedWidthMarker]
// public partial class TestClassFull
// {
//     [FixedWidth(31, 30, PadCharacter = '_', IndexOffset = 1, MapType = typeof(CustomMap))]
//     public string? LastName { get; set; }
// }