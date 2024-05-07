using Global;
using static Global.EasyObject;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;
using System;

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
        EasyObject pr = DLL0.API.Call("parse", new EasyObject().Add("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """).Add(" (1 + 2) * 3 "));
        //Print(pr, "pr");
        Assert.Equal("Additive", (string)pr.Dynamic.name);
        Assert.Equal("Multiplicative", (string)pr.Dynamic.nodes[0].name);
    }
    [Fact]
    public void Test02()
    {
        var exception1 = Assert.Throws<Exception>(() => {
            EasyObject pr = DLL0.API.Call("parse", new EasyObject().Add("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """).Add(" xxx "));
        });
        Assert.Equal("[input_error]", exception1.Message);
    }
    [Fact]
    public void Test03()
    {
        var exception1 = Assert.Throws<Exception>(() => {
            EasyObject pr = DLL0.API.Call("parse", new EasyObject().Add("""
    bad grammar!
    """).Add(" xxx "));
        });
        Assert.Equal("[grammar_error]", exception1.Message);
    }
}
