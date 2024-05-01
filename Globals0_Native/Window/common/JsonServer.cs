using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace nuget_tools.Globals0_Native;
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
        var input = Util.UTF8AddrToString(inputAddr);
        var args = Util.FromJson(input);
        MethodInfo mi = this.apiType!.GetMethod(name);
        dynamic result = null;
        if (mi == null)
        {
            result = $"API not found: {name}";
        }
        else
        {
            try
            {
                result = mi.Invoke(null, new object[] { args });
                result = new object[] { result };
            }
            catch (TargetInvocationException ex)
            {
                result = ex.InnerException.ToString().Replace("\r\n", "\n");
            }
        }
        string output = Util.ToJson(result);
        HandleCallPtr.Value = Util.StringToUTF8Addr(output);
        return HandleCallPtr.Value;
    }
}
