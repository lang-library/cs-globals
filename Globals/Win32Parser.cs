using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using static Global.GObject;

namespace Global;

public class Win32Parser
{
    static Win32Parser()
    {
        string dir = Internal.InstallResourceZip("PegParserDLL");
        string dll = @$"{dir}\PegParserDLL.dll";
        LoadLibraryExW(
            dll,
            IntPtr.Zero,
            LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH);
    }

    protected PegParser parser = null;

    public Win32Parser(string grammar)
    {
        this.parser = PegParserDLL.CreateParser(grammar);
    }

    ~Win32Parser()
    {
        this.parser.Dispose();
    }

    public PegAST Parse(string input)
    {
        //IntPtr inputPtr = Util.StringToWideAddr(input);
        using (PegResult result = this.parser.Parse(input))
        {
            //Util.FreeHGlobal(inputPtr);
            if (result.error)
            {
                throw new Exception(result.error_msg);
            }
            return result.ast;
        }
    }
#if false
    public static AST Parse(string grammar, string input)
    {
        EasyObject eo = ParseToEasyObject(grammar, input);
        AST ast = EasyObjectToAST(eo);
        return ast;
    }
    protected static EasyObject ParseToEasyObject(string grammar, string input)
    {
        EasyObject eo = DLL0.API.Call("parse", new EasyObject().Add(grammar).Add(input));
        return eo;
    }
    protected static AST EasyObjectToAST(EasyObject x)
    {
        if (x.IsNull) return null;
        var ast = new AST();
        ast.name = x["name"].Cast<string>();
        if (!x["token"].IsNull)
        {
            ast.is_token = true;
            ast.name_choice = ast.name;
            ast.token = x["token"].Cast<string>();
            return ast;
        }
        else
        {
            ast.is_token = false;
            ast.choice = x["choice"].Cast<int>();
            ast.name_choice = x["name_choice"].Cast<string>();
            ast.nodes = new List<AST>();
            foreach (var child in x["nodes"].AsList)
            {
                var node = EasyObjectToAST(child);
                ast.nodes.Add(node);
            }
            return ast;
        }
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

