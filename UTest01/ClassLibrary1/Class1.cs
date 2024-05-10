using Global;
using static Global.GObject;
using Global;
//using MyJson;
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Threading;  // Marshalを使うのに必要

namespace ClassLibrary1;

public static class API
{
    public static GObject add2(GObject args)
    {
        return GObject.FromObject(args[0].Cast<double>() + args[1].Cast<double>());
    }
}

public class Class1
{
    static JsonServer server = new JsonServer(typeof(API));
    static ThreadLocal<IntPtr> resultPtr = new ThreadLocal<IntPtr>();
    [DllExport]
    public static IntPtr Call(IntPtr name_, IntPtr args_)
    {
#if false
        string name = Util.UTF8AddrToString(name_);
        string args = Util.UTF8AddrToString(args_);
        EasyObject.Echo(name, "name");
        EasyObject.Echo(args, "args");
        string result = "[33]";
        resultPtr.Value = Util.StringToUTF8Addr(result);
        return resultPtr.Value;
#else
        return server.HandleCall(name_, args_);
#endif
    }
}
