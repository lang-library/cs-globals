using System;
using System.Windows.Forms;
namespace Exe;
public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            nuget_tools.Globals0_Native.Service.Main(Application.ExecutablePath, args);
        }
        catch (Exception e)
        {
            //Log(e.ToString());
            //Message(e.ToString(), "Exception");
            Console.Error.WriteLine(e.ToString());
        }
    }
}