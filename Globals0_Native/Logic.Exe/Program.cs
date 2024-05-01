using System;
using static Globals.Util;
using System.Windows.Forms;
using Globals;
using MyJson;
namespace Exe;
public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            string _appFile = Application.ExecutablePath;
            string _args = MyData.ToJson(MyData.FromObject(new { exe = _appFile, args = new string[] { } }), true);
            IntPtr ret = global::nuget_tools.Globals0_Native.APIServer.Call(Util.StringToUTF8Addr("main"), Util.StringToUTF8Addr(_args));
            string error = Util.UTF8AddrToString(ret);
            if (error != "") throw new Exception(error);
        }
        catch (Exception e)
        {
            Log(e.ToString());
            Message(e.ToString(), "Exception");
        }
    }
}