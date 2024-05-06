using Global;
using System;
using static Global.EasyObject;
namespace Exe;
public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            var pr = DLL0.API.Call("parse", Null.Add("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """.Replace("\r\n", "\n")).Add(" (1 + 2) * 3 "));
            Echo(pr, "pr");
        }
        catch (Exception e)
        {
            Log(e.ToString());
            Message(e.ToString(), "Exception");
        }
    }
}