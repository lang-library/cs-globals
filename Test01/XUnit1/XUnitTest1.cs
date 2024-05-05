using Global;
using static Global.Util;
//using MyJson;
//using static MyJson.MyData;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Esprima.Ast;
using Global;

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
        int port = Util.FreeTcpPort();
        Assert.True(port != 0, "port != 0");
        JsonClient cli = new JsonClient(@"ClassLibrary1.dll", typeof(XUnitTest1).Assembly);
        var result = cli.Call("add2", EasyObject.FromObject(new object[] { 11, 33 }));
        Print(result, "result");
        Assert.Equal(44, result.Cast<int>());
    }
}
