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
    public object Parse(string json)
    {
        return this.jsonParser.Parse(json);
    }

    public string Stringify(object x, bool indent)
    {
        return this.objParser.Stringify(x, indent);
    }
}
