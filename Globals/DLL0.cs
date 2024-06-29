namespace Global;
public class DLL0
{
    public static JsonClient API = null;
    static DLL0()
    {
        string dllPath = Internal.InstallResourceDll(typeof(DLL0).Assembly, "dll0");
        API = new JsonClient(dllPath);
    }
}
