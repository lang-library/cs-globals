using MyJson;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Xunit;

namespace nuget_tools.Globals0_Native;

public class Service
{
    static Service()
    {
        //Initializer.Initialize();
    }
    protected static MyJS myjs = new MyJS();
    public static void Init(string[] asmSpecList)
    {
        myjs.Init(asmSpecList, new Assembly[] { typeof(Service).Assembly });
    }
    public static void SetValue(string name, dynamic? value)
    {
        myjs.SetValue(name, value);
    }
    public static dynamic? GetValue(string name)
    {
        return myjs.GetValue(name);
    }
    public static void Execute(string script, object[] vars = null)
    {
        myjs.Execute(script, vars);
    }
    public static dynamic? Evaluate(string script, object[] vars = null)
    {
        return myjs.Evaluate(script, vars);
    }
    public static dynamic? Call(string name, object[] vars = null)
    {
        return myjs.Call(name, vars);
    }
    public static void Main(string exe, string[] args)
    {
        AllocConsole();
        Global.Initialize();
        Debug(IntPtr.Size, "IntPtr.Size");
        Debug(new
        {
            RuntimeVersion = Environment.Version.ToString(),
            AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            exe = exe,
            args = args,
        });
#if false
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());
#else
        Console.Write("Hit any key to exit...");
        Console.ReadKey();
#endif
    }
    public static decimal Add2(decimal a, decimal b)
    {
        Util.Log($"a={a} b={b}");
        decimal answer = a + b;
        Util.Log($"answer={answer}");
        return answer;
    }
}
public class API
{
    static API()
    {
        //Initializer.Initialize();
    }
    public static object main(MyData args)
    {
        var exe = args["exe"].Value;
        MyData mj = args["args"];
        var argsArray = mj.AsStringArray;
        Service.Main(exe, argsArray);
        return null;
    }
    public static object add2(MyData args)
    {
        var ary = args.AsDecimalArray;
        Assert.Equal(2, ary.Length);
        if (ary.Length == 0)
        {
            string error = "add2(): 0 arguments";
            throw new Exception(error);
        }
        if (ary.Length != 2)
        {
            string error = $"add2(): expects 2 arguments but {ary.Length} passed";
            throw new Exception(error);
        }
        return Service.Add2(ary[0], ary[1]);
    }
    public static object Init(MyData args)
    {
        var asmSpecList = args.AsStringArray;
        Service.Init(asmSpecList);
        return null;
    }
    public static object SetValue(MyData args)
    {
        if (args.Count != 2)
        {
            string error = $"SetValue(): expects 2 arguments but {args.Count} passed";
            throw new Exception(error);
        }
        Service.SetValue(args[0].Value, MyData.ToObject(args[1]));
        return null;
    }
    public static object GetValue(MyData args)
    {
        if (args.Count != 1)
        {
            string error = $"GetValue(): expects 1 arguments but {args.Count} passed";
            throw new Exception(error);
        }
        return Service.GetValue(args[0].Value);
    }
    public static object Execute(MyData args)
    {
        if (args.Count != 1 && args.Count != 2)
        {
            string error = $"Execute(): expects 1 or 2 arguments but {args.Count} passed";
            throw new Exception(error);
        }
        object[] vars = null;
        if (args.Count == 2) vars = args[1].AsObjectArray;
        Service.Execute(args[0].Value, vars);
        return null;
    }
    public static object Evaluate(MyData args)
    {
        if (args.Count != 1 && args.Count != 2)
        {
            string error = $"Evaluate(): expects 1 or 2 arguments but {args.Count} passed";
            throw new Exception(error);
        }
        object[] vars = null;
        if (args.Count == 2) vars = args[1].AsObjectArray;
        Service.Execute(args[0].Value, vars);
        return Service.Evaluate(args[0].Value, vars);
    }
    public static object Call(MyData args)
    {
        if (args.Count != 1 && args.Count != 2)
        {
            string error = $"Call(): expects 1 or 2 arguments but {args.Count} passed";
            throw new Exception(error);
        }
        object[] vars = null;
        if (args.Count == 2) vars = args[1].AsObjectArray;
        Service.Execute(args[0].Value, vars);
        return Service.Call(args[0].Value, vars);
    }
}
