using Globals;
using MyJson;
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Threading;  // Marshalを使うのに必要

namespace ClassLibrary1
{
    public class Class1
    {
        static ThreadLocal<IntPtr> resultPtr = new ThreadLocal<IntPtr>();
        [DllExport]
        public static IntPtr Call(IntPtr name_, IntPtr args_)
        {
            string name = Util.UTF8AddrToString(name_);
            string args = Util.UTF8AddrToString(args_);
            MyData.Echo(name, "name");
            MyData.Echo(args, "args");
            string result = "[33]";
            resultPtr.Value = Util.StringToUTF8Addr(result);
            return resultPtr.Value;
        }
    }
}
