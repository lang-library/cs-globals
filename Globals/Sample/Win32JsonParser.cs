using NUnit.Framework;
using static Global.EasyObject;

namespace Global.Sample;

public class Win32JsonParser
{
    protected Win32Parser parser = null;
    public Win32JsonParser()
    {
        this.parser = new Win32Parser("""
    # JSON grammar based on RFC 4627 (http://www.ietf.org/rfc/rfc4627.txt)

    json        <- object / array / boolean / number / string / null

    object      <- '{' (member (',' member)*)? '}' { no_ast_opt }
    member      <- string ':' value

    array       <- '[' (value (',' value)*)? ']'

    value       <- boolean / null / number / string / object / array

    boolean     <- 'false' / 'true'
    null        <- 'null'

    number      <- < minus int frac exp >
    minus       <- '-'?
    int         <- '0' / [1-9][0-9]*
    frac        <- ('.' [0-9]+)?
    exp         <- ([eE] [-+]? [0-9]+)?

    string      <- '"' < char* > '"'
    char        <- unescaped / escaped
    escaped     <- '\\' (["\\/bfnrt] / 'u' [a-fA-F0-9]{4})
    unescaped   <- [\u0020-\u0021\u0023-\u005b\u005d-\u10ffff]

    %whitespace <- [ \t\r\n]*
    """);
    }
    public object Parse(string json)
    {
        PegAST ast = this.parser.Parse(json);
        using (ast) // Dispose ast
            return DoParse(ast);
    }
    object DoParse(PegAST ast)
    {
        switch (ast.name_choice)
        {
            case "json/3": // number
                {
                    Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    Assert.That(ast.nodes[0].name, Is.EqualTo("number"));
                    return DoParse(ast.nodes[0]);
                }
            case "number":
                {
                    Assert.That(ast.is_token, Is.True);
                    return decimal.Parse(ast.token);
                }
            default:
                Echo(ast);
                throw new System.Exception($"{ast.name_choice} not supported");
        }
    }
}