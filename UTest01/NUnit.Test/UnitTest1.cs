using Global;
using static Global.GObject;
using System;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Echo("Setup() called");
        GObject.ClearSettings();
    }

    [Test]
    public void Test011()
    {
        int port = Util.FreeTcpPort();
        Assert.That(port != 0, Is.EqualTo(true));
        JsonClient cli = new JsonClient(@"ClassLibrary1.dll", typeof(Tests).Assembly);
        var result = cli.Call("add2", GObject.FromObject(new object[] { 11, 33 }));
        Echo(result, "result");
        Assert.That(result.Cast<int>(), Is.EqualTo(44));
    }
    [Test]
    public void Test012()
    {
        var pr = DLL0.API.Call("parse", new GObject().Add("""
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """).Add(" (1 + 2) * 3 "));
        Echo(pr, "pr");
        int port = Util.FreeTcpPort();
        Assert.That(port != 0, Is.EqualTo(true));
        JsonClient cli = new JsonClient(@"ClassLibrary1.dll", typeof(Tests).Assembly);
        var result = cli.Call("add2", GObject.FromObject(new object[] { 11, 33 }));
        Echo(result, "result");
        Assert.That(result.Cast<int>(), Is.EqualTo(44));
    }
}