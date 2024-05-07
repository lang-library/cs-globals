using NUnit.Framework;
using static Global.EasyObject;

namespace Global.Sample;

public class IntCalculator
{
    protected string grammar;
    public IntCalculator(string grammar)
    {
        this.grammar = grammar;
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
                    Assert.That(ast.nodes[0].name, Is.EqualTo("abc"));
                    Assert.That(ast.nodes[1].name, Is.EqualTo("xyz"));
                    return 0;
                }
            default:
                Echo(ast);
                throw new System.Exception($"{ast.name_choice} not supported");
        }
    }
}