using System;
using System.Collections.Generic;
namespace Global;
public class ProcessRunner
{
    static ProcessRunner()
    {
    }
    public static void Initialize()
    {
        ;
    }
    public static void HandleEvents()
    {
        DLL1.API.Call("process_events", null);
    }
    public static int RunProcess(bool windowed, string exePath, string[] args, string cwd = "", Dictionary<string, string> env = null)
    {
        var result = DLL1.API.Call("run_process", EasyObject.EmptyArray.Add(windowed).Add(exePath).Add(args).Add(cwd).Add(env));
        return (int)result.Dynamic/*[0]*/;
    }
    public static bool LaunchProcess(bool windowed, string exePath, string[] args, string cwd = "", Dictionary<string, string> env = null, string fileToDelete = "")
    {
        var result = DLL1.API.Call("launch_process", EasyObject.EmptyArray.Add(windowed).Add(exePath).Add(args).Add(cwd).Add(env).Add(fileToDelete));
        return (bool)result.Dynamic/*[0]*/;
    }
    public static string ProcessOutputUtf8(bool merge, string exePath, string[] args, string cwd = "", Dictionary<string, string> env = null)
    {
        var result =  DLL1.API.Call("process_stdout_utf8", EasyObject.EmptyArray.Add(merge).Add(exePath).Add(args).Add(cwd).Add(env));
        return (string)result.Dynamic/*[0]*/;
    }
    public static string ProcessOutputLocal8Bit(bool merge, string exePath, string[] args, string cwd = "", Dictionary<string, string> env = null)
    {
        var result = DLL1.API.Call("process_stdout_local8bit", EasyObject.EmptyArray.Add(merge).Add(exePath).Add(args).Add(cwd).Add(env));
        return (string)result.Dynamic/*[0]*/;
    }
    public static byte[] ProcessOutputBytes(bool merge, string exePath, string[] args, string cwd = "", Dictionary<string, string> env = null)
    {
        var result = DLL1.API.Call("process_stdout_base64", EasyObject.EmptyArray.Add(merge).Add(exePath).Add(args).Add(cwd).Add(env));
        string base64 = (string)result.Dynamic/*[0]*/;
        byte[] bytes = Convert.FromBase64String(base64);
        return bytes;
    }
}
