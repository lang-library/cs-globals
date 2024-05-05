using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Security.Authentication;
using Global;

public class ExceptionTest
{
    private readonly ITestOutputHelper Out;
    void Print(object x, string title = null)
    {
        Out.WriteLine(EasyObject.ToPrintable(x, title));
    }
    public ExceptionTest(ITestOutputHelper testOutputHelper)
    {
        Out = testOutputHelper;
        Print("Setup() called");
    }
    [Fact]
    public void Test01()
    {
#if false
        var exception1 = Assert.Throws<ArgumentException>(() => { new MyNumber(null); });
        Assert.Equal("Argument is null", exception1.Message);
        var exception2 = Assert.Throws<ArgumentException>(() => { new MyNumber("abc"); });
        Assert.Equal("Argument is not numeric: System.String", exception2.Message);
#endif
    }
}
