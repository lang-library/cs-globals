using System;
using MyJson;
using System.Text;
using System.Collections.Generic;
using System.Dynamic;
namespace Main;


class MyClass
{
    public int abc = 123;
    public static int Add2(int x, int y)
    {
        return x + y;
    }
}
static class Program
{
    [STAThread]
    static void Main(string[] originalArgs)
    {
        MyTool tool = new MyTool().SetDebugOutput(true);
        tool.Echo("welcom ようこそ!");
        {
            dynamic exp = new ExpandoObject();
            exp.A = 123;
            Console.WriteLine(exp);
            var dic = exp as IDictionary<string, object>;
            foreach (var key in dic.Keys)
            {
                Console.WriteLine(key);
                Console.WriteLine(dic[key]);
            }
            tool.Log(exp, "exp");
#if true
            //MyJson.MyData.NumberAsDecimal = false;
            MyJson.MyData objArray = MyJson.MyData.FromJson("""[11, 22, { a: 777}]""");
            tool.Echo(objArray, "objArray");
            var objArray2 = objArray.AsObjectArray;
            foreach(var obj in objArray2)
            {
                tool.Echo(obj);
            }
            var objList = objArray.AsObjectList;
            foreach (var obj in objList)
            {
                tool.Echo(obj);
            }
            var decArray = objArray.AsDecimalArray;
            foreach (var obj in decArray)
            {
                tool.Echo(obj);
            }
            var decList = objArray.AsDecimalList;
            foreach (var obj in decList)
            {
                tool.Echo(obj);
            }
            var dblArray = objArray.AsDoubleArray;
            foreach (var obj in dblArray)
            {
                tool.Echo(obj);
            }
            var dblList = objArray.AsDoubleList;
            foreach (var obj in dblList)
            {
                tool.Echo(obj);
            }
#endif
#if true
            MyJson.MyData myjson = MyJson.MyData.FromObject("abcハロー©");
            tool.Echo(myjson, "myjson");
            string s = myjson.Value;
            tool.Echo(s, "s");
            myjson = MyJson.MyData.FromObject(1.23);
            tool.Echo(myjson, "myjson");
            double dbl = myjson.AsDouble;
            tool.Echo(dbl, "dbl");
            myjson = MyJson.MyData.FromObject(4.56f);
            tool.Echo(myjson, "myjson");
            float flt = myjson.AsFloat;
            tool.Echo(flt, "flt");
            myjson = MyJson.MyData.FromObject(123);
            tool.Echo(myjson, "myjson");
            int i = myjson.AsInt;
            tool.Echo(i, "i");
            myjson = MyJson.MyData.FromObject(456L);
            tool.Echo(myjson, "myjson");
            long l = myjson.AsLong;
            tool.Echo(l, "l");
            myjson = MyJson.MyData.FromObject(789UL);
            tool.Echo(myjson, "myjson");
            ulong ul = myjson.AsULong;
            tool.Echo(ul, "ul");
            myjson = MyJson.MyData.FromObject(true);
            tool.Echo(myjson, "myjson");
            bool b = myjson.AsBool;
            tool.Echo(b, "b");
            myjson = MyJson.MyData.FromObject(1234567890123456789m);
            tool.Echo(myjson, "myjson");
            decimal dec = myjson.AsDecimal;
            tool.Echo(dec, "dec");
            //MyJson.MyData.DecimalAsString = true;
            myjson = MyJson.MyData.FromObject(1234567890123456789m);
            tool.Echo(myjson, "myjson");
            dec = myjson.AsDecimal;
            tool.Echo(dec, "dec");
            myjson = MyJson.MyData.FromObject((byte)123);
            tool.Echo(myjson, "myjson");
            byte bt = myjson.AsByte;
            tool.Echo(bt, "bt");
            myjson = MyJson.MyData.FromObject((sbyte)-123);
            tool.Echo(myjson, "myjson");
            sbyte sbt = myjson.AsSByte;
            tool.Echo(sbt, "sbt");
            short st = -456;
            myjson = MyJson.MyData.FromObject(st);
            tool.Echo(myjson, "myjson");
            st = myjson.AsShort;
            tool.Echo(st, "st");
            ushort ust = 777;
            myjson = MyJson.MyData.FromObject(ust);
            tool.Echo(myjson, "myjson");
            ust = myjson.AsUShort;
            tool.Echo(ust, "ust");
            var now = DateTime.Now;
            myjson = MyJson.MyData.FromObject(now);
            tool.Echo(myjson, "myjson");
            DateTime dt = myjson.AsDateTime;
            tool.Echo(dt, "dt");
            TimeSpan tsp = new TimeSpan(1, 10, 30, 50, 500);
            myjson = MyJson.MyData.FromObject(tsp);
            tool.Echo(myjson, "myjson");
            tsp = myjson.AsTimeSpan;
            tool.Echo(tsp, "tsp");
            Guid guid = Guid.NewGuid();
            myjson = MyJson.MyData.FromObject(guid);
            tool.Echo(myjson, "myjson");
            guid = myjson.AsGuid;
            tool.Echo(guid, "guid");
            byte[] bytes = Encoding.UTF8.GetBytes("abc");
            myjson = MyJson.MyData.FromObject(bytes);
            tool.Echo(myjson, "myjson");
            bytes = myjson.AsByteArray;
            tool.Echo(bytes, "bytes");
            var byteList = new List<byte>(bytes);
            myjson = MyJson.MyData.FromObject(byteList);
            tool.Echo(myjson, "myjson");
            byteList = myjson.AsByteList;
            tool.Echo(byteList, "byteList");
            string[] strings = new string[] { "abc", "xyz" };
            myjson = MyJson.MyData.FromObject(strings);
            tool.Echo(myjson, "myjson");
            strings = myjson.AsStringArray;
            tool.Echo(strings, "strings");
            var stringList = new List<string>(strings);
            myjson = MyJson.MyData.FromObject(stringList);
            tool.Echo(myjson, "myjson");
            stringList = myjson.AsStringList;
            tool.Echo(stringList, "stringList");
            myjson = MyJson.MyData.FromJson("""
                { a: [1, 2, 3, true, null] }
                """);
            tool.Echo(myjson, "myjson");
            //MyJson.MyData.DecimalAsString = false;
            myjson = MyJson.MyData.FromObject(12345678901234567890123456789m);
            tool.Echo(myjson, "myjson");
            //MyJson.MyData.DecimalAsString = true;
            var o1 = MyData.ToObject(myjson);
            tool.Echo(o1, "o1");
            tool.Echo(tool.FullName(o1));
            //MyJson.MyData.DecimalAsString = false;
            var o2 = MyData.ToObject(myjson);
            tool.Echo(o2, "o2");
            tool.Echo(tool.FullName(o2));
            //MyJson.MyData.DecimalAsString = true;
            tool.Echo(o2, "o2");
            tool.Echo(tool.FullName(o2));
#endif
        }
        Environment.Exit(0);
#if false
        var xyz = MyJson.FromObject(new { a = 12345678901234567890123456789m });
        tool.Echo(xyz);
        MyJson.DecimalAsString = true;
        xyz = new MyParser(true).FromObject(new { a = 12345678901234567890123456789m });
        tool.Echo(xyz);
        var now = DateTime.Now;
        tool.Echo(now, "now");
        tool.Echo(new { now = now });
        var nowStr = tool.DateTimeString(now);
        tool.Echo(nowStr);
        //DateTime.TryParse();
        var jno2 = FromJson("{ b : 12345678901234567890123456789 }");
        tool.Echo(jno2);
        var jno = new { a = 12345678901234567890123456789m };
        tool.Echo(jno);
        MyJson jn = 12345678901234567890123456789m;
        //tool.Echo(FullName(jn));
        tool.Echo(jn);
        //tool.Echo(jn.ToString());
        jn = DateTime.Now;
        tool.Echo(jn);


        tool.Echo(FromJson("{'abc': /* this is comment */123, b: 'ハロー©'}"));
        var api = new JsonClient("cpp_api_01.dll");
        var res = api.CallAsMyJson("add2", new[] { 11, 22 });
        tool.Echo(res);
        //tool.Echo(FullName(res));
        var res2 = api.Call("add2", new[] { 11, 22 });
        tool.Echo(res2);
        //tool.Echo(FullName(res));

        tool.Echo(MyJson.FromString("{'abc': /* this is comment */123, b: 'ハロー©'}"));
        var bytes = System.Text.Encoding.UTF8.GetBytes("abcハロー©");
        tool.Echo(bytes);
        var bytesJson = ToJson(bytes);
        bytes = MyJson.FromString(bytesJson).AsByteArray;
        var s = System.Text.Encoding.UTF8.GetString(bytes);
        tool.Echo(s);
        var nowJson = ToJson(now);
        now = MyJson.FromString(nowJson).AsDateTime;
        Console.WriteLine(now);
        tool.Echo(now);
        var objJson = ToJson(new { now = now });
        tool.Echo(objJson);
        var obj = MyJson.FromString(objJson);
        tool.Echo(obj);
        var dt = obj["now"];
        tool.Echo(dt);
        now = dt.AsDateTime;
        Console.WriteLine(now);
        decimal dec = 18446744073709551615123m;
        var decObj = AsMyJson(new { dec = dec });
        tool.tool.Echo(decObj);
        var dec2 = decObj["dec"];
        Console.WriteLine($"dec2.AsDecimal={dec2.AsDecimal}");
        decimal dec3 = dec2.AsDecimal;
        Console.WriteLine(dec3);
        double dec4 = dec2.AsDouble;
        Console.WriteLine(dec4);


        var test = MyJson.FromString("[1, 2, /*abc*/ 3]");
        tool.Echo(test);
        var engine = JintScript.CreateEngine();
        engine.Execute("print(appFile());");
        engine.Execute("print(appDir());");
        var ldb = new LiteDBProps(".javacommons", "slim.runtime");
        ldb.Put("abc", new[] { "abc" });
        tool.tool.Echo(ldb.Get("abc"));
        //string s = (string)tool.FromJson("'abc'");
        //tool.tool.Echo(s, "s");
        tool.tool.Echo(tool.GetACP(), "tool.GetACP()");
        tool.tool.Echo(new { a = 12345678901234567890123456m, b = "hello" }); //18,446,744,073,709,551,615
        tool.tool.Echo(new { a = 18446744073709551615m, b = "hello" }); //18,446,744,073,709,551,615
        var d = MyJson.FromString("12345678901234567890123456");
        tool.tool.Echo(d);
        tool.tool.Echo((double)d);
        //tool.Print((int)d);
        tool.tool.Echo(d.AsDouble);
        tool.tool.Echo(d.AsDouble.ToString());
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
        var connectionString = "Data Source=db.db3";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "create table if not exists user(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)";
            command.ExecuteNonQuery();
            command.CommandText = "insert into user(name) values($name)";
            command.Parameters.AddWithValue("$name", "tomトム©");
            command.ExecuteNonQuery();
            command.CommandText =
            @"
             SELECT name
             FROM user
             ";
            //command.Parameters.AddWithValue("$id", 1);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    Console.WriteLine($"Hello, {name}!");
                }
            }
            connection.Close();
        }
#endif
    }
}