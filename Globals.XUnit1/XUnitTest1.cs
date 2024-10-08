﻿using Global;
using static Global.EasyObject;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;
using System;
using Global.Sample;
using System.Web.UI.WebControls;

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
        var parser = new Win32Parser("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """);
        var ast = parser.Parse(" (1 + 2) * 3 ");
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
        var parser = new Win32Parser("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """);
        var ast = parser.Parse("xyz");
        });
        Assert.Equal("[input_error]", exception1.Message);
    }
    [Fact]
    public void Test03()
    {
        var exception1 = Assert.Throws<Exception>(() => {
            var parser = new Win32Parser("bad grammar!");
            var ast = parser.Parse("xyz");
        });
        Assert.Equal("[grammar_error]", exception1.Message);
    }
    [Fact]
    public void Test04()
    {
        var cal = new Win32IntCalculator();
        var result = cal.Calculate("11 + 22");
        Print(result, "result");
        Assert.Equal(33, result);
    }
    [Fact]
    public void Test05()
    {
        var cal = new Win32IntCalculator();
        var result = cal.Calculate(" (1 + 2) * 3 ");
        Print(result, "result");
        Assert.Equal(9, result);
    }
}
