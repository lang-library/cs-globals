using System.Reflection;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading;
using System.Net.Sockets;
using System.CodeDom;
using Global;
//using MyJson;

namespace Global;
public class Util
{
    public static bool DebugFlag = false;
    static Util()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
    }
#if false
    public static void AllocConsole()
    {
        WinConsole.Initialize();
    }
    public static void FreeConsole()
    {
        WinConsole.Deinitialize();
    }
    public static void ReallocConsole()
    {
        FreeConsole();
        AllocConsole();
    }
#endif
    public static string FindExePath(string exe)
    {
        string cwd = "";
        return FindExePath(exe, cwd);
    }
    public static string FindExePath(string exe, string cwd)
    {
        exe = Environment.ExpandEnvironmentVariables(exe);
        if (Path.IsPathRooted(exe))
        {
            if (!File.Exists(exe)) return null;
            return Path.GetFullPath(exe);
        }
        var PATH = Environment.GetEnvironmentVariable("PATH") ?? "";
        PATH = $"{cwd};{PATH}";
        foreach (string test in PATH.Split(';'))
        {
            string path = test.Trim();
            if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                return Path.GetFullPath(path);
        }
        return null;
    }
    public static string FindExePath(string exe, Assembly assembly)
    {
        int bit = IntPtr.Size * 8;
        string cwd = AssemblyDirectory(assembly);
        string result = FindExePath(exe, cwd);
        if (result == null)
        {
            result = FindExePath(exe, $"{cwd}\\{bit}bit");
            if (result == null)
            {
                cwd = Path.Combine(cwd, "assets");
                result = FindExePath(exe, $"{cwd}\\{bit}bit");
            }
        }
        return result;
    }
    public static string AssemblyDirectory(Assembly assembly)
    {
        string codeBase = assembly.CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
    }
    public static string GuidString()
    {
        return Guid.NewGuid().ToString("D");
    }
    public static uint GetACP()
    {
        return NativeMethods.GetACP();
    }
    public static uint SessionId()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return NativeMethods.WTSGetActiveConsoleSessionId();
        }
        return 0;
    }
    public static string[] TextToLines(string text)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(text))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }
        return lines.ToArray();
    }
    public static string RandomString(Random r, string[] chars, int length)
    {
        if (chars.Length == 0 || length < 0)
        {
            throw new ArgumentException();
        }
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            int idx = r.Next(0, chars.Length);
            sb.Append(chars[idx]);
        }
        return sb.ToString();
    }
    public static string[] ExpandWildcard(string path)
    {
        string? dir = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(dir)) dir = ".";
        string fname = Path.GetFileName(path);
        string[] files = Directory.GetFiles(dir, fname);
        List<string> result = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            result.Add(Path.GetFullPath(files[i]));
        }
        return result.ToArray();
    }
    public static string[] ExpandWildcardList(params string[] pathList)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < pathList.Length; i++)
        {
            //Util.Print(pathList[i], "pathList[i]");
            string[] files = ExpandWildcard(pathList[i]);
            result.AddRange(files.ToList());
        }
        return result.ToArray();
    }
    public static void CheckNetworkAvailability(string url)
    {
        if (DebugFlag)
        {
            string no_network_var = Environment.GetEnvironmentVariable("NO_NETWORK") ?? "0";
            int no_network;
            int.TryParse(no_network_var, out no_network);
            if (no_network != 0)
            {
                throw new WebException($"Could not access {url} (NO_NETWORK is set)");
            }
        }
    }
    public static string GetStringFromUrl(string url)
    {
        CheckNetworkAvailability(url);
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        WebHeaderCollection header = response.Headers;
        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
    public static void DownloadBinaryFromUrl(string url, string destinationPath)
    {
        CheckNetworkAvailability(url);
        Dirs.PrepareForFile(destinationPath);
        WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
        var objResponse = objRequest.GetResponse();
        byte[] buffer = new byte[32768];
        using (Stream input = objResponse.GetResponseStream())
        {
            using (FileStream output = new FileStream(destinationPath, FileMode.CreateNew))
            {
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
    public static List<string> GetMacAddressList()
    {
        var list = NetworkInterface
                   .GetAllNetworkInterfaces()
                   .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                   .Select(nic => String.Join("-", SplitStringByLengthList(nic.GetPhysicalAddress().ToString().ToLower(), 2)))
                   .ToList();
        return list;
    }
    public static IEnumerable<string> SplitStringByLengthLazy(string str, int maxLength)
    {
        for (int index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }
    public static List<string> SplitStringByLengthList(string str, int maxLength)
    {
        return SplitStringByLengthLazy(str, maxLength).ToList();
    }
    public static byte[] ReadFileHeadBytes(string path, int maxSize)
    {
        System.IO.FileStream fs = new System.IO.FileStream(
            path,
            System.IO.FileMode.Open,
            System.IO.FileAccess.Read);
        byte[] array = new byte[maxSize];
        int size = fs.Read(array, 0, array.Length);
        fs.Close();
        byte[] result = new byte[size];
        Array.Copy(array, 0, result, 0, result.Length);
        return result;
    }
    public static bool IsBinaryFile(string path)
    {
        byte[] head = ReadFileHeadBytes(path, 8000);
        for (int i = 0; i < head.Length; i++)
        {
            if (head[i] == 0) return true;
        }
        return false;
    }
    public static bool LaunchProcess(string exePath, string[] args, Dictionary<string, string>? vars = null)
    {
        //ProcessMutex.WaitOne();
        string argList = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0) argList += " ";
            if (args[i].Contains(" "))
                argList += $"\"{args[i]}\"";
            else
                argList += args[i];
        }
        Process process = new Process();
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = argList;
        if (vars != null)
        {
            foreach (string key in vars.Keys)
            {
                process.StartInfo.EnvironmentVariables[key] = vars[key];
            }
        }
        bool result = process.Start();
        //ProcessMutex.ReleaseMutex();
        return result;
    }
    public static void Beep()
    {
        SystemSounds.Beep.Play();
    }
    public static void FreeHGlobal(IntPtr x)
    {
        Marshal.FreeHGlobal(x);
    }
    public static IntPtr StringToWideAddr(string s)
    {
        return Marshal.StringToHGlobalUni(s);
    }
    public static string WideAddrToString(IntPtr s)
    {
        return Marshal.PtrToStringUni(s);
    }
    public static IntPtr StringToUTF8Addr(string s)
    {
        int len = Encoding.UTF8.GetByteCount(s);
        byte[] buffer = new byte[len + 1];
        Encoding.UTF8.GetBytes(s, 0, s.Length, buffer, 0);
        IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
        Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
        return nativeUtf8;
    }
    public static string UTF8AddrToString(IntPtr s)
    {
        int len = 0;
        while (Marshal.ReadByte(s, len) != 0) ++len;
        byte[] buffer = new byte[len];
        Marshal.Copy(s, buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }
    public static string DateTimeString(DateTime x)
    {
        return x.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
    }
    public static int RunToConsole(string exePath, string[] args, Dictionary<string, string>? vars = null)
    {
        //ProcessMutex.WaitOne();
        string argList = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0) argList += " ";
            if (args[i].Contains(" "))
                argList += $"\"{args[i]}\"";
            else
                argList += args[i];
        }
        Process process = new Process();
        //ProcessMutex.ReleaseMutex();
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = argList;
        if (vars != null)
        {
            var keys = vars.Keys;
            foreach (var key in keys)
            {
                process.StartInfo.EnvironmentVariables[key] = vars[key];
            }
        }
        process.OutputDataReceived += (sender, e) => { Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { Console.Error.WriteLine(e.Data); };
        process.Start();
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { process.Kill(); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        process.CancelOutputRead();
        process.CancelErrorRead();
        return process.ExitCode;
    }
    /**
     * <summary> Sleeps for specified milliseconds (指定されたミリ秒間スリープする)
     * </summary>
     * <description>
     * @code
     * using System;
     * using Global;
     * Util.Print(DateTime.Now, "begin");
     * Util.Sleep(3000); // sleeps for 3 seconds
     * Util.Print(DateTime.Now, "end");
     * @endcode
     * @code
     * begin: 2023-11-05T21:30:41.8610034+09:00
     * end: 2023-11-05T21:30:45.1998930+09:00
     * @endcode
     * </description>
     * @param[in] milliseconds milliseconds (ミリ秒)
     */
    public static void Sleep(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }
    public static string AssemblyName(Assembly assembly)
    {
        return System.Reflection.AssemblyName.GetAssemblyName(assembly.Location).Name;
    }
    public static int FreeTcpPort()
    {
        // https://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }
    public static string FullName(dynamic x)
    {
        if (x is null) return "null";
        string fullName = ((object)x).GetType().FullName;
        return fullName.Split('`')[0];
    }
    //public static string ToJson(object x, bool indent = false, bool display = false)
#if false
    public static string ToJson(object x, bool indent = false)
    {
        if (x is MyData)
        {
            return MyData.ToJson(((MyData)x), indent);
        }
        //var myJson = MyJson.FromObject(x, display);
        var myJson = MyData.FromObject(x);
        return MyData.ToJson(myJson, indent);
    }
#endif
#if false
    protected static string ReformatJson(string json, bool indent = false)
    {
        MyData node = MyData.FromJson(json);
        return MyData.ToJson(node, indent);
    }
#endif
#if false
    public static dynamic FromJson(string json)
    {
        var myJson = MyData.FromJson(json);
        return MyData.ToObject(myJson);
    }
    public static MyData AsMyJson(object x)
    {
        if (x is MyData) return ((MyData)x).Clone();
        return MyData.FromObject(x);
    }
    public static string ToString(dynamic x)
    {
        if (x is null) return "null";
        if (x is string)
        {
            return "\"" + (string)x + "\"";
        }
        string output = null;
        if (x is MyString)
        {
            var value = (MyString)x;
            output = "\"" + value.Value + "\"";
            //output = "'" + value.Value + "'";
        }
        else if (x is MyData)
        {
            var value = (MyData)x;
            output = MyData.ToJson(value, 2);
        }
        else if (x is System.DateTime)
        {
            output = Util.DateTimeString(x); //x.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }
        else
        {
            try
            {
                //output = ToJson(x, true, true);
                output = ToJson(x, true);
            }
            catch (Exception)
            {
                output = x.ToString();
            }
        }
        return $"<{Util.FullName(x)}> {output}";
    }
#endif
#if false
    public static void Echo(object x, string title = null)
    {
        new MyTool()
            .SetDebugOutput(DebugFlag)
            .Echo(x, title);
    }
    public static void Log(dynamic x, string? title = null)
    {
        new MyTool()
            .SetDebugOutput(DebugFlag)
            .Log(x, title);
    }
    public static void Debug(dynamic x, string? title = null)
    {
        new MyTool()
            .SetDebugOutput(DebugFlag)
            .Debug(x, title);
    }
#endif
    public static string[] ResourceNames(Assembly assembly)
    {
        return assembly.GetManifestResourceNames();
    }
    public static Stream? ResourceAsStream(Assembly assembly, string name)
    {
        string resourceName = name.Contains(":") ? name.Replace(":", ".") : $"{AssemblyName(assembly)}.{name}";
        Stream? stream = assembly.GetManifestResourceStream(resourceName);
        return stream;
    }
    public static string StreamAsText(Stream stream)
    {
        if (stream is null) return null; // "";
        long pos = stream.Position;
        var streamReader = new StreamReader(stream);
        var text = streamReader.ReadToEnd();
        stream.Position = pos;
        return text;
    }
    public static string ResourceAsText(Assembly assembly, string name)
    {
        string resourceName = name.Contains(":") ? name.Replace(":", ".") : $"{AssemblyName(assembly)}.{name}";
        Stream stream = assembly.GetManifestResourceStream(resourceName);
        return StreamAsText(stream);
    }
    public static byte[] StreamAsBytes(Stream stream)
    {
        if (stream is null) return null;
        long pos = stream.Position;
        byte[] bytes = new byte[(int)stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
        stream.Position = pos;
        return bytes;
    }
    public static byte[] ResourceAsBytes(Assembly assembly, string name)
    {
        string resourceName = name.Contains(":") ? name.Replace(":", ".") : $"{AssemblyName(assembly)}.{name}";
        Stream stream = assembly.GetManifestResourceStream(resourceName);
        return StreamAsBytes(stream);
    }
    public static EasyObject StreamAsJson(Stream stream)
    {
        string json = StreamAsText(stream);
        return EasyObject.FromJson(json);
    }
    public static EasyObject ResourceAsMyJson(Assembly assembly, string name)
    {
        string json = ResourceAsText(assembly, name);
        return EasyObject.FromJson(json);
    }
    public static byte[]? ToUtf8Bytes(string? s)
    {
        if (s is null) return null;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
        return bytes;
    }
#if false
    public static void Message(dynamic x, string? title = null)
    {
        if (title is null) title = "Message";
        if ((x as string) != null)
        {
            var s = (string)x;
            System.Diagnostics.Debug.WriteLine(s);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                NativeMethods.MessageBoxW(IntPtr.Zero, s, title, 0);
            }
            else
            {
                Util.Log(s, title);
            }
            return;
        }
        {
            var s = Util.ToString(x);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                NativeMethods.MessageBoxW(IntPtr.Zero, s, title, 0);
            }
            else
            {
                Util.Log(s, title);
            }
        }
    }
#endif
    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int MessageBoxW(
            IntPtr hWnd, string lpText, string lpCaption, uint uType);
        [DllImport("kernel32.dll")]
        internal static extern uint WTSGetActiveConsoleSessionId();
        [DllImport("kernel32.dll")]
        internal static extern uint GetACP();
    }
}