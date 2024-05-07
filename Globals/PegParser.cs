using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using static Global.EasyObject;

namespace Global;

public class PegParser
{
    public static AST Parse(string grammar, string input)
    {
        EasyObject eo = ParseToEasyObject(grammar, input);
        AST ast = EasyObjectToAST(eo);
        return ast;
    }
    protected static EasyObject ParseToEasyObject(string grammar, string input)
    {
        EasyObject eo = DLL0.API.Call("parse", new EasyObject().Add(grammar).Add(input));
        return eo;
    }
    protected static AST EasyObjectToAST(EasyObject x)
    {
        if (x.IsNull) return null;
        var ast = new AST();
        ast.name = x["name"].Cast<string>();
        if (!x["token"].IsNull)
        {
            ast.is_token = true;
            ast.token = x["token"].Cast<string>();
            return ast;
        }
        else
        {
            ast.is_token = false;
            ast.choice = x["choice"].Cast<int>();
            ast.name_choice = x["name_choice"].Cast<string>();
            ast.nodes = new List<AST>();
            foreach (var child in x["nodes"].AsList)
            {
                var node = EasyObjectToAST(child);
                ast.nodes.Add(node);
            }
            return ast;
        }
    }
}
