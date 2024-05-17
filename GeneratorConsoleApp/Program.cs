// See https://aka.ms/new-console-template for more information

using GeneratorConsoleApp;

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

var a = c.WriteToStringBuilder().ToString();

Console.WriteLine();