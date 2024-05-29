using FixedWidthLibraryCore.FixedWidth;
using FluentAssertions;
using JetBrains.Annotations;

namespace FixedWidthLibrary.IntegrationTests.FixedWidth;

public interface IFixedWidthTest<T>
{
    void Input_Should_Equal_Theory(string input, T value);
    void Input_Should_Serialize_To_Input(string input);
}

public interface IFixedWidthNonNullableTest<T> : IFixedWidthTest<T>
{
    void Input_Should_Throw_If_Not_Valid(string input);
}

public interface IFixedWidthNullableTest<T> : IFixedWidthTest<T>
{
    void Input_Should_Be_Null_If_Not_Valid(string input);
}

[TestSubject(typeof(FixedWidthBoolNullable))]
public class FixedWidthTest : IFixedWidthNonNullableTest<bool>
{
    [Theory]
    [InlineData("Y", true)]
    [InlineData("N", false)]
    public void Input_Should_Equal_Theory(string input, bool value)
    {
        //Arrange
        var testClass = new NullableBoolTestClass(input);

        //Act
        var result = testClass.Result;

        //Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData("Y")]
    [InlineData("N")]
    public void Input_Should_Serialize_To_Input(string input)
    {
        //Arrange
        var testClass = new BoolTestClass(input);

        //Act
        var result = testClass.WriteToStringBuilder().ToString()[..^2];

        //Assert
        input.Should().Be(result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("X")]
    public void Input_Should_Throw_If_Not_Valid(string input)
    {
        //Arrange
        var testClass = () => new BoolTestClass(input);

        //Assert
        testClass.Should().Throw<ArgumentOutOfRangeException>();
    }
}

[FixedWidthMarker]
public partial class BoolTestClass
{
    [FixedWidth(0, 1, TrueValue = "Y", FalseValue = "N")]
    public bool TestBool { get; set; }
}