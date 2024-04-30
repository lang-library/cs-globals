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
        MyData.ForceASCII = false;
        MyData.DecimalAsString = false;
    }

    [Test]
    public void Test1()
    {
        MyData.DecimalAsString = false;
        string json = ObjectToJson(12345678901234567890123456789m);
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("""12345678901234567890123456789"""));
    }

    [Test]
    public void Test2()
    {
        MyData.DecimalAsString = false;
        string json = ObjectToJson(new { a = true });
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("""{"a":true}"""));
    }
    [Test]
    public void Test3()
    {
        MyData.DecimalAsString = false;
        string json = ObjectToJson(new object[] { true, false, null });
        tool.Echo(json, "json");
        Assert.That(json, Is.EqualTo("""[true,false,null]"""));
        MyData array = MyData.FromString(json);
        tool.Echo($"array.IsArray: {array.IsArray}");
        Assert.True(array.IsArray);
        var array2 = array.AsStringArray;
        CheckObjectJson(array2, """["True","False","null"]""");
    }
    [Test]
    public void Test4()
    {
        MyData.DecimalAsString = false;
        string json = ObjectToJson(123);
        Assert.That(json, Is.EqualTo("""123"""));
        MyData array = MyData.FromString(json);
        Assert.False(array.IsArray);
        var array2 = array.AsStringArray;
        CheckObjectJson(array2, """null""");
    }
    [Test]
    public void Test5()
    {
        MyData.DecimalAsString = false;
        var n1 = new MyNumber(777);
        CheckObjectJson(n1, "777");
        Assert.That(() => new MyNumber(null), Throws.TypeOf<ArgumentException>()
        .With.Message.EqualTo("Argument is null"));
        Assert.That(() => new MyNumber("abc"), Throws.TypeOf<ArgumentException>()
        .With.Message.EqualTo("Argument is not numeric: System.String"));
    }
    protected string ObjectToJson(object x, bool indent = false)
    {
        MyData mj = MyData.FromObject(x);
        return mj.ToString(indent);
    }
    protected string ReformatJson(string json, bool indent = false)
    {
        MyData mj = MyData.FromString(json);
        return mj.ToString(indent);
    }
    protected void CheckObjectJson(object x, string expectedJson)
    {
        string actualJson = ObjectToJson(x);
        Assert.That(actualJson, Is.EqualTo(expectedJson));
    }
}