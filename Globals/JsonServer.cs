//using MyJson;
using Global;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Global;
public class JsonServer
{
    Type? apiType = null;
    public JsonServer(Type apiType)
    {
        this.apiType = apiType;
    }
    static ThreadLocal<IntPtr> HandleCallPtr = new ThreadLocal<IntPtr>();
    public IntPtr HandleCall(IntPtr nameAddr, IntPtr inputAddr)
    {
        if (HandleCallPtr.Value != IntPtr.Zero)
        {
            Util.FreeHGlobal(HandleCallPtr.Value);
            HandleCallPtr.Value = IntPtr.Zero;
        }
        var name = Util.UTF8AddrToString(nameAddr);
        //Util.Log($"Calling {name}()");
        var input = Util.UTF8AddrToString(inputAddr);
        var args = GObject.FromJson(input);
        MethodInfo mi = this.apiType!.GetMethod(name);
        GObject result = GObject.FromObject(null);
        if (mi == null)
        {
            result = GObject.FromObject($"API not found: {name}");
        }
        else
        {
            try
            {
                result = GObject.FromObject(mi.Invoke(null, new object[] { args }));
                result = GObject.FromObject(new object[] { result });
            }
            catch (TargetInvocationException ex)
            {
                result = GObject.FromObject(ex.InnerException.ToString().Replace("\r\n", "\n"));
            }
        }
        string output = result.ToJson(true);
        HandleCallPtr.Value = Util.StringToUTF8Addr(output);
        return HandleCallPtr.Value;
    }
}
