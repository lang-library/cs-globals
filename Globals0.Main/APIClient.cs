using System;

namespace nuget_tools.Globals0_Native;

public static class APIClient
{
    static APIClient()
    {
        Initializer.Initialize();
    }
    [DllExport]
    [STAThread]
    public static IntPtr Call(IntPtr nameAddr, IntPtr inputAddr)
    {
        return APIServer.Call(nameAddr, inputAddr);
    }
}
