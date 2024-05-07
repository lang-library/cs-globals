using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global;

public class AST: RedundantObject
{
    public int? choice = null;
    public string name = null;
    public string name_choice = null;
    public List<AST> nodes = null;
}
