using Globals;
using static Globals.Util;
using MyJson;
using static MyJson.MyData;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Esprima.Ast;

public class XUnitTest1
{
    private readonly ITestOutputHelper Out;
    public XUnitTest1(ITestOutputHelper testOutputHelper)
    {
        Out = testOutputHelper;
        MyData.DebugOutput = false;
        MyData.DecimalAsString = false;
        MyData.ForceASCII = false;
        MyData.NumberAsDecimal = false;
        MyData.XUnitOutput = Out;
        Echo("Setup() called");
    }
    [Fact]
    public void Test01()
    {
#if fales
        MyData
            .SetDecimalAsString(true)
            .SetForceASCII(false)
            .SetNumberAsDecimal(false)
            .SetXUnitOutput(Out);
#endif
        int port = Util.FreeTcpPort();
        Assert.True(port != 0, "port != 0");
        JsonClient cli = new JsonClient(@"ClassLibrary1.dll", typeof(Program).Assembly);
        var result = cli.Call("add2", new object[] { 11, 22 });
        MyData.Echo(result, "result");
    }
}
