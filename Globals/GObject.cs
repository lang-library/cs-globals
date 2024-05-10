using Global.Sample;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Global;

public enum GObjectType
{
    @string, @number, @boolean, @object, @array, @null
}

public class GObject : DynamicObject, IObjectWrapper
{
    internal object m_data = null;

    public static IJsonHandler DefaultJsonHandler = new Win32NLJsonHandler(true, false);
    public static IJsonHandler JsonHandler = null;
    public static bool DebugOutput = false;
    public static bool ShowDetail = false;
    public static bool ForceASCII = false;

    public static void ClearSettings()
    {
        GObject.JsonHandler = DefaultJsonHandler;
        GObject.DebugOutput = false;
        GObject.ShowDetail = false;
        GObject.ForceASCII = false;
    }

    static GObject()
    {
        GObject.ClearSettings();
    }

    public GObject()
    {
        this.m_data = null;
    }

    public GObject(object x)
    {
        this.m_data = new ObjectParser(false).Parse(x);
    }

    public dynamic Dynamic {  get { return this; } }

    public override string ToString()
    {
        return this.ToPrintable();
    }

    public string ToPrintable()
    {
        return GObject.ToPrintable(this);
    }

    public static GObject Null { get { return new GObject(); } }
    public static GObject EmptyArray { get { return new GObject(new List<object>()); } }
    public static GObject EmptyObject { get { return new GObject(new Dictionary<string, object>()); } }

    public static GObjectType @string { get { return GObjectType.@string; } }
    public static GObjectType @boolean { get { return GObjectType.@boolean; } }
    public static GObjectType @object { get { return GObjectType.@object; } }
    public static GObjectType @array { get { return GObjectType.@array; } }
    public static GObjectType @null { get { return GObjectType.@null; } }

    public bool IsString { get { return this.TypeValue == GObjectType.@string; } }
    public bool IsNumber { get { return this.TypeValue == GObjectType.@number; } }
    public bool IsBoolean { get { return this.TypeValue == GObjectType.@boolean; } }
    public bool IsObject { get { return this.TypeValue == GObjectType.@object; } }
    public bool IsArray { get { return this.TypeValue == GObjectType.@array; } }
    public bool IsNull { get { return this.TypeValue == GObjectType.@null; } }

    private static object UnWrapInternal(object x)
    {
        while (x is GObject)
        {
            x = ((GObject)x).m_data;
        }
        return x;
    }

    private static GObject WrapInternal(object x)
    {
        if (x is GObject) return x as GObject;
        return new GObject(x);
    }

    public object UnWrap()
    {
        return GObject.UnWrapInternal(this);
    }

    public GObjectType TypeValue
    {
        get
        {
            object obj = UnWrapInternal(this);
            if (obj == null) return GObjectType.@null;
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Boolean:
                    return GObjectType.@boolean;
                case TypeCode.String:
                case TypeCode.Char:
                case TypeCode.DateTime:
                    return GObjectType.@string;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return GObjectType.@number;
                case TypeCode.Object:
                    return (obj is List<object>) ? GObjectType.@array : GObjectType.@object;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                default:
                    if (obj is TimeSpan || obj is Guid) return GObject.@string;
                    return GObjectType.@null;
            }
        }
    }

    public string TypeName
    {
        get
        {
            return this.TypeValue.ToString();
        }
    }

    private List<object> list
    {
        get { return m_data as List<object>; }
    }

    private Dictionary<string, object> dictionary
    {
        get { return m_data as Dictionary<string, object>; }
    }

    public int Count
    {
        get
        {
            if (list != null) return list.Count;
            if (dictionary != null) return dictionary.Count;
            return 0;
        }
    }

    public List<string> Keys
    {
        get
        {
            var keys = new List<string>();
            if (dictionary == null) return keys;
            foreach (var key in dictionary.Keys) keys.Add(key);
            return keys;
        }
    }

    public bool ContainsKey(string name)
    {
        if (dictionary == null) return false;
        return dictionary.ContainsKey(name);
    }

    public GObject Add(object x)
    {
        if (list == null) m_data = new List<object>();
        list.Add(x);
        return this;
    }

    public GObject Add(string key, object x)
    {
        if (dictionary == null) m_data = new Dictionary<string, object>();
        dictionary.Add(key, x);
        return this;
    }

    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        result = null;
        if (dictionary == null) return true;
        string name = binder.Name;
        result = null;
        dictionary.TryGetValue(name, out result);
        result = WrapInternal(result);
        return true;
    }

    public override bool TrySetMember(
        SetMemberBinder binder, object value)
    {
        value = UnWrapInternal(value);
        if (dictionary == null)
        {
            m_data = new Dictionary<string, object>();
        }
        string name = binder.Name;
        dictionary[name] = value;
        return true;
    }
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
        result = null;
        var idx = indexes[0];
        if (idx is int)
        {
            int pos = (int)indexes[0];
            if (list == null)
            {
                result = WrapInternal(null);
                return true;
            }
            if (list.Count < (pos + 1))
            {
                result = WrapInternal(null);
                return true;
            }
            result = WrapInternal(list[pos]);
            return true;
        }
        if (dictionary == null)
        {
            result = null;
            return true;
        }
        result = null;
        dictionary.TryGetValue((string)indexes[0], out result);
        result = WrapInternal(result);
        return true;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    {
        if (value is GObject) value = ((GObject)value).m_data;
        var idx = indexes[0];
        if (idx is int)
        {
            int pos = (int)indexes[0];
            if (pos < 0) throw new ArgumentException("index is below 0");
            if (list == null)
            {
                m_data = new List<object>();
            }
            while (list.Count < (pos + 1))
            {
                list.Add(null);
            }
            list[pos] = value;
            return true;
        }
        if (dictionary == null)
        {
            m_data = new Dictionary<string, object>();
        }
        string name = (string)indexes[0];
        dictionary[name] = value;
        return true;
    }

    public override bool TryConvert(ConvertBinder binder, out object result)
    {
        if (binder.Type == typeof(IEnumerable))
        {
            if (list != null)
            {
                var ie1 = list.Select(x => x);
                result = ie1;
                return true;
            }
            if (dictionary != null)
            {
                var ie2 = dictionary.Select(x => x);
                result = ie2;
                return true;
            }
            result = (new List<object>()).Select(x => x);
            return true;
        }
        else
        {
            result = Convert.ChangeType(m_data, binder.Type);
            return true;
        }
    }

    public static GObject FromObject(object obj)
    {
        return new GObject(obj);
    }

    public static GObject FromJson(string json)
    {
        return new GObject(JsonHandler.Parse(json));
    }

    public dynamic ToObject()
    {
        return new ObjectParser(false).Parse(m_data);
    }

    public string ToJson(bool indent = false)
    {
        return JsonHandler.Stringify(m_data, indent);
    }

    public static string ToPrintable(object x, string title = null)
    {
        return ObjectParser.ToPrintable(ShowDetail, x, title);
    }

    public static void Echo(object x, string title = null)
    {
        String s = ToPrintable(x, title);
        Console.WriteLine(s);
        System.Diagnostics.Debug.WriteLine(s);
    }
    public static void Log(object x, string? title = null)
    {
        String s = ToPrintable(x, title);
        Console.Error.WriteLine("[Log] " + s);
        System.Diagnostics.Debug.WriteLine("[Log] " + s);
    }
    public static void Debug(object x, string? title = null)
    {
        if (!DebugOutput) return;
        String s = ToPrintable(x, title);
        Console.Error.WriteLine("[Debug] " + s);
        System.Diagnostics.Debug.WriteLine("[Debug] " + s);
    }
    public static void Message(object x, string? title = null)
    {
        if (title == null) title = "Message";
        String s = ToPrintable(x, null);
        NativeMethods.MessageBoxW(IntPtr.Zero, s, title, 0);
    }
    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int MessageBoxW(
            IntPtr hWnd, string lpText, string lpCaption, uint uType);
    }
    public GObject this[string name]
    {
        get
        {
            if (dictionary == null) return null;
            object o = null;
            dictionary.TryGetValue(name, out o);
            return WrapInternal(o);
        }
        set
        {
            if (dictionary == null)
            {
                m_data = new Dictionary<string, object>();
            }
            dictionary[name] = UnWrapInternal(value);
        }
    }
    public GObject this[int pos]
    {
        get
        {
            if (list == null)
            {
                return WrapInternal(null);
            }
            if (list.Count < (pos + 1))
            {
                return WrapInternal(null);
            }
            return WrapInternal(list[pos]);
        }
        set
        {
            if (pos < 0) throw new ArgumentException("index below 0");
            if (list == null)
            {
                m_data = new List<object>();
            }
            while (list.Count < (pos + 1))
            {
                list.Add(null);
            }
            list[pos] = value;
        }
    }
    public T Cast<T>()
    {
        return (T)Convert.ChangeType(this.m_data, typeof(T));
    }
    public List<GObject> AsList
    {
        get
        {
            var result = new List<GObject>();
            if (list == null) return result;
            foreach(var item in list)
            {
                result.Add(WrapInternal(item));
            }
            return result;
        }
    }
    public Dictionary<string, GObject> AsDictionary
    {
        get
        {
            var result = new Dictionary<string, GObject>();
            if (dictionary == null) return result;
            foreach (var item in dictionary)
            {
                result[item.Key] = WrapInternal(item.Value);
            }
            return result;
        }

    }

    public static string FullName(dynamic x)
    {
        if (x is null) return "null";
        string fullName = ((object)x).GetType().FullName;
        return fullName.Split('`')[0];
    }

    public static implicit operator GObject(bool x) { return new GObject(x); }
    public static implicit operator GObject(string x) { return new GObject(x); }
    public static implicit operator GObject(char x) { return new GObject(x); }
    public static implicit operator GObject(short x) { return new GObject(x); }
    public static implicit operator GObject(int x) { return new GObject(x); }
    public static implicit operator GObject(long x) { return new GObject(x); }
    public static implicit operator GObject(ushort x) { return new GObject(x); }
    public static implicit operator GObject(uint x) { return new GObject(x); }
    public static implicit operator GObject(ulong x) { return new GObject(x); }
    public static implicit operator GObject(float x) { return new GObject(x); }
    public static implicit operator GObject(double x) { return new GObject(x); }
    public static implicit operator GObject(decimal x) { return new GObject(x); }
    public static implicit operator GObject(sbyte x) { return new GObject(x); }
    public static implicit operator GObject(byte x) { return new GObject(x); }
    public static implicit operator GObject(DateTime x) { return new GObject(x); }
    public static implicit operator GObject(TimeSpan x) { return new GObject(x); }
    public static implicit operator GObject(Guid x) { return new GObject(x); }

    public void Nullify()
    {
        this.m_data = null;
    }
}
