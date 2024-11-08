using Global;
using static Global.EasyObject;
using Global.Sample;
using System;
using Xunit;
using Newtonsoft.Json.Linq;
using System.IO;
using Jil;
using System.Windows.Forms.VisualStyles;

namespace Main;


static class Program
{
    const int maxTrial = 1;
    static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    static TimeSpan ts;
    static string bigJson;
    static void Demo00()
    {
        var cal = new Win32IntCalculator();
        var result = cal.Calculate("11 + 22");
        Echo(result, "result(1)");
        Assert.Equal(33, result);
        result = cal.Calculate(" (1 + 2) * 3 ");
        Echo(result, "result(2)");
        Assert.Equal(9, result);
    }
    static void Demo01()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers(); sw.Start();
        sw.Start();
        EasyObject eo = null;
        for (int c = 0; c < maxTrial; c++)
        {
            eo = FromJson(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■EasyObject(FromJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        GC.Collect();
        GC.WaitForPendingFinalizers(); sw.Start();
        string eoJson = null;
        for (int c = 0; c < maxTrial; c++)
        {
            eoJson = eo.ToJson(true);
        }
        sw.Stop();
        Console.WriteLine("■EasyObject(ToJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        eo = null;
        eoJson = null;
    }
    static void Demo02()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        sw.Start();
        for (int c = 0; c < maxTrial; c++)
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
        GC.Collect();
        GC.WaitForPendingFinalizers();
        sw.Start();
        for (int c = 0; c < maxTrial; c++)
        {
            using (var output = new StringWriter())
            {
                JSON.Serialize(output);
            }
        }
        sw.Stop();
        Console.WriteLine("■JIl(ToJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
    }
    [STAThread]
    static void Main(string[] args)
    {
        ShowDetail = true;
        Echo(new { args = args });

        if (args.Length == 0) return;

        int n = int.Parse(args[0]);
        Echo(n, "n");


        if (n == 0)
        {
            Demo00();
            return;
        }

        bigJson = File.ReadAllText("assets/qiita-9ea0c8fd43b61b01a8da.json");

        switch (n)
        {
            case 1:
                Demo01();
                break;
            case 2:
                Demo02();
                break;
        }

        return;

        //Echo(bigJson);
        GC.Collect();
        GC.WaitForPendingFinalizers(); sw.Start();
        for (int c = 0; c < maxTrial; c++)
        {
            JObject jsonObject = JObject.Parse(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■Newtonsoft.Json");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        GC.Collect();
        GC.WaitForPendingFinalizers();
#if false
        sw.Start();
        var w32parser = new Win32JsonParser();
        for (int c = 0; c < maxTrial; c++)
        {
            w32parser.Parse(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■Win32JsonParser");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
#endif

#if false
        var nlp = new Win32NLJsonParser(true);
        object nlr = nlp.Parse("""
            { "a": 123, "b": [11, true, false, null], //line comment
              "c": /*comment*/ "hello\nハロー©" }
            """);
        Echo(nlr);
#endif
        GC.Collect();
        GC.WaitForPendingFinalizers(); Win32NLJsonHandler handler;
        sw.Start();
        handler = new Win32NLJsonHandler(true, false);
        object nlhobj = null;
        for (int c = 0; c < maxTrial; c++)
        {
            nlhobj = handler.Parse(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■Win32NLJsonParser(FromJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        sw.Start();
        handler = new Win32NLJsonHandler(true, false);
        for (int c = 0; c < maxTrial; c++)
        {
            //using (var input = new StringReader(bigJson))
            {
                /*var result = */
                handler.Stringify(nlhobj, true);
            }
        }
        sw.Stop();
        Console.WriteLine("■Win32NLJsonParser(ToJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        sw.Start();
        sw.Start();
        EasyObject eo2 = null;
        for (int c = 0; c < maxTrial; c++)
        {
            eo2 = FromJson(bigJson);
        }
        sw.Stop();
        Console.WriteLine("■EasyObject(FromJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        sw.Start();
        for (int c = 0; c < maxTrial; c++)
        {
            var eoJson2 = eo2.ToJson(true);
        }
        sw.Stop();
        Console.WriteLine("■EasyObject(ToJson)");
        ts = sw.Elapsed;
        Console.WriteLine($"　{ts}");
        Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
        Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
    }
}