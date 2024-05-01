global using System.Linq;
global using Globals;
global using static Globals.Util;
using System.Net;

namespace nuget_tools.Globals0_Native;

internal static class Global
{
    static Global()
    {
        DebugFlag = true;
        Debug("Initializing Global.");
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
    }
    public static void Initialize()
    {
        ;
    }
}
