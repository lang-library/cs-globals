namespace MyJson.Test;

using Globals;

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
        string json = ObjectToJson(new {a = true});
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
}