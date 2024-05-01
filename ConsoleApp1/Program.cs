using Globals;
using MyJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            JsonClient cli = new JsonClient(@"ClassLibrary1.dll", typeof(Program).Assembly);
            var result = cli.Call("add2", new object[] { 11, 22 });
            MyData.Echo(result, "result");
        }
    }
}
