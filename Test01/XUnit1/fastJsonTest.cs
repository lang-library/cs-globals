using Globals;
using static Globals.Util;
using MyJson;
using static MyJson.MyData;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Esprima.Ast;
using System.Collections.Generic;
using System.IO;
using Jil;
using Jint.Native;

public class FastJsonTest
{
    private readonly ITestOutputHelper Out;
    public FastJsonTest(ITestOutputHelper testOutputHelper)
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
        MyData.ShowDetail = true;
        //fastJSON.JSON.Parameters.EnableAnonymousTypes = true;
        List<int> ls = new List<int>();
        ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });
        var s = fastJSON.JSON.ToJSON(ls);
        var o = fastJSON.JSON.ToObject(s); // long[] {1,2,3,4,5,10}
        Echo(o);
    }
    [Fact]
    public void Test02()
    {
        MyData.ShowDetail = true;
        var s = fastJSON.JSON.ToJSON(new { a = 123 });
        var o = fastJSON.JSON.ToObject(s); // long[] {1,2,3,4,5,10}
        Echo(o);
    }
    [Fact]
    public void Test03()
    {
        MyData.ShowDetail = true;
        string json;
        using (var output = new StringWriter())
        {
            JSON.Serialize(
                new
                {
                    MyInt = 1,
                    MyString = "hello world",
                    // etc.
                },
                output
            );
            json = output.ToString();
        }
        Echo(json, "json");
        dynamic result;
        using (var input = new StringReader(json))
        {
            result = JSON.DeserializeDynamic(input);
        }
        Echo(result, "result");
        Echo(result.MyInt, "result.MyInt");
        Echo(result.GetType(), "result.GetType()");
    }
}
