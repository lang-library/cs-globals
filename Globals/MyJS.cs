#if false
using Global;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Global;

public class MyJS
{
    protected Jint.Engine engine = JintScript.CreateEngine();
    protected List<Assembly> asmList = new List<Assembly>();
    public MyJS()
    {
    }
    public void Init(string[] asmSpecList = null, Assembly[] memAsmList = null)
    {
        Console.Error.WriteLine($"[myjs] Initializing {typeof(MyJS).Assembly.Location}...");
        asmList.Clear();
        if (memAsmList != null )
        {
            foreach (var asm in memAsmList)
            {
                asmList.Add(asm);
            }
        }
        if (asmSpecList != null )
        {
            foreach (var asmSpec in asmSpecList)
            {
                var asm = LoadAssemblyForSpec(asmSpec/*, cwd*/);
                asmList.Add(asm);
            }
        }
        engine = JintScript.CreateEngine(asmList.ToArray());
    }
    protected Assembly LoadAssemblyForSpec(string asmSpec, string cwd = null)
    {
        Console.Error.WriteLine($"[myjs] //+ {asmSpec}");
        string realPath = null;
        if (cwd != null)
        {
            realPath = Util.FindExePath(asmSpec, cwd);
        }
        else
        {
            realPath = Util.FindExePath(asmSpec);
        }
        if (realPath is null)
        {
            string error = $"{asmSpec} not found";
            Console.Error.WriteLine(error);
            throw new Exception(error);
        }
        Console.Error.WriteLine($"[myjs] Loading from {realPath}...");
        var asm = Assembly.LoadFrom(realPath);
        return asm;
    }
    public void SetValue(string name, dynamic? value)
    {
        engine.Execute($"globalThis.{name}=({EasyObject.FromObject(value).ToJson()})");
    }
    public dynamic? GetValue(string name)
    {
        return engine.GetValue(name).ToObject();
    }
    public EasyObject GetValueAsEasyObject(string name)
    {
        return EasyObject.FromObject(GetValue(name));
    }
    public void Execute(string script, params object[] vars)
    {
        if (vars is null) vars = new object[] { };
        for (int i = 0; i < vars.Length; i++)
        {
            SetValue($"${i + 1}", vars[i]);
        }
        engine.Execute(script);
        for (int i = 0; i < vars.Length; i++)
        {
            engine.Execute($"delete globalThis.${i + 1};");
        }
    }
    public dynamic? Evaluate(string script, params object[] vars)
    {
        if (vars is null) vars = new object[] { };
        for (int i = 0; i < vars.Length; i++)
        {
            SetValue($"${i + 1}", vars[i]);
        }
        var result = engine.Evaluate(script).ToObject();
        for (int i = 0; i < vars.Length; i++)
        {
            engine.Execute($"delete globalThis.${i + 1};");
        }
        return result;
    }
    public EasyObject EvaluateAsEasyObject(string script, params object[] vars)
    {
        return EasyObject.FromObject(Evaluate(script, vars));
    }
    public dynamic? Call(string name, params object[] vars)
    {
        if (vars is null) vars = new object[] { };
        string script = name + "(";
        for (int i = 0; i < vars.Length; i++)
        {
            if (i > 0) script += ", ";
            script += $"${i + 1}";
        }
        script += ")";
        var result = Evaluate(script, vars);
        for (int i = 0; i < vars.Length; i++)
        {
            engine.Execute($"delete globalThis.${i + 1};");
        }
        return result;
    }
    public EasyObject CallAsEasyObject(string name, params object[] vars)
    {
        return EasyObject.FromObject(Call(name, vars));
    }
}
#endif
