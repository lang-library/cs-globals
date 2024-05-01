using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace nuget_tools.Globals0_Native;

static class Initializer
{
    public static string AssemblyDirectory
    {
        get
        {
            string codeBase = typeof(Initializer).Assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
    public static string AssemblyBaseDirectory = AssemblyDirectory;
    static Initializer()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            string fileName = new AssemblyName(args.Name).Name + ".dll";
            string assemblyPath = Path.Combine(AssemblyBaseDirectory, fileName);
            assemblyPath = assemblyPath.Replace("/", "\\");
            if (File.Exists(assemblyPath))
            {
                //Console.Error.WriteLine($"Loading: {assemblyPath}");
                return Assembly.LoadFile(assemblyPath);
            }
#if false
            string ownerName = GetOwnerName();
            string appName = GetAppName();
            if (fileName == $"{ownerName}.{appName}.runtime.dll")
            {
                try
                {
                    string profDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    var dllBytes = _ResourceAsBytes(typeof(Initializer).Assembly, $"Main:{ownerName}.{appName}.runtime.dll");
                    SHA256 crypto = new SHA256CryptoServiceProvider();
                    byte[] hashValue = crypto.ComputeHash(dllBytes);
                    string name = $"{ownerName}.{appName}.runtime";
                    string sha256 = String.Join("", hashValue.Select(x => x.ToString("x2")).ToArray());
                    string dllName = $"{name}-{sha256}.dll";
                    string dllDir = Path.Combine(profDir, ".javacommons/json-api");
                    string dllPath = Path.Combine(dllDir, dllName);
                    if (!File.Exists(dllPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dllPath) !);
                        string guid = Guid.NewGuid().ToString("D");
                        string tempPath = $"{dllPath}.{guid}";
                        File.WriteAllBytes(tempPath, dllBytes);
                        try
                        {
                            File.Move(tempPath, dllPath);
                        }
                        catch(Exception /*ex*/)
                        {
                        }
                    }
                    dllPath = dllPath.Replace("/", "\\");
                    //Console.Error.WriteLine($"Loading: {dllPath}");
                    return Assembly.LoadFile(dllPath);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                    return null;
                }
            }
            else
            {
                return null;
            }
#else
			return null;
#endif            
        };
    }
    public static string GetOwnerName()
    {
        return _ResourceAsText(typeof(Initializer).Assembly, "Main:OWNER.txt");
    }
    public static string GetAppName()
    {
        return _ResourceAsText(typeof(Initializer).Assembly, "Main:NAME.txt");
    }
    public static string _AssemblyName(Assembly assembly)
    {
        return System.Reflection.AssemblyName.GetAssemblyName(assembly.Location).Name;
    }
    public static void _DumpResourceNames(Assembly assembly)
    {
        string[] names = assembly.GetManifestResourceNames();
        foreach (string name in names)
        {
            Console.Error.WriteLine(name);
        }
    }
    private static byte[] _StreamAsBytes(Stream stream)
    {
        if (stream == null)
        {
            return null;
        }
        long position = stream.Position;
        byte[] array = new byte[(int)stream.Length];
        stream.Read(array, 0, (int)stream.Length);
        stream.Position = position;
        return array;
    }
    private static string _StreamAsText(Stream stream)
    {
        if (stream == null)
        {
            return null;
        }
        long position = stream.Position;
        string result = new StreamReader(stream).ReadToEnd();
        stream.Position = position;
        return result;
    }
    private static byte[] _ResourceAsBytes(Assembly assembly, string name)
    {
        string name2 = (name.Contains(":") ? name.Replace(":", ".") : (_AssemblyName(assembly) + "." + name));
        return _StreamAsBytes(assembly.GetManifestResourceStream(name2));
    }
    private static string _ResourceAsText(Assembly assembly, string name)
    {
        string name2 = (name.Contains(":") ? name.Replace(":", ".") : (_AssemblyName(assembly) + "." + name));
        return _StreamAsText(assembly.GetManifestResourceStream(name2));
    }
    public static void Initialize()
    {
        ;
    }
}
