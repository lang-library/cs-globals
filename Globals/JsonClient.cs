//using MyJson;
using Global;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Global;
public class JsonClient
{
#if false
    static Dictionary<int, JsonClient> apiMap = new Dictionary<int, JsonClient>();
#endif
    IntPtr Handle = IntPtr.Zero;
    IntPtr CallPtr = IntPtr.Zero;
    delegate IntPtr proto_Call(IntPtr name, IntPtr args);
    delegate IntPtr proto_LastError();
    public JsonClient(string dllSpec)
    {
        string dllPath = Util.FindExePath(dllSpec);
        if (dllPath is null)
        {
            GObject.Log(dllSpec, "dllSpec");
            GObject.Log(dllPath, "dllPath");
            Environment.Exit(1);
        }
        this.LoadDll(dllPath);
    }
    public JsonClient(string dllSpec, string cwd)
    {
        string dllPath = Util.FindExePath(dllSpec, cwd);
        if (dllPath is null)
        {
            GObject.Log(dllSpec, "dllSpec");
            GObject.Log(dllPath, "dllPath");
            Environment.Exit(1);
        }
        this.LoadDll(dllPath);
    }
    public JsonClient(string dllSpec, Assembly assembly)
    {
        string dllPath = Util.FindExePath(dllSpec, assembly);
        if (dllPath is null)
        {
            GObject.Log(dllSpec, "dllSpec");
            GObject.Log(dllPath, "dllPath");
            Environment.Exit(1);
        }
        this.LoadDll(dllPath);
    }
    private void LoadDll(string dllPath)
    {
        this.Handle = LoadLibraryExW(
            dllPath,
            IntPtr.Zero,
            LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH
            );
        if (this.Handle == IntPtr.Zero)
        {
            GObject.Log($"DLL not loaded: {dllPath}");
            Environment.Exit(1);
        }
        this.CallPtr = GetProcAddress(Handle, "Call");
        if (this.CallPtr == IntPtr.Zero)
        {
            GObject.Log("Call() not found");
            Environment.Exit(1);
        }
    }
    public GObject Call(dynamic name, GObject args)
    {
        IntPtr pName = Util.StringToUTF8Addr(name);
        proto_Call pCall = (proto_Call)Marshal.GetDelegateForFunctionPointer(this.CallPtr, typeof(proto_Call));
        var argsJson = args.ToJson();
        IntPtr pArgsJson = Util.StringToUTF8Addr(argsJson);
        IntPtr pResult = pCall(pName, pArgsJson);
        string result = Util.UTF8AddrToString(pResult);
        result = result.Trim();
        Marshal.FreeHGlobal(pName);
        Marshal.FreeHGlobal(pArgsJson);
        if (result.StartsWith("\""))
        {
            string error = GObject.FromJson(result).Cast<string>();
            throw new Exception(error);
        }
        else if (result.StartsWith("["))
        {
            var list = GObject.FromJson(result);
            if (list.Count == 0) return GObject.FromObject(null);
            return list[0];
        }
        else
        {
            string error = $"Malformed result json: {result}";
            throw new Exception(error);
        }
    }
#if false
    public dynamic Call(dynamic name, dynamic args)
    {
        var result = Util.FromJson(CallAsJson(name, args));
        return result;
    }
    public MyData CallAsMyJson(dynamic name, dynamic args)
    {
        var result = Call(name, args);
        return Util.AsMyJson(result);
    }
#endif
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr LoadLibraryW(string lpFileName);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibraryExW(string dllToLoad, IntPtr hFile, LoadLibraryFlags flags);
    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    [System.Flags]
    public enum LoadLibraryFlags : uint
    {
        DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
        LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
        LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
        LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
        LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
        LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000
    }
}
