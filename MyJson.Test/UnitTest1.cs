namespace MyJson.Test;

using Antlr4.Runtime.Misc;
using Globals;
using System;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        MyJson.ForceASCII = false;
        MyJson.DecimalAsString = false;
    }

    [Test]
    public void Test1()
    {
        MyJson.DecimalAsString = false;
        string json = ObjectToJson(12345678901234567890123456789m);
        TestContext.Out.WriteLine(json);
        Assert.That(json, Is.EqualTo("""12345678901234567890123456789"""));
    }

    [Test]
    public void Test2()
    {
        MyJson.DecimalAsString = false;
        string json = ObjectToJson(new { a = true });
        TestContext.Out.WriteLine(json);
        Assert.That(json, Is.EqualTo("""{"a":true}"""));
    }
    [Test]
    public void Test3()
    {
        MyJson.DecimalAsString = false;
        string json = ObjectToJson(new object[] { true, false, null });
        TestContext.Out.WriteLine(json);
        Assert.That(json, Is.EqualTo("""[true,false,null]"""));
        MyJson array = MyJson.FromString(json);
        TestContext.Out.WriteLine($"array.IsArray: {array.IsArray}");
        Assert.True(array.IsArray);
        var array2 = array.AsStringArray;
        json = ObjectToJson(array2);
        TestContext.Out.WriteLine(json);
        Assert.That(json, Is.EqualTo("""["True","False","null"]"""));
    }
    [Test]
    public void Test4()
    {
        MyJson.DecimalAsString = false;
        string json = ObjectToJson(123);
        Assert.That(json, Is.EqualTo("""123"""));
        MyJson array = MyJson.FromString(json);
        Assert.False(array.IsArray);
        var array2 = array.AsStringArray;
        CheckObjectJson(array2, """null""");
    }
    [Test]
    public void Test5()
    {
        MyJson.DecimalAsString = false;
        var n1 = new MyNumber(777);
        Assert.That(() => new MyNumber(null), Throws.TypeOf<ArgumentException>()
        .With.Message.EqualTo("Argument is null"));
        Assert.That(() => new MyNumber("abc"), Throws.TypeOf<ArgumentException>()
        .With.Message.EqualTo("Argument is not numeric: System.String"));
    }
    protected string ObjectToJson(object x, bool indent = false)
    {
        MyJson mj = MyJson.FromObject(x);
        return mj.ToString(indent);
    }
    protected string ReformatJson(string json, bool indent = false)
    {
        MyJson mj = MyJson.FromString(json);
        return mj.ToString(indent);
    }
    protected void CheckObjectJson(object x, string expectedJson)
    {
        string actualJson = ObjectToJson(x);
        Assert.That(actualJson, Is.EqualTo(expectedJson));
    }
}