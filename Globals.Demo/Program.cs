using Global;
using static Global.EasyObject;
using Global.Sample;
using System;

namespace Main;


static class Program
{
    [STAThread]
    static void Main(string[] originalArgs)
    {
        var cal = new IntCalculator("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """);
        var result = cal.Calculate("11 + 22");
        Echo(result, "result");
    }
}