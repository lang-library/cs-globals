using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.Sample;

public class Win32NLJsonHandler : IJsonHandler
{
    Win32NLJsonParser jsonParser;
    ObjectParser objParser;
    public Win32NLJsonHandler(bool NumberAsDecimal, bool ForceASCII)
    {
        this.jsonParser = new Win32NLJsonParser(NumberAsDecimal);
        this.objParser = new ObjectParser(ForceASCII);
    }
    object IJsonHandler.Parse(string json)
    {
        return this.jsonParser.Parse(json);
    }

    string IJsonHandler.Stringify(object x, bool indent)
    {
        return this.objParser.Stringify(x, indent);
    }
}
