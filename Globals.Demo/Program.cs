﻿using Global;
using static Global.EasyObject;
using Global.Sample;
using System;
using Xunit;

namespace Main;


static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ShowDetail = true;
        Echo(new { args = args });
        var cal = new IntCalculator();
        var result = cal.Calculate("11 + 22");
        Echo(result, "result(1)");
        Assert.Equal(33, result);
        result = cal.Calculate(" (1 + 2) * 3 ");
        Echo(result, "result(2)");
        Assert.Equal(9, result);
        Echo(Null);
        Echo(DateTime.Now);
        Echo(new { a = 123 });
        Echo(FromObject(Null));
        Echo(FromObject(DateTime.Now));
        Echo(FromObject(new { a = 123 }));
        var r = Win32Parser.Parse2("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """, " (1 + 2) * 3 ");
        Echo(r, "r");
    }
}