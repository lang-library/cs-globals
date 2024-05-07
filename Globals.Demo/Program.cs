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
        var cal = new IntCalculator();
        var result = cal.Calculate("11 + 22");
        Echo(result, "result");
    }
}