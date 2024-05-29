using JetBrains.Annotations;

namespace FixedWidthLibrary.IntegrationTests.FixedWidth;

[TestSubject(typeof(FixedWidthBoolNullable))]
public class FixedWidthBoolNullableTest
{
    [Theory]
    [InlineData("Y", true)]
    [InlineData("N", false)]
    public void Input_Should_Equal_Theory(string input, bool? value)
    {
        //Arrange
        var testClass = new NullableBoolTestClass(input);
        
        //Act & Assert
        Assert.Equal(value, testClass.Result);
    }

    [Theory]
    [InlineData("Y")]
    [InlineData("N")]
    public void Input_Should_Serialize_To_Input(string input)
    {
        //Arrange
        var testClass = new NullableBoolTestClass(input);

        //Act
        var result = testClass.WriteToStringBuilder().ToString()[..^2];

        //Assert
        Assert.Equal(input, result);
    }
}

[FixedWidthMarker]
internal partial class NullableBoolTestClass
{
    [FixedWidth(0, 1, TrueValue = "Y", FalseValue = "N")] public bool Result { get; set; }
}