namespace MyData.Test;

using Antlr4.Runtime.Misc;
using MyJson;
using System;

public class Tests
{
    MyTool tool = new MyTool();
    [SetUp]
    public void Setup()
    {
        //MyData.ForceASCII = false;
        //MyData.DecimalAsString = false;
        tool.Echo("Setup() called");
    }

    [Test]
    public void Test1A()
    {
        var myTool = new MyTool()
            .SetDecimalAsString(true) // decimal を文字列に変換
            .SetForceASCII(false)
            .SetNumberAsDecimal(false); // オブジェクトの読み込み時には、false でも decimal のまま読み込まれる
        string json = ObjectToJson(myTool, 12345678901234567890123456789m);
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("\"12345678901234567890123456789\"")); // 文字列のJSON
    }
    [Test]
    public void Test1B()
    {
        var myTool = new MyTool()
            .SetDecimalAsString(true)
            .SetForceASCII(false)
            .SetNumberAsDecimal(false);
        string json = ReformatJson(myTool, "12345678901234567890123456789");
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("""1.23456789012346E+28"""));
    }
    [Test]
    public void Test1C()
    {
        var myTool = new MyTool()
            .SetDecimalAsString(true)
            .SetForceASCII(false)
            .SetNumberAsDecimal(true);
        var o = ObjectToObject(myTool, 12345678901234567890123456789m);
        tool.Echo(o, "o");
        Assert.That(o, Is.EqualTo("12345678901234567890123456789")); // 文字列
    }

    [Test]
    public void Test2()
    {
        //MyData.DecimalAsString = false;
        string json = ObjectToJson(tool, new { a = true });
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("""{"a":true}"""));
    }
    [Test]
    public void Test3()
    {
        //MyData.DecimalAsString = false;
        string json = ObjectToJson(tool, new object[] { true, false, null });
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("""[true,false,null]"""));
        MyData array = MyData.FromJson(json);
        tool.Echo($"array.IsArray: {array.IsArray}");
        Assert.True(array.IsArray);
        var array2 = array.AsStringArray;
        CheckObjectJson(array2, """["True","False","null"]""");
    }
    [Test]
    public void Test4()
    {
        //MyData.DecimalAsString = false;
        string json = ObjectToJson(tool, 123);
        Assert.That(json, Is.EqualTo("""123"""));
        MyData array = MyData.FromJson(json);
        Assert.False(array.IsArray);
        var array2 = array.AsStringArray;
        CheckObjectJson(array2, """null""");
    }
    [Test]
    public void Test5()
    {
        //MyData.DecimalAsString = false;
        var n1 = new MyNumber(777);
        CheckObjectJson(n1, "777");
        Assert.That(() => new MyNumber(null), Throws.TypeOf<ArgumentException>()
        .With.Message.EqualTo("Argument is null"));
        Assert.That(() => new MyNumber("abc"), Throws.TypeOf<ArgumentException>()
        .With.Message.EqualTo("Argument is not numeric: System.String"));
    }
    protected string ObjectToJson(MyTool myTool, object x, bool indent = false)
    {
        MyData mj = myTool.FromObject(x);
        return myTool.ToJson(mj, indent);
    }
    protected dynamic ObjectToObject(MyTool myTool, object x)
    {
        MyData mj = myTool.FromObject(x);
        return myTool.ToObject(mj);
    }
    protected string ReformatJson(MyTool myTool, string json, bool indent = false)
    {
        MyData mj = myTool.FromJson(json);
        return myTool.ToJson(mj, indent);
    }
    protected void CheckObjectJson(object x, string expectedJson)
    {
        string actualJson = ObjectToJson(tool, x);
        Assert.That(actualJson, Is.EqualTo(expectedJson));
    }
}