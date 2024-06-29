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
using Jil;

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

        var nlp = new Win32NLJsonParser(true);
        object nlr = nlp.Parse("""
            { "a": 123, "b": [11, true, false, null], //line comment
              "c": /*comment*/ "hello\nハロー©" }
            """);
        Echo(nlr);

        Win32NLJsonHandler handler;
        sw.Start();
        handler = new Win32NLJsonHandler(true, false);
        for (int c = 0; c < 5; c++)
        {
            handler.Parse(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■Win32NLJsonParser(FromJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");

        sw.Start();
        handler = new Win32NLJsonHandler(true, false);
        for (int c = 0; c < 5; c++)
        {
            using (var input = new StringReader(bigJson))
            {
                /*var result = */
                handler.Stringify(nlr, true);
            }
        }
        sw.Stop();
        Console.WriteLine("■Win32NLJsonParser(ToJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");

        sw.Start();
        for (int c = 0; c < 5; c++)
        {
            using (var input = new StringReader(bigJson))
            {
                /*var result = */
                JSON.DeserializeDynamic(input);
            }
        }
        sw.Stop();
        Console.WriteLine("■JIl(FromJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");

        sw.Start();
        for (int c = 0; c < 5; c++)
        {
            using (var output = new StringWriter())
            {
                JSON.Serialize(nlr);
            }
        }
        sw.Stop();
        Console.WriteLine("■JIl(ToJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");

        Busybox.Run("pwd");
    }
}