using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
namespace nuget_tools.Globals0_Native;
internal class Internal
{
    public static string InstallResourceDll(string name)
    {
        int bit = IntPtr.Size * 8;
        return Installer.InstallResourceDll(
            typeof(Internal).Assembly,
            Dirs.ProfilePath(".javacommons", "nuget_tools.Globals0_Native"),
            $"nuget_tools.Globals0_Native:{name}-x{bit}.dll"
            );

    }
    public static string InstallResourceZip(string name)
    {
        int bit = IntPtr.Size * 8;
        string dir = Installer.InstallResourceZip(
            typeof(Internal).Assembly,
            Dirs.ProfilePath(".javacommons", "nuget_tools.Globals0_Native"),
            $"nuget_tools.Globals0_Native:{name}.zip"
            );
        return Path.Combine(dir, $"x{bit}");
    }
}
