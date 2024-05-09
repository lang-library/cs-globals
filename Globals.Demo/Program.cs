using Global;
using static Global.EasyObject;
using Global.Sample;
using System;
using Xunit;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections;
using System.Linq;

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
#if true
        var jp = new Win32JsonParser();
        object o;
        o = jp.Parse("null");
        Echo(o, "o");
        o = jp.Parse("true");
        Echo(o, "o");
        o = jp.Parse("false");
        Echo(o, "o");
        o = jp.Parse("123");
        Echo(o, "o");
        o = jp.Parse(@"""helloハロー©""");
        Echo(o, "o");
        o = jp.Parse(@"[11, 22, ""helloハロー©""]");
        Echo(o, "o");
        o = jp.Parse(@"{ ""a"": 11, ""b"": 22, ""c"": ""helloハロー©""}");
        Echo(o, "o");
#endif
        string bigJson = File.ReadAllText("assets/qiita-9ea0c8fd43b61b01a8da.json");
        //Echo(bigJson);
        var sw = new System.Diagnostics.Stopwatch();
        TimeSpan ts;
        sw.Start();
        for (int c = 0; c < 5; c++)
        {
            //var test = FromJson(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■EasyObject");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        sw.Start();
        for (int c = 0; c < 5; c++)
        {
            //JObject jsonObject = JObject.Parse(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■Newtonsoft.Json");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        sw.Start();
        var w32parser = new Win32JsonParser();
        for (int c = 0; c < 5; c++)
        {
            //w32parser.Parse(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■Win32JsonParser");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");

        var nlp = new Win32NLJsonParser();
        var nlr = nlp.Parse("""
            { "a": 123, "b": [11, 22, 33], "c": null }
            """);
        Echo(nlr);
        Echo(nlr.ast.dict as IDictionary);
        Echo(nlr.ast.dict as System.Collections.Generic.IDictionary<string, object>);
        Echo(nlr.ast.dict.GetType().IsGenericType);
        var tp = nlr.ast.dict.GetType();
        if (tp.GetInterfaces().Any(
  i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>)))
        {
            Console.WriteLine($"Do something");
        }
        var ifc = IsGenericIDictionaryType(tp);
        Echo(ifc.FullName);
        if (ifc != null)
        {
            ProcessGenericIDictionaryType(nlr.ast.dict);
        }
    }
    public static void ProcessGenericIDictionaryType<T>(System.Collections.Generic.IDictionary<string, T> dict)
    {
        foreach(var key in dict.Keys)
        {
            Echo(key);
        }

    }
    public static Type IsGenericIDictionaryType(Type type)
    {
        if (type == null) return null;
        var ifs = type.GetInterfaces();
        foreach(var i in ifs )
        {
            if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
            {
                Type keyType = i.GetGenericArguments()[0];
                Type valType = i.GetGenericArguments()[1];
                //if (keyType == typeof(string))
                {
                    return valType;
                }

            }
        }
        return null;
    }
}