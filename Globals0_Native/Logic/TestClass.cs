using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nuget_tools.Globals0_Native;

public class TestClass
{
    public int w;
    public int h;
    public TestClass(int w, int h)
    {
        this.w = w;
        this.h = h;
    }
    public int Area()
    {
        return this.w * this.h;
    }
}
