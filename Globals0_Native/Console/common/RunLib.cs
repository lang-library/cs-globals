using System;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace nuget_tools.Globals0_Native;

internal class RunLib
{
    public static void EntryPoint(string exe, string[] originalArgs)
    {
        string dllName = null;
        string[] args = null;
        string embedded = TextEmbedder.TextEmbed(exe);
        if (embedded != null)
        {
            dllName = embedded;
            //Console.Error.WriteLine($"{exe} has embedded path ==> {dllName}");
            args = originalArgs;
        }
        else
        {
            if (originalArgs.Length < 1)
            {
                Console.Error.WriteLine("Please specify dll name.");
                Environment.Exit(1);
            }
            dllName = originalArgs[0];
            ArraySegment<string> arySeg = new ArraySegment<string>(originalArgs, 1, originalArgs.Length - 1);
            args = arySeg.ToArray();
        }
        if (!dllName.ToUpper().EndsWith(".DLL"))
        {
            dllName = FindDllFromNugetTools(dllName);
        }
        JsonClient api = new JsonClient(dllName, Directory.GetCurrentDirectory());
        api.Call("main", new { exe = exe, args = args });
        Environment.Exit(0);
    }
    public static string FindDllFromNugetTools(string appName)
    {
        string userName = "nuget-tools";
        string[] appNameParts = appName.Split('.');
        if (appNameParts.Length == 1)
        {
        }
        else if (appNameParts.Length == 2)
        {
            userName = appNameParts[0];
            appName = appNameParts[1];
        }
        else
        {
            Console.Error.WriteLine("Invalid program name.");
            Environment.Exit(1);
        }
        string xmlUrl = $"https://gitlab.com/{userName}/tools/-/raw/main/{appName}.xml";
        var xml = Util.GetStringFromUrl(xmlUrl);
        XDocument doc = XDocument.Parse(xml);
        XElement root = doc.Root;
        var version = root.Element("version").Value;
        var url = root.Element("url").Value;
        //Console.Error.WriteLine(version);
        //Console.Error.WriteLine(url);
        var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //Console.Error.WriteLine(profilePath);
        var installPath = $"{profilePath}\\.javacommons\\.software\\{userName}\\{appName}-{version}";
        //Console.Error.WriteLine(installPath);
        if (!Directory.Exists(installPath))
        {
            //Console.Error.WriteLine($"{installPath} �����݂��܂���");
            DirectoryInfo di = new DirectoryInfo(installPath);
            DirectoryInfo diParent = di.Parent;
            string parent = diParent.FullName;
            //Console.Error.WriteLine($"{parent} ��������܂�");
            Directory.CreateDirectory(parent);
            string destinationPath = $"{parent}\\{appName}-{version}.zip";
            FileInfo fi = new FileInfo(destinationPath);
            if (!fi.Exists)
            {
                //Console.Error.WriteLine($"{destinationPath} �Ƀ_�E�����[�h���܂�");
                Util.DownloadBinaryFromUrl(url, destinationPath);
                //Console.Error.WriteLine($"{destinationPath} �Ƀ_�E�����[�h���������܂���");
            }
            //Console.Error.WriteLine($"{installPath} �ɓW�J���܂�");
            ZipFile.ExtractToDirectory(destinationPath, installPath);
            //Console.Error.WriteLine($"{installPath} �ɓW�J���܂���");
        }
        int bit = IntPtr.Size * 8;
        string mainDllPath = Path.Combine(installPath, $"{userName}.{appName}.native.dll");
        if (!File.Exists(mainDllPath))
        {
            //Console.Error.WriteLine($"{mainDllPath} ���݂���܂���");
            Console.Error.WriteLine($"{mainDllPath} not found");
            Environment.Exit(1);
        }
        return mainDllPath;
    }
}
