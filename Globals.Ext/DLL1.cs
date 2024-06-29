namespace Global;
public class DLL1
{
    public static JsonClient API = null;
    static DLL1()
    {
        string dllPath = Internal.InstallResourceDll(typeof(DLL1).Assembly, "dll1");
        API = new JsonClient(dllPath);
    }
}
