#region License and information
#endregion License and information

using Antlr4.Runtime;
using Globals.Parser.Json5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Globals;

#region enums
public enum MyNodeType
{
    Array = 1,
    Object = 2,
    String = 3,
    Number = 4,
    NullValue = 5,
    Boolean = 6,
    None = 7,
    Custom = 0xFF,
}
public enum MyTextMode
{
    Compact,
    Indent
}
#endregion enums

#region MyJson
public abstract partial class MyJson
{
    #region Enumerators
    public struct Enumerator
    {
        private enum Type { None, Array, Object }
        private Type type;
        private Dictionary<string, MyJson>.Enumerator m_Object;
        private List<MyJson>.Enumerator m_Array;
        public bool IsValid { get { return type != Type.None; } }
        public Enumerator(List<MyJson>.Enumerator aArrayEnum)
        {
            type = Type.Array;
            m_Object = default(Dictionary<string, MyJson>.Enumerator);
            m_Array = aArrayEnum;
        }
        public Enumerator(Dictionary<string, MyJson>.Enumerator aDictEnum)
        {
            type = Type.Object;
            m_Object = aDictEnum;
            m_Array = default(List<MyJson>.Enumerator);
        }
        public KeyValuePair<string, MyJson> Current
        {
            get
            {
                if (type == Type.Array)
                    return new KeyValuePair<string, MyJson>(string.Empty, m_Array.Current);
                else if (type == Type.Object)
                    return m_Object.Current;
                return new KeyValuePair<string, MyJson>(string.Empty, null);
            }
        }
        public bool MoveNext()
        {
            if (type == Type.Array)
                return m_Array.MoveNext();
            else if (type == Type.Object)
                return m_Object.MoveNext();
            return false;
        }
    }
    public struct ValueEnumerator
    {
        private Enumerator m_Enumerator;
        public ValueEnumerator(List<MyJson>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
        public ValueEnumerator(Dictionary<string, MyJson>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
        public ValueEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
        public MyJson Current { get { return m_Enumerator.Current.Value; } }
        public bool MoveNext() { return m_Enumerator.MoveNext(); }
        public ValueEnumerator GetEnumerator() { return this; }
    }
    public struct KeyEnumerator
    {
        private Enumerator m_Enumerator;
        public KeyEnumerator(List<MyJson>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
        public KeyEnumerator(Dictionary<string, MyJson>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
        public KeyEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
        public string Current { get { return m_Enumerator.Current.Key; } }
        public bool MoveNext() { return m_Enumerator.MoveNext(); }
        public KeyEnumerator GetEnumerator() { return this; }
    }

    public class LinqEnumerator : IEnumerator<KeyValuePair<string, MyJson>>, IEnumerable<KeyValuePair<string, MyJson>>
    {
        private MyJson m_Node;
        private Enumerator m_Enumerator;
        internal LinqEnumerator(MyJson aNode)
        {
            m_Node = aNode;
            if (m_Node != null)
                m_Enumerator = m_Node.GetEnumerator();
        }
        public KeyValuePair<string, MyJson> Current { get { return m_Enumerator.Current; } }
        object IEnumerator.Current { get { return m_Enumerator.Current; } }
        public bool MoveNext() { return m_Enumerator.MoveNext(); }

        public void Dispose()
        {
            m_Node = null;
            m_Enumerator = new Enumerator();
        }

        public IEnumerator<KeyValuePair<string, MyJson>> GetEnumerator()
        {
            return new LinqEnumerator(m_Node);
        }

        public void Reset()
        {
            if (m_Node != null)
                m_Enumerator = m_Node.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LinqEnumerator(m_Node);
        }
    }

    #endregion Enumerators

    #region common interface

    public static bool ForceASCII = false; // Use Unicode by default
    public static bool NumberAsDecimal = false;
    public static bool DecimalAsString = false;

    public abstract MyNodeType Tag { get; }

    public virtual MyJson this[int aIndex] { get { return null; } set { } }

    public virtual MyJson this[string aKey] { get { return null; } set { } }

    public virtual string Value { get { return ""; } set { } }

    public virtual int Count { get { return 0; } }

    public virtual bool IsNumber { get { return false; } }
    public virtual bool IsString { get { return false; } }
    public virtual bool IsBoolean { get { return false; } }
    public virtual bool IsNull { get { return false; } }
    public virtual bool IsArray { get { return false; } }
    public virtual bool IsObject { get { return false; } }

    public virtual bool Inline { get { return false; } set { } }

    public virtual void Add(string aKey, MyJson aItem)
    {
    }
    public virtual void Add(MyJson aItem)
    {
        Add("", aItem);
    }

    public virtual MyJson Remove(string aKey)
    {
        return null;
    }

    public virtual MyJson Remove(int aIndex)
    {
        return null;
    }

    public virtual MyJson Remove(MyJson aNode)
    {
        return aNode;
    }
    public virtual void Clear() { }

    public virtual MyJson Clone()
    {
        return null;
    }

    public virtual IEnumerable<MyJson> Children
    {
        get
        {
            yield break;
        }
    }

    public IEnumerable<MyJson> DeepChildren
    {
        get
        {
            foreach (var C in Children)
                foreach (var D in C.DeepChildren)
                    yield return D;
        }
    }

    public virtual bool HasKey(string aKey)
    {
        return false;
    }

    public virtual MyJson GetValueOrDefault(string aKey, MyJson aDefault)
    {
        return aDefault;
    }

    #region ToString()
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        WriteToStringBuilder(sb, 0, 0, MyTextMode.Compact);
        return sb.ToString();
    }

    public virtual string ToString(int aIndent)
    {
        StringBuilder sb = new StringBuilder();
        WriteToStringBuilder(sb, 0, aIndent, MyTextMode.Indent);
        return sb.ToString();
    }
    public virtual string ToString(bool indent)
    {
        if (indent) return ToString(2);
        return ToString();
    }
    internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode);
    #endregion ToString()

    #region ToObject()
    public dynamic ToObject()
    {
        if (this is MyNull) return null;
        if (this is MyBool) return this.AsBool;
        if (this is MyNumber)
        {
            if (DecimalAsString) return this.AsDouble;
            return this.AsDecimal;
        }
        if (this is MyString) return this.Value;
        if (this is MyArray)
        {
            var result = new List<object>();
            var array = this as MyArray;
            for (int i=0; i<array!.Count; i++)
            {
                result.Add(array[i].ToObject());
            }
            return result;
        }
        if (this is MyObject)
        {
            var result = new Dictionary<string, object>();
            var obj = this as MyObject;
            var keys = obj!.Keys;
            foreach (var key in keys)
            {
                result[key] = obj![key].ToObject();
            }
            return result;
        }
        throw new Exception($"{this.GetType().FullName} is not supported");
    }
    #endregion ToObject()

    #region enumerator
    public abstract Enumerator GetEnumerator();
    public IEnumerable<KeyValuePair<string, MyJson>> Linq { get { return new LinqEnumerator(this); } }
    public KeyEnumerator Keys { get { return new KeyEnumerator(GetEnumerator()); } }
    public ValueEnumerator Values { get { return new ValueEnumerator(GetEnumerator()); } }
    #endregion enumerator

    #endregion common interface

    #region typecasting properties


    public virtual double AsDouble
    {
        get
        {
            double v = 0.0;
            if (double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                return v;
            return 0.0;
        }
        set
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public virtual int AsInt
    {
        get { return (int)AsDouble; }
        set { AsDouble = value; }
    }

    public virtual float AsFloat
    {
        get { return (float)AsDouble; }
        set { AsDouble = value; }
    }

    public virtual bool AsBool
    {
        get
        {
            bool v = false;
            if (bool.TryParse(Value, out v))
                return v;
            return !string.IsNullOrEmpty(Value);
        }
        set
        {
            Value = (value) ? "true" : "false";
        }
    }

    public virtual long AsLong
    {
        get
        {
            long val = 0;
            if (long.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
                return val;
            return 0L;
        }
        set
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public virtual ulong AsULong
    {
        get
        {
            ulong val = 0;
            if (ulong.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
                return val;
            return 0;
        }
        set
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public virtual MyArray AsArray
    {
        get
        {
            return this as MyArray;
        }
    }

    public virtual MyObject AsObject
    {
        get
        {
            return this as MyObject;
        }
    }


    #endregion typecasting properties

    #region operators

    #region String
    public static implicit operator MyJson(string s)
    {
        return FromObject(s);
    }
    public static implicit operator string(MyJson d)
    {
        return (d == null) ? null : d.Value;
    }
    #endregion String

    #region Double
    public static implicit operator MyJson(double n)
    {
        return FromObject(n);
    }
    public static implicit operator double(MyJson d)
    {
        return (d == null) ? 0 : d.AsDouble;
    }
    #endregion Double

    #region Float
    public static implicit operator MyJson(float n)
    {
        return FromObject(n);
    }
    public static implicit operator float(MyJson d)
    {
        return (d == null) ? 0 : d.AsFloat;
    }
    #endregion Float

    #region Int
    public static implicit operator MyJson(int n)
    {
        return FromObject(n);
    }
    public static implicit operator int(MyJson d)
    {
        return (d == null) ? 0 : d.AsInt;
    }
    #endregion Int

    #region Long
    public static implicit operator MyJson(long n)
    {
        return FromObject(n);
    }
    public static implicit operator long(MyJson d)
    {
        return (d == null) ? 0L : d.AsLong;
    }
    #endregion Long

    #region ULong
    public static implicit operator MyJson(ulong n)
    {
        return FromObject(n);
    }
    public static implicit operator ulong(MyJson d)
    {
        return (d == null) ? 0 : d.AsULong;
    }
    #endregion ULong

    #region Bool
    public static implicit operator MyJson(bool b)
    {
        return FromObject(b);
    }
    public static implicit operator bool(MyJson d)
    {
        return (d == null) ? false : d.AsBool;
    }
    #endregion Bool

    #region Decimal
    public virtual decimal AsDecimal
    {
        get
        {
            decimal result;
            if (!decimal.TryParse(Value, out result))
                result = 0;
            return result;
        }
        set
        {
            Value = value.ToString();
        }
    }

    public static implicit operator MyJson(decimal aDecimal)
    {
        return FromObject(aDecimal);
    }

    public static implicit operator decimal(MyJson d)
    {
        return (d == null) ? 0 : d.AsDecimal;
    }
    #endregion Decimal

    #region UInt
    public virtual uint AsUInt
    {
        get
        {
            return (uint)AsDouble;
        }
        set
        {
            AsDouble = value;
        }
    }

    public static implicit operator MyJson(uint aUInt)
    {
#if false
        return new MyNumber(aUInt);
#else
        return new MyNumber((decimal)aUInt);
#endif
    }

    public static implicit operator uint(MyJson aNode)
    {
        return aNode.AsUInt;
    }
    #endregion UInt

    #region Byte
    public virtual byte AsByte
    {
        get
        {
            return (byte)AsInt;
        }
        set
        {
            AsInt = value;
        }
    }

    public static implicit operator MyJson(byte aByte)
    {
        return FromObject(aByte);
    }

    public static implicit operator byte(MyJson d)
    {
        return d == null ? (byte)0 : d.AsByte;
    }
    #endregion Byte
    #region SByte
    public virtual sbyte AsSByte
    {
        get
        {
            return (sbyte)AsInt;
        }
        set
        {
            AsInt = value;
        }
    }

    public static implicit operator MyJson(sbyte aSByte)
    {
        return FromObject(aSByte);
    }

    public static implicit operator sbyte(MyJson d)
    {
        return d == null ? (sbyte)0 : d.AsSByte;
    }
    #endregion SByte

    #region Short
    public virtual short AsShort
    {
        get
        {
            return (short)AsInt;
        }
        set
        {
            AsInt = value;
        }
    }

    public static implicit operator MyJson(short aShort)
    {
        return FromObject(aShort);
    }

    public static implicit operator short(MyJson d)
    {
        return d == null ? (short)0 : d.AsShort;
    }
    #endregion Short
    #region UShort
    public virtual ushort AsUShort
    {
        get
        {
            return (ushort)AsInt;
        }
        set
        {
            AsInt = value;
        }
    }

    public static implicit operator MyJson(ushort aUShort)
    {
        return FromObject(aUShort);
    }

    public static implicit operator ushort(MyJson d)
    {
        return d == null ? (ushort)0 : d.AsUShort;
    }
    #endregion UShort

    #region DateTime
    public virtual System.DateTime AsDateTime
    {
        get
        {
            System.DateTime result;
            if (!System.DateTime.TryParse(Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                result = new System.DateTime(0);
            return result;
        }
        set
        {
            //Value = value.ToString(CultureInfo.InvariantCulture);
            Value = Util.DateTimeString(value);
        }
    }

    public static implicit operator MyJson(System.DateTime aDateTime)
    {
        return new MyString(Util.DateTimeString(aDateTime));
    }

    public static implicit operator System.DateTime(MyJson aNode)
    {
        return aNode.AsDateTime;
    }
    #endregion DateTime
    #region TimeSpan
    public virtual System.TimeSpan AsTimeSpan
    {
        get
        {
            System.TimeSpan result;
            if (!System.TimeSpan.TryParse(Value, CultureInfo.InvariantCulture, out result))
                result = new System.TimeSpan(0);
            return result;
        }
        set
        {
            Value = value.ToString();
        }
    }

    public static implicit operator MyJson(System.TimeSpan aTimeSpan)
    {
        return FromObject(aTimeSpan);
    }

    public static implicit operator System.TimeSpan(MyJson d)
    {
        return d == null ? new TimeSpan(0) : d.AsTimeSpan;
    }
    #endregion TimeSpan

    #region Guid
    public virtual System.Guid AsGuid
    {
        get
        {
            System.Guid result;
            System.Guid.TryParse(Value, out result);
            return result;
        }
        set
        {
            Value = value.ToString();
        }
    }

    public static implicit operator MyJson(System.Guid aGuid)
    {
        return FromObject(aGuid);
    }

    public static implicit operator System.Guid(MyJson d)
    {
        return d == null ? Guid.Empty : d.AsGuid;
    }
    #endregion Guid

    #region ByteArray
    public virtual byte[] AsByteArray
    {
        get
        {
            if (this.IsNull || !this.IsArray)
                return null;
            int count = Count;
            byte[] result = new byte[count];
            for (int i = 0; i < count; i++)
                result[i] = this[i].AsByte;
            return result;
        }
        set
        {
            if (!IsArray || value == null)
                return;
            Clear();
            for (int i = 0; i < value.Length; i++)
                Add(value[i]);
        }
    }

    public static implicit operator MyJson(byte[] aByteArray)
    {
        return FromObject(aByteArray);
    }

    public static implicit operator byte[](MyJson d)
    {
        return d == null ? null : d.AsByteArray;
    }
    #endregion ByteArray
    #region ByteList
    public virtual List<byte> AsByteList
    {
        get
        {
            if (this.IsNull || !this.IsArray)
                return null;
            int count = Count;
            List<byte> result = new List<byte>(count);
            for (int i = 0; i < count; i++)
                result.Add(this[i].AsByte);
            return result;
        }
        set
        {
            if (!IsArray || value == null)
                return;
            Clear();
            for (int i = 0; i < value.Count; i++)
                Add(value[i]);
        }
    }

    public static implicit operator MyJson(List<byte> aByteList)
    {
        return FromObject(aByteList);
    }

    public static implicit operator List<byte>(MyJson d)
    {
        return d == null ? null : d.AsByteList;
    }
    #endregion ByteList

    #region StringArray
    public virtual string[] AsStringArray
    {
        get
        {
            if (this.IsNull || !this.IsArray)
                return null;
            int count = Count;
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
                result[i] = this[i].Value;
            return result;
        }
        set
        {
            if (!IsArray || value == null)
                return;
            Clear();
            for (int i = 0; i < value.Length; i++)
                Add(value[i]);
        }
    }

    public static implicit operator MyJson(string[] aStringArray)
    {
        return FromObject(aStringArray);
    }

    public static implicit operator string[](MyJson d)
    {
        return d == null ? null : d.AsStringArray;
    }
    #endregion StringArray
    #region StringList
    public virtual List<string> AsStringList
    {
        get
        {
            if (this.IsNull || !this.IsArray)
                return null;
            int count = Count;
            List<string> result = new List<string>(count);
            for (int i = 0; i < count; i++)
                result.Add(this[i].Value);
            return result;
        }
        set
        {
            if (!IsArray || value == null)
                return;
            Clear();
            for (int i = 0; i < value.Count; i++)
                Add(value[i]);
        }
    }

    public static implicit operator MyJson(List<string> aStringList)
    {
        return FromObject(aStringList);
    }

    public static implicit operator List<string>(MyJson d)
    {
        return d == null ? null : d.AsStringList;
    }
    #endregion StringList

    #region ==/!=
    public static bool operator ==(MyJson a, object b)
    {
        if (ReferenceEquals(a, b))
            return true;
        bool aIsNull = a is MyNull || ReferenceEquals(a, null); //|| a is MyLazyCreator;
        bool bIsNull = b is MyNull || ReferenceEquals(b, null); //|| b is MyLazyCreator;
        if (aIsNull && bIsNull)
            return true;
        return !aIsNull && a.Equals(b);
    }
    public static bool operator !=(MyJson a, object b)
    {
        return !(a == b);
    }
    #endregion ==/!=

    #region Equals()/GetHashCode()
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion Equals()/GetHashCode()

    #region Escape()
    [ThreadStatic]
    private StringBuilder m_EscapeBuilder;
    internal StringBuilder EscapeBuilder
    {
        get
        {
            if (m_EscapeBuilder == null)
                m_EscapeBuilder = new StringBuilder();
            return m_EscapeBuilder;
        }
    }
    internal string Escape(string aText)
    {
        var sb = EscapeBuilder;
        sb.Length = 0;
        if (sb.Capacity < aText.Length + aText.Length / 10)
            sb.Capacity = aText.Length + aText.Length / 10;
        foreach (char c in aText)
        {
            switch (c)
            {
                case '\\':
                    sb.Append("\\\\");
                    break;
                case '\"':
                    sb.Append("\\\"");
                    break;
                case '\n':
                    sb.Append("\\n");
                    break;
                case '\r':
                    sb.Append("\\r");
                    break;
                case '\t':
                    sb.Append("\\t");
                    break;
                case '\b':
                    sb.Append("\\b");
                    break;
                case '\f':
                    sb.Append("\\f");
                    break;
                default:
                    if (c < ' ' || (ForceASCII && c > 127))
                    {
                        ushort val = c;
                        sb.Append("\\u").Append(val.ToString("X4"));
                    }
                    else
                        sb.Append(c);
                    break;
            }
        }
        string result = sb.ToString();
        sb.Length = 0;
        return result;
    }
    #endregion Escape()

    #endregion operators

    #region FromString()
    public static MyJson FromString(string aJSON)
    {
        if (String.IsNullOrEmpty(aJSON)) return null;
        var inputStream = new AntlrInputStream(aJSON);
        var lexer = new JSON5Lexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new JSON5Parser(commonTokenStream);
        var context = parser.json5();
        return JSON5ToObject(context);
    }

    private static MyJson ParseJsonString(string aJSON)
    {
        Stack<MyJson> stack = new Stack<MyJson>();
        MyJson ctx = null;
        int i = 0;
        StringBuilder Token = new StringBuilder();
        string TokenName = "";
        bool QuoteMode = false;
        bool TokenIsQuoted = false;
        bool HasNewlineChar = false;
        while (i < aJSON.Length)
        {
            switch (aJSON[i])
            {

                case '"':
                    QuoteMode ^= true;
                    TokenIsQuoted |= QuoteMode;
                    break;

                case '\r':
                case '\n':
                    HasNewlineChar = true;
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
            throw new Exception("My Parse: Quotation marks seems to be messed up.");
        }
        return ctx;
    }

    private static MyJson JSON5ToObject(ParserRuleContext x)
    {
        if (x is JSON5Parser.Json5Context)
        {
            for (int i = 0; i < x.children.Count; i++)
            {
                if (x.children[i] is Antlr4.Runtime.Tree.ErrorNodeImpl)
                {
                    return null;
                }
            }

            return JSON5ToObject((ParserRuleContext)x.children[0]);
        }
        else if (x is JSON5Parser.ValueContext)
        {
            if (x.children[0] is Antlr4.Runtime.Tree.TerminalNodeImpl)
            {
                string t = JSON5Terminal(x.children[0])!;
                if (t.StartsWith("\""))
                {
                    return ParseJsonString(t);
                }

                if (t.StartsWith("'"))
                {
                    t = t.Substring(1, t.Length - 2).Replace("\\'", ",").Replace("\"", "\\\"");
                    t = "\"" + t + "\"";
                    return ParseJsonString(t);
                }

                switch (t)
                {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    case "null":
                        return null;
                }

                throw new Exception($"Unexpected JSON5Parser+ValueContext: {t}");
                //return t;
            }

            return JSON5ToObject((ParserRuleContext)x.children[0]);
        }
        else if (x is JSON5Parser.ArrContext)
        {
            var result = new MyArray();
            for (int i = 0; i < x.children.Count; i++)
            {
                if (x.children[i] is JSON5Parser.ValueContext value)
                {
                    result.Add(JSON5ToObject(value));
                }
            }

            return result;
        }
        else if (x is JSON5Parser.ObjContext)
        {
            var result = new MyObject();
            for (int i = 0; i < x.children.Count; i++)
            {
                if (x.children[i] is JSON5Parser.PairContext pair)
                {
                    var pairObj = JSON5ToObject(pair);
                    result[(string)pairObj!["key"]] = pairObj["value"];
                }
            }

            return result;
        }
        else if (x is JSON5Parser.PairContext)
        {
            var result = new MyObject();
            for (int i = 0; i < x.children.Count; i++)
            {
                if (x.children[i] is JSON5Parser.KeyContext key)
                {
                    result["key"] = JSON5ToObject(key);
                }

                if (x.children[i] is JSON5Parser.ValueContext value)
                {
                    result["value"] = JSON5ToObject(value);
                }
            }

            return result;
        }
        else if (x is JSON5Parser.KeyContext)
        {
            if (x.children[0] is Antlr4.Runtime.Tree.TerminalNodeImpl)
            {
                string t = JSON5Terminal(x.children[0])!;
                if (t.StartsWith("\""))
                {
                    return ParseJsonString(t);
                }

                if (t.StartsWith("'"))
                {
                    t = t.Substring(1, t.Length - 2).Replace("\\'", ",").Replace("\"", "\\\"");
                    t = "\"" + t + "\"";
                    return ParseJsonString(t);
                }

                return t;
            }
            else
            {
                return "?";
            }
        }
        else if (x is JSON5Parser.NumberContext)
        {
            string n = JSON5Terminal(x.children[0]);
            if (n == "-" || n == "+")
            {
                string sign = n;
                n = sign + JSON5Terminal(x.children[1]);
            }
            decimal result;
            if (!decimal.TryParse(n, out result))
                result = 0;
            if (NumberAsDecimal)
                return new MyNumber(result);
            return new MyNumber(Convert.ToDouble(result));
        }
        else
        {
            throw new Exception($"Unexpected: {x.GetType().FullName}");
        }
    }

    private static string? JSON5Terminal(Antlr4.Runtime.Tree.IParseTree x)
    {
        if (x is Antlr4.Runtime.Tree.TerminalNodeImpl t)
        {
            return t.ToString();
        }

        return null;
    }
    #endregion FromString()

    #region FromObject()
    //public static MyJson FromObject(object item, bool display = false)
    public static MyJson FromObject(object item)
    {
        if (item == null)
        {
            return MyNull.CreateOrGet();
        }

        Type type = item.GetType();
        if (type == typeof(string) || type == typeof(char))
        {
            string str = item.ToString();
            return new MyString(str);
        }
        else if (type == typeof(byte) || type == typeof(sbyte))
        {
            //return FromString(item.ToString());
            return new MyNumber(item);
        }
        else if (type == typeof(short) || type == typeof(ushort))
        {
            //return FromString(item.ToString());
            return new MyNumber(item);
        }
        else if (type == typeof(int) || type == typeof(uint))
        {
            //return FromString(item.ToString());
            return new MyNumber(item);
        }
        else if (type == typeof(long) || type == typeof(ulong))
        {
            //return FromString(item.ToString());
            return new MyNumber(item);
        }
        else if (type == typeof(float))
        {
            //return new MyNumber(Convert.ToDecimal(item));
            return new MyNumber(item);
        }
        else if (type == typeof(double))
        {
            //return new MyNumber(Convert.ToDecimal(item));
            return new MyNumber(item);
        }
        else if (type == typeof(decimal))
        {
#if false
            if (!display && DecimalAsString)
                return new MyString(item.ToString());
#endif
            //return new MyNumber((decimal)item);
            return new MyNumber(item);
        }
        else if (type == typeof(bool))
        {
            return new MyBool((bool)item);
        }
        else if (type == typeof(DateTime))
        {
            return new MyString(Util.DateTimeString((DateTime)item));
        }
        else if (type == typeof(TimeSpan))
        {
            return new MyString(item.ToString());
        }
        else if (type == typeof(Guid))
        {
            return new MyString(item.ToString());
        }
#if false
        else if (type == typeof(byte[]))
        {
            return new MyArray { AsByteArray = (byte[])item };
        }
        else if (type == typeof(List<byte>))
        {
            return new MyArray { AsByteList = (List<byte>)item };
        }
#endif
        else if (type.IsEnum)
        {
            return new MyString(item.ToString());
        }
        else if (item is IList)
        {
            IList list = item as IList;
            var result = new MyArray();
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(FromObject(list[i]));
            }
            return result;
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Type keyType = type.GetGenericArguments()[0];
            var result = new MyObject();
            //Refuse to output dictionary keys that aren't of type string
            if (keyType != typeof(string))
            {
                return result;
            }
            IDictionary dict = item as IDictionary;
            foreach (object key in dict.Keys)
            {
                result[(string)key] = FromObject(dict[key]);
            }
            return result;
        }
        else
        {
            Type keyType = type.GetGenericArguments()[0];
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var result = new MyObject();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (fieldInfos[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;
                object value = fieldInfos[i].GetValue(item);
                result[GetMemberName(fieldInfos[i])] = FromObject(value);
            }
            PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                if (!propertyInfo[i].CanRead || propertyInfo[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;
                object value = propertyInfo[i].GetValue(item, null);
                result[GetMemberName(propertyInfo[i])] = FromObject(value);
            }
            return result;
        }
    }

    static string GetMemberName(MemberInfo member)
    {
        if (member.IsDefined(typeof(DataMemberAttribute), true))
        {
            DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
            if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                return dataMemberAttribute.Name;
        }

        return member.Name;
    }
#endregion FromObject()

}
// End of MyJson
#endregion MyJson

#region MyArray
public partial class MyArray : MyJson
{
    private List<MyJson> m_List = new List<MyJson>();
    private bool inline = false;
    public override bool Inline
    {
        get { return inline; }
        set { inline = value; }
    }

    public override MyNodeType Tag { get { return MyNodeType.Array; } }
    public override bool IsArray { get { return true; } }
    public override Enumerator GetEnumerator() { return new Enumerator(m_List.GetEnumerator()); }

    public override MyJson this[int aIndex]
    {
        get
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return MyNull.CreateOrGet(); // new MyLazyCreator(this);
            return m_List[aIndex];
        }
        set
        {
            if (value == null)
                value = MyNull.CreateOrGet();
            if (aIndex < 0 || aIndex >= m_List.Count)
                m_List.Add(value);
            else
                m_List[aIndex] = value;
        }
    }

    public override MyJson this[string aKey]
    {
        get { return MyNull.CreateOrGet()/*new MyLazyCreator(this)*/; }
        set
        {
            if (value == null)
                value = MyNull.CreateOrGet();
            m_List.Add(value);
        }
    }

    public override int Count
    {
        get { return m_List.Count; }
    }

    public override void Add(string aKey, MyJson aItem)
    {
        if (aItem == null)
            aItem = MyNull.CreateOrGet();
        m_List.Add(aItem);
    }

    public override MyJson Remove(int aIndex)
    {
        if (aIndex < 0 || aIndex >= m_List.Count)
            return null;
        MyJson tmp = m_List[aIndex];
        m_List.RemoveAt(aIndex);
        return tmp;
    }

    public override MyJson Remove(MyJson aNode)
    {
        m_List.Remove(aNode);
        return aNode;
    }

    public override void Clear()
    {
        m_List.Clear();
    }

    public override MyJson Clone()
    {
        var node = new MyArray();
        node.m_List.Capacity = m_List.Capacity;
        foreach (var n in m_List)
        {
            if (n != null)
                node.Add(n.Clone());
            else
                node.Add(null);
        }
        return node;
    }

    public override IEnumerable<MyJson> Children
    {
        get
        {
            foreach (MyJson N in m_List)
                yield return N;
        }
    }


    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append('[');
        int count = m_List.Count;
#if false
        if (inline)
            aMode = MyTextMode.Compact;
#endif
        for (int i = 0; i < count; i++)
        {
            if (i > 0)
                aSB.Append(',');
            if (aMode == MyTextMode.Indent)
                aSB.AppendLine();

            if (aMode == MyTextMode.Indent)
                aSB.Append(' ', aIndent + aIndentInc);
            m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
        }
        if (aMode == MyTextMode.Indent)
            aSB.AppendLine().Append(' ', aIndent);
        aSB.Append(']');
    }
}
// End of MyArray
#endregion MyArray

#region MyObject
public partial class MyObject : MyJson
{
    private Dictionary<string, MyJson> m_Dict = new Dictionary<string, MyJson>();

    private bool inline = false;
    public override bool Inline
    {
        get { return inline; }
        set { inline = value; }
    }

    public override MyNodeType Tag { get { return MyNodeType.Object; } }
    public override bool IsObject { get { return true; } }

    public override Enumerator GetEnumerator() { return new Enumerator(m_Dict.GetEnumerator()); }


    public override MyJson this[string aKey]
    {
        get
        {
            if (m_Dict.ContainsKey(aKey))
                return m_Dict[aKey];
            else
                return MyNull.CreateOrGet()/*new MyLazyCreator(this, aKey)*/;
        }
        set
        {
            if (value == null)
                value = MyNull.CreateOrGet();
            if (m_Dict.ContainsKey(aKey))
                m_Dict[aKey] = value;
            else
                m_Dict.Add(aKey, value);
        }
    }

    public override MyJson this[int aIndex]
    {
        get
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            return m_Dict.ElementAt(aIndex).Value;
        }
        set
        {
            if (value == null)
                value = MyNull.CreateOrGet();
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return;
            string key = m_Dict.ElementAt(aIndex).Key;
            m_Dict[key] = value;
        }
    }

    public override int Count
    {
        get { return m_Dict.Count; }
    }

    public override void Add(string aKey, MyJson aItem)
    {
        if (aItem == null)
            aItem = MyNull.CreateOrGet();

        if (aKey != null)
        {
            if (m_Dict.ContainsKey(aKey))
                m_Dict[aKey] = aItem;
            else
                m_Dict.Add(aKey, aItem);
        }
        else
            m_Dict.Add(Guid.NewGuid().ToString(), aItem);
    }

    public override MyJson Remove(string aKey)
    {
        if (!m_Dict.ContainsKey(aKey))
            return null;
        MyJson tmp = m_Dict[aKey];
        m_Dict.Remove(aKey);
        return tmp;
    }

    public override MyJson Remove(int aIndex)
    {
        if (aIndex < 0 || aIndex >= m_Dict.Count)
            return null;
        var item = m_Dict.ElementAt(aIndex);
        m_Dict.Remove(item.Key);
        return item.Value;
    }

    public override MyJson Remove(MyJson aNode)
    {
        try
        {
            var item = m_Dict.Where(k => k.Value == aNode).First();
            m_Dict.Remove(item.Key);
            return aNode;
        }
        catch
        {
            return null;
        }
    }

    public override void Clear()
    {
        m_Dict.Clear();
    }

    public override MyJson Clone()
    {
        var node = new MyObject();
        foreach (var n in m_Dict)
        {
            node.Add(n.Key, n.Value.Clone());
        }
        return node;
    }

    public override bool HasKey(string aKey)
    {
        return m_Dict.ContainsKey(aKey);
    }

    public override MyJson GetValueOrDefault(string aKey, MyJson aDefault)
    {
        MyJson res;
        if (m_Dict.TryGetValue(aKey, out res))
            return res;
        return aDefault;
    }

    public override IEnumerable<MyJson> Children
    {
        get
        {
            foreach (KeyValuePair<string, MyJson> N in m_Dict)
                yield return N.Value;
        }
    }

    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append('{');
        bool first = true;
#if false
        if (inline)
            aMode = MyTextMode.Compact;
#endif
        foreach (var k in m_Dict)
        {
            if (!first)
                aSB.Append(',');
            first = false;
            if (aMode == MyTextMode.Indent)
                aSB.AppendLine();
            if (aMode == MyTextMode.Indent)
                aSB.Append(' ', aIndent + aIndentInc);
            aSB.Append('\"').Append(Escape(k.Key)).Append('\"');
            if (aMode == MyTextMode.Compact)
                aSB.Append(':');
            else
                aSB.Append(" : ");
            k.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
        }
        if (aMode == MyTextMode.Indent)
            aSB.AppendLine().Append(' ', aIndent);
        aSB.Append('}');
    }

}
// End of MyObject
#endregion MyObject

#region MyString
public partial class MyString : MyJson
{
    private string m_Data;

    public override MyNodeType Tag { get { return MyNodeType.String; } }
    public override bool IsString { get { return true; } }

    public override Enumerator GetEnumerator() { return new Enumerator(); }


    public override string Value
    {
        get { return m_Data; }
        set
        {
            m_Data = value;
        }
    }

    public MyString(string aData)
    {
        m_Data = aData;
    }
    public override MyJson Clone()
    {
        return new MyString(m_Data);
    }

    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append('\"').Append(Escape(m_Data)).Append('\"');
    }
    public override bool Equals(object obj)
    {
        if (base.Equals(obj))
            return true;
        string s = obj as string;
        if (s != null)
            return m_Data == s;
        MyString s2 = obj as MyString;
        if (s2 != null)
            return m_Data == s2.m_Data;
        return false;
    }
    public override int GetHashCode()
    {
        return m_Data.GetHashCode();
    }
    public override void Clear()
    {
        m_Data = "";
    }
}
// End of MyString
#endregion MyString

#region MyNumber
public partial class MyNumber : MyJson
{
#if false
    decimal m_Data;
#else
    object m_Data;
#endif

    public override MyNodeType Tag { get { return MyNodeType.Number; } }
    public override bool IsNumber { get { return true; } }
    public override Enumerator GetEnumerator() { return new Enumerator(); }

    public override string Value
    {
        get
        {
            //return m_Data.ToString(CultureInfo.InvariantCulture);
            return m_Data.ToString();
        }
        set
        {
#if false
            double v;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                m_Data = v;
#else
            decimal v;
            if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                m_Data = v;
#endif
        }
    }

    public override double AsDouble
    {
#if false
        get { return m_Data; }
        set { m_Data = value; }
#else
        get
        {
            return Convert.ToDouble(m_Data);
        }
        set
        {
            //m_Data = Convert.ToDecimal(value);
            m_Data = value;
        }
#endif
    }
    public override long AsLong
    {
        get { return (long)Convert.ToDecimal(m_Data); }
        set { m_Data = value; }
    }
    public override ulong AsULong
    {
        get { return (ulong)Convert.ToDecimal(m_Data); }
        set { m_Data = value; }
    }

#if false
#else
    //public MyNumber(decimal aData)
    public MyNumber(object aData)
    {
        m_Data = aData;
    }
#endif

#if false
    public MyNumber(double aData)
    {
#if false
        m_Data = aData;
#else
        m_Data = Convert.ToDecimal(aData);
#endif
    }
#endif

    public MyNumber(string aData)
    {
        Value = aData;
    }

    public override MyJson Clone()
    {
        return new MyNumber(m_Data);
    }

    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append(Value.ToString(CultureInfo.InvariantCulture));
    }
    private static bool IsNumeric(object value)
    {
        return value is int || value is uint
            || value is float || value is double
            || value is decimal
            || value is long || value is ulong
            || value is short || value is ushort
            || value is sbyte || value is byte;
    }
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (base.Equals(obj))
            return true;
        MyNumber s2 = obj as MyNumber;
        if (s2 != null)
            return m_Data == s2.m_Data;
#if false
        if (IsNumeric(obj))
            return Convert.ToDouble(obj) == m_Data;
#else
        if (IsNumeric(obj))
            return Convert.ToDecimal(obj) == Convert.ToDecimal(m_Data);
#endif
        return false;
    }
    public override int GetHashCode()
    {
        return m_Data.GetHashCode();
    }
    public override void Clear()
    {
        m_Data = 0;
    }
}
// End of MyNumber
#endregion MyNumber

#region MyBool
public partial class MyBool : MyJson
{
    private bool m_Data;

    public override MyNodeType Tag { get { return MyNodeType.Boolean; } }
    public override bool IsBoolean { get { return true; } }
    public override Enumerator GetEnumerator() { return new Enumerator(); }

    public override string Value
    {
        get { return m_Data.ToString(); }
        set
        {
            bool v;
            if (bool.TryParse(value, out v))
                m_Data = v;
        }
    }
    public override bool AsBool
    {
        get { return m_Data; }
        set { m_Data = value; }
    }

    public MyBool(bool aData)
    {
        m_Data = aData;
    }

    public MyBool(string aData)
    {
        Value = aData;
    }

    public override MyJson Clone()
    {
        return new MyBool(m_Data);
    }

    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append((m_Data) ? "true" : "false");
    }
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (obj is bool)
            return m_Data == (bool)obj;
        return false;
    }
    public override int GetHashCode()
    {
        return m_Data.GetHashCode();
    }
    public override void Clear()
    {
        m_Data = false;
    }
}
// End of MyBool
#endregion MyBool

#region MyNull
public partial class MyNull : MyJson
{
    static MyNull m_StaticInstance = new MyNull();
    public static bool reuseSameInstance = true;
    public static MyNull CreateOrGet()
    {
        if (reuseSameInstance)
            return m_StaticInstance;
        return new MyNull();
    }
    private MyNull() { }

    public override MyNodeType Tag { get { return MyNodeType.NullValue; } }
    public override bool IsNull { get { return true; } }
    public override Enumerator GetEnumerator() { return new Enumerator(); }

    public override string Value
    {
        get { return "null"; }
        set { }
    }
    public override bool AsBool
    {
        get { return false; }
        set { }
    }

    public override MyJson Clone()
    {
        return CreateOrGet();
    }

    public override bool Equals(object obj)
    {
        if (object.ReferenceEquals(this, obj))
            return true;
        return (obj is MyNull);
    }
    public override int GetHashCode()
    {
        return 0;
    }

    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append("null");
    }
}
// End of MyNull
#endregion MyNull
