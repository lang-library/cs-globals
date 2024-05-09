using Global;
using static Global.EasyObject;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;
using System;
using Global.Sample;

public class XUnitTest1
{
    private readonly ITestOutputHelper Out;
    void Print(object x, string title = null)
    {
        Out.WriteLine(EasyObject.ToPrintable(x, title));
    }
    public XUnitTest1(ITestOutputHelper testOutputHelper)
    {
        Out = testOutputHelper;
        EasyObject.ClearSettings();
    }
    [Fact]
    public void Test01()
    {
        AST ast = Win32Parser.Parse("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """, " (1 + 2) * 3 ");
        Print(ast, "ast");
        Assert.Equal("Additive", ast.name);
        Assert.Equal("Additive/1", ast.name_choice);
        Assert.Equal("Multiplicative", ast.nodes[0].name);
        Assert.Equal("Multiplicative/0", ast.nodes[0].name_choice);
    }
    [Fact]
    public void Test02()
    {
        var exception1 = Assert.Throws<Exception>(() => {
            AST ast = Win32Parser.Parse("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """, " xxx ");
        });
        Assert.Equal("[input_error]", exception1.Message);
    }
    [Fact]
    public void Test03()
    {
        var exception1 = Assert.Throws<Exception>(() => {
            AST ast = Win32Parser.Parse("""
    bad grammar!
    """, " xxx ");
        });
        Assert.Equal("[grammar_error]", exception1.Message);
    }
    [Fact]
    public void Test04()
    {
        var cal = new IntCalculator();
        var result = cal.Calculate("11 + 22");
        Print(result, "result");
        Assert.Equal(33, result);
    }
    [Fact]
    public void Test05()
    {
        var cal = new IntCalculator();
        var result = cal.Calculate(" (1 + 2) * 3 ");
        Print(result, "result");
        Assert.Equal(9, result);
    }
}
