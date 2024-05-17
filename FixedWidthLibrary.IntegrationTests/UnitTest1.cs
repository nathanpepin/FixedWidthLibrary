namespace FixedWidthLibrary.IntegrationTests;

public class FixedWidthTests
{
    [Fact]
    public void Test()
    {
        var c = new MyClass
        {
            FirstName = "Jon",
            LastName = "Snow",
            Age = 22,
            DateOfBirth = new DateTime(1991,
                1,
                1),
            IsCool = true,
            Gender = 'M',
            Money = 10_123m,
            Debt = -300,
            Wisdom = -1.1f,
            Height = long.MaxValue
        };

        var text = c.WriteToStringBuilder().ToString();
        //
        var b = new MyClass(text.ToCharArray());
        //
        Assert.Equal("Jon", b.FirstName);
        Assert.Equal("Snow", b.LastName);
        Assert.Equal(22, b.Age);
        Assert.Equal(new DateTime(1991, 1, 1), b.DateOfBirth);
        Assert.True(b.IsCool);
        Assert.Equal('M', b.Gender);
        Assert.Equal(10_123m, b.Money);
        Assert.Equal(-300d, b.Debt);
        Assert.Equal(-1.1f, b.Wisdom);
        Assert.Equal(long.MaxValue, b.Height);
    }
}