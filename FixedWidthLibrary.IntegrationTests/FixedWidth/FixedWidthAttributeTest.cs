using System;
using JetBrains.Annotations;
using Xunit;
using FixedWidthLibraryCore;
using FluentAssertions;

namespace FixedWidthLibrary.IntegrationTests.FixedWidth;

[TestSubject(typeof(FixedWidthAttribute))]
public class FixedWidthAttributeTest
{
    private readonly FixedWidthAttribute _fixedWidthAttribute;

    public FixedWidthAttributeTest()
    {
        _fixedWidthAttribute = new FixedWidthAttribute(0, 5);
    }

    [Fact]
    public void AssignNullable_ReturnsExpectedValue_ForPaddedLeftValue()
    {
        _fixedWidthAttribute.Pad = Direction.Left;

        var actual = _fixedWidthAttribute.ParseNullableInt(" 12345");

        actual.Should().Be(123);
    }
}