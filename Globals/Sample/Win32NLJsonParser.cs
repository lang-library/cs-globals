using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Global.EasyObject;

namespace Global.Sample;

public class Win32NLJsonParser
{
    static Win32NLJsonParser()
    {
        string dir = Internal.InstallResourceZip("NLJsonParserDLL");
        string dll = @$"{dir}\NLJsonParserDLL.dll";
        //Echo(dll, "dll");
        //Echo(File.Exists(dll));
        LoadLibraryExW(
            dll,
            IntPtr.Zero,
            LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH);
    }
    NLJsonParser parser = null;
    public Win32NLJsonParser()
    {
        this.parser = NLJsonParserDLL.CreateParser();
    }
    public NLJsonResult Parse(string json)
    {
        return this.parser.Parse(json);
    }
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
