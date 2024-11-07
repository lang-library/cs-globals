//using NUnit.Framework;
using System.Text;
using System;
using static Global.EasyObject;
using System.Collections.Generic;

namespace Global.Sample;

public class Win32JsonParser
{
    protected Win32Parser parser = null;
    public Win32JsonParser()
    {
        this.parser = new Win32Parser("""
    # JSON grammar based on RFC 4627 (http://www.ietf.org/rfc/rfc4627.txt)

    json        <- boolean / null / number / string / object / array
    

    object      <- '{' (member (',' member)*)? '}' { no_ast_opt }
    member      <- string ':' json

    array       <- '[' (json (',' json)*)? ']'

    boolean     <- 'false' / 'true'
    null        <- 'null'

    number      <- < minus int frac exp >
    minus       <- '-'?
    int         <- '0' / [1-9][0-9]*
    frac        <- ('.' [0-9]+)?
    exp         <- ([eE] [-+]? [0-9]+)?

    #string      <- '"' < char* > '"'
    string      <- < '"' char* '"' >
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
            case "json/0": // booolean
                {
                    //Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    //Assert.That(ast.nodes[0].name, Is.EqualTo("boolean"));
                    return DoParse(ast.nodes[0]);
                }
            case "json/1": // null
                {
                    //Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    //Assert.That(ast.nodes[0].name, Is.EqualTo("null"));
                    return null;
                }
            case "json/2": // number
                {
                    //Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    //Assert.That(ast.nodes[0].name, Is.EqualTo("number"));
                    return DoParse(ast.nodes[0]);
                }
            case "json/3": // string
                {
                    //Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    //Assert.That(ast.nodes[0].name, Is.EqualTo("string"));
                    return DoParse(ast.nodes[0]);
                }
            case "json/4": // object
                {
                    //Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    //Assert.That(ast.nodes[0].name, Is.EqualTo("object"));
                    return DoParse(ast.nodes[0]);
                }
            case "json/5": // array
                {
                    //Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    //Assert.That(ast.nodes[0].name, Is.EqualTo("array"));
                    return DoParse(ast.nodes[0]);
                }
            case "array/0":
                {
                    //Assert.That(ast.is_token, Is.False);
                    var result = new List<object>();
                    foreach (var node in ast.nodes)
                    {
                        result.Add(DoParse(node));
                    }
                    return result;
                }
            case "object/0":
                {
                    //Assert.That(ast.is_token, Is.False);
                    var result = new Dictionary<string, object>();
                    foreach (var node in ast.nodes)
                    {
                        //Echo(node, "node");
                        KeyValuePair<string, object> pair = (KeyValuePair<string, object>)DoParse(node);
                        //Echo(pair);
                        //result.Add(DoParse(node));
                        result.Add(pair.Key, pair.Value);
                    }
                    return result;
                }
            case "member/0":
                {
                    //Assert.That(ast.is_token, Is.False);
                    //Assert.That(ast.nodes.Count, Is.EqualTo(2));
                    return new KeyValuePair<string, object>(
                        (string)DoParse(ast.nodes[0]),
                        DoParse(ast.nodes[1]));
                }
            case "boolean":
                {
                    //Assert.That(ast.is_token, Is.True);
                    return (ast.token == "true") ? true : false;
                }
            case "number":
                {
                    //Assert.That(ast.is_token, Is.True);
                    return decimal.Parse(ast.token);
                }
            case "string":
                {
                    //Assert.That(ast.is_token, Is.True);
                    return ParseJsonString(ast.token);
                }
            default:
                Echo(ast);
                throw new System.Exception($"{ast.name_choice} not supported");
        }
    }
    public static string ParseJsonString(string aJSON)
    {
        int i = 0;
        StringBuilder Token = new StringBuilder();
        bool QuoteMode = false;
        while (i < aJSON.Length)
        {
            switch (aJSON[i])
            {

                case '"':
                    QuoteMode ^= true;
                    break;

                case '\r':
                case '\n':
                    break;

                case ' ':
                case '\t':
                    if (QuoteMode)
                        Token.Append(aJSON[i]);
                    break;

                case '\\':
                    ++i;
                    if (QuoteMode)
                    {
                        char C = aJSON[i];
                        switch (C)
                        {
                            case 't':
                                Token.Append('\t');
                                break;
                            case 'r':
                                Token.Append('\r');
                                break;
                            case 'n':
                                Token.Append('\n');
                                break;
                            case 'b':
                                Token.Append('\b');
                                break;
                            case 'f':
                                Token.Append('\f');
                                break;
                            case 'u':
                                {
                                    string s = aJSON.Substring(i + 1, 4);
                                    Token.Append((char)int.Parse(
                                        s,
                                        System.Globalization.NumberStyles.AllowHexSpecifier));
                                    i += 4;
                                    break;
                                }
                            default:
                                Token.Append(C);
                                break;
                        }
                    }
                    break;

                case '\uFEFF': // remove / ignore BOM (Byte Order Mark)
                    break;

                default:
                    Token.Append(aJSON[i]);
                    break;
            }
            ++i;
        }
        if (QuoteMode)
        {
            throw new Exception("ParseJsonString(): Quotation marks seems to be messed up.");
        }
        return Token.ToString();
    }
}