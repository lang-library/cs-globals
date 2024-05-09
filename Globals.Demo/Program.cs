using Global;
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
        var cal = new Win32IntCalculator();
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
        var jp = new Win32JsonParser();
        object o;
        o = jp.Parse("123");
        Echo(o, "o");
    }
}