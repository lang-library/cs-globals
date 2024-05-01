using System;

namespace nuget_tools.Globals0_Native;

public static class APIServer
{
    private static JsonServer server = new JsonServer(typeof(API));

    public static IntPtr Call(IntPtr nameAddr, IntPtr inputAddr)
    {
        return server.HandleCall(nameAddr, inputAddr);
    }
}
