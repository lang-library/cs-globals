using Global;
using static Global.GObject;
using Xunit;
using Xunit.Abstractions;

public class XUnitTest1
{
    private readonly ITestOutputHelper Out;
    void Print(object x, string title = null)
    {
        Out.WriteLine(GObject.ToPrintable(x, title));
    }
    public XUnitTest1(ITestOutputHelper testOutputHelper)
    {
        Out = testOutputHelper;
        GObject.ClearSettings();
    }
    [Fact]
    public void Test01()
    {
        Assert.Equal(Null.TypeValue, @null);
        int port = Util.FreeTcpPort();
        Assert.True(port != 0, "port != 0");
        JsonClient cli = new JsonClient(@"ClassLibrary1.dll", typeof(XUnitTest1).Assembly);
        var result = cli.Call("add2", GObject.FromObject(new object[] { 11, 33 }));
        Print(result, "result");
        Assert.Equal(44, result.Cast<int>());
    }
}
