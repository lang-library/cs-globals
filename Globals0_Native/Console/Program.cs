using System;
using static nuget_tools.Globals0_Native.Util;
using System.Windows.Forms;
namespace nuget_tools.Globals0_Native;
public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            DebugFlag = true;
            RunLib.EntryPoint(Application.ExecutablePath, args);
        }
        catch (Exception e)
        {
            Log(e.ToString());
            Message(e.ToString(), "Exception");
        }
    }
}