using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Globals;
public class DLL1
{
    public static Globals.JsonAPI API = null;
    static DLL1()
    {
        string dllPath = Internal.InstallResourceDll("dll1");
        API = new Globals.JsonAPI(dllPath);
    }
}
