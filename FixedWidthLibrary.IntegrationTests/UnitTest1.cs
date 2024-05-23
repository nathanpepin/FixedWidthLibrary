// using System.Text;
// using FixedWidthLibraryCore;
// using FluentAssertions;
//
// namespace FixedWidthLibrary.IntegrationTests;
//
// public class FixedWidthTests
// {
//     [Fact]
//     public void Test()
//     {
//         var c = new TestClassFull
//         {
//             FirstName = "Jon",
//             LastName = "Snow",
//             Age = 22,
//             DateOfBirth = new DateTime(1991,
//                 1,
//                 1),
//             IsCool = true,
//             Gender = 'M',
//             Money = 10_123m,
//             Debt = -300,
//             Wisdom = -1.1f,
//             Height = long.MaxValue
//         };
//
//         var text = c.WriteToStringBuilder().ToString();
//         //
//         var b = new TestClassFull(text.ToCharArray());
//         //
//         Assert.Equal("Jon", b.FirstName);
//         Assert.Equal("Snow", b.LastName);
//         Assert.Equal(22, b.Age);
//         Assert.Equal(new DateTime(1991, 1, 1), b.DateOfBirth);
//         Assert.True(b.IsCool);
//         Assert.Equal('M', b.Gender);
//         Assert.Equal(10_123m, b.Money);
//         Assert.Equal(-300d, b.Debt);
//         Assert.Equal(-1.1f, b.Wisdom);
//         Assert.Equal(long.MaxValue, b.Height);
//     }
//
//     [Fact]
//     public void TestDateOnly()
//     {
//         //Arrange
//         var start = new FixedWidthDateOnly { Date = new DateOnly(1991, 1, 1) };
//
//         //Act
//         var serialized = start.WriteToStringBuilder().ToString();
//         var deserialized = new FixedWidthDateOnly(serialized.ToCharArray());
//         
//         //Assert
//         serialized.Should().Be("**19910101\r\n");
//         deserialized.Should().BeEquivalentTo(start);
//     }
//
//     [Fact]
//     public void WriteToStreamTest()
//     {
//         var c = new TestClassFull
//         {
//             FirstName = "Jon",
//             LastName = "Snow",
//             Age = 22,
//             DateOfBirth = new DateTime(1991,
//                 1,
//                 1),
//             IsCool = true,
//             Gender = 'M',
//             Money = 10_123m,
//             Debt = -300,
//             Wisdom = -1.1f,
//             Height = long.MaxValue
//         };
//         
//         using var stream = new MemoryStream();
//         c.WriteToStream(stream);
//
//
//         stream.Seek(0, SeekOrigin.Begin);
//         
//
//         using var streamReader = new StreamReader(stream);
//         var text = streamReader.ReadToEnd();
//         ;
//     }
// }