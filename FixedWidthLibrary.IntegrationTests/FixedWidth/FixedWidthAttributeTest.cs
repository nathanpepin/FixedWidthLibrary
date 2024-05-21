using System;
using JetBrains.Annotations;
using Xunit;
using FixedWidthLibraryCore;
using FluentAssertions;

namespace FixedWidthLibrary.IntegrationTests.FixedWidth;

[TestSubject(typeof(FixedWidthAttribute))]
public class FixedWidthAttributeTest
{
    [Fact]
    public void AssignNullable_ReturnsExpectedValue_ForPaddedLeftValue()
    {
        //Arrange
        var fixedWidthAttribute = new FixedWidthInt(0, 5) { Pad = Direction.Left };
        const string testValue = " 12345";

        //Act
        var deserialized = fixedWidthAttribute.Parse(" 12345");
        var serialized = fixedWidthAttribute.SerializeToString(deserialized);

        //Assert
        deserialized.Should().Be(123);
        serialized.Should().Be(testValue);
    }
}