using NUnit.Framework;
using static Global.EasyObject;

namespace Global.Sample;

public class IntCalculator
{
    protected string grammar;
    public IntCalculator(/*string grammar*/)
    {
        //this.grammar = grammar;
        this.grammar = """
    # Grammar for Calculator...
    Additive    <- Multiplicative '+' Additive / Multiplicative
    Multiplicative   <- Primary '*' Multiplicative / Primary
    Primary     <- '(' Additive ')' / Number
    Number      <- < [0-9]+ >
    %whitespace <- [ \t]*
    """;
    }
    public int Calculate(string input)
    {
        AST ast = PegParser.Parse(this.grammar, input);
        return DoCalculate(ast);
    }
    int DoCalculate(AST ast)
    {
        switch(ast.name_choice)
        {
            case "Additive/0":
                {
                    Assert.That(ast.nodes.Count, Is.EqualTo(2));
                    Assert.That(ast.nodes[0].name, Is.EqualTo("Multiplicative"));
                    Assert.That(ast.nodes[1].name, Is.EqualTo("Additive"));
                    return DoCalculate(ast.nodes[0]) + DoCalculate(ast.nodes[1]);
                }
            case "Additive/1":
                {
                    Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    Assert.That(ast.nodes[0].name, Is.EqualTo("Multiplicative"));
                    return DoCalculate(ast.nodes[0]);
                }
            case "Multiplicative/0":
                {
                    Assert.That(ast.nodes.Count, Is.EqualTo(2));
                    Assert.That(ast.nodes[0].name, Is.EqualTo("Primary"));
                    Assert.That(ast.nodes[1].name, Is.EqualTo("Multiplicative"));
                    return DoCalculate(ast.nodes[0]) * DoCalculate(ast.nodes[1]);
                }
            case "Multiplicative/1":
                {
                    Assert.That(ast.nodes.Count, Is.EqualTo(1));
                    Assert.That(ast.nodes[0].name, Is.EqualTo("Primary"));
                    return DoCalculate(ast.nodes[0]);
                }
            case "Primary/0":
                {
                    {
                        Assert.That(ast.nodes.Count, Is.EqualTo(1));
                        Assert.That(ast.nodes[0].name, Is.EqualTo("Additive"));
                        return DoCalculate(ast.nodes[0]);
                    }
                }
            case "Primary/1":
                {
                    {
                        Assert.That(ast.nodes.Count, Is.EqualTo(1));
                        Assert.That(ast.nodes[0].name, Is.EqualTo("Number"));
                        return DoCalculate(ast.nodes[0]);
                    }
                }
            case "Number":
                {
                    Assert.That(ast.is_token, Is.True);
                    return int.Parse(ast.token);
                }
            default:
                Echo(ast);
                throw new System.Exception($"{ast.name_choice} not supported");
        }
    }
}