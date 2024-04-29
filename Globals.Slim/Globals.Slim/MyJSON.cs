#region License and information
#endregion License and information

using Antlr4.Runtime;
using Globals.Parser.Json5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Globals;

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

public abstract partial class MyJSON
{
    #region Enumerators
    public struct Enumerator
    {
        private enum Type { None, Array, Object }
        private Type type;
        private Dictionary<string, MyJSON>.Enumerator m_Object;
        private List<MyJSON>.Enumerator m_Array;
        public bool IsValid { get { return type != Type.None; } }
        public Enumerator(List<MyJSON>.Enumerator aArrayEnum)
        {
            type = Type.Array;
            m_Object = default(Dictionary<string, MyJSON>.Enumerator);
            m_Array = aArrayEnum;
        }
        public Enumerator(Dictionary<string, MyJSON>.Enumerator aDictEnum)
        {
            type = Type.Object;
            m_Object = aDictEnum;
            m_Array = default(List<MyJSON>.Enumerator);
        }
        public KeyValuePair<string, MyJSON> Current
        {
            get
            {
                if (type == Type.Array)
                    return new KeyValuePair<string, MyJSON>(string.Empty, m_Array.Current);
                else if (type == Type.Object)
                    return m_Object.Current;
                return new KeyValuePair<string, MyJSON>(string.Empty, null);
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
        public ValueEnumerator(List<MyJSON>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
        public ValueEnumerator(Dictionary<string, MyJSON>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
        public ValueEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
        public MyJSON Current { get { return m_Enumerator.Current.Value; } }
        public bool MoveNext() { return m_Enumerator.MoveNext(); }
        public ValueEnumerator GetEnumerator() { return this; }
    }
    public struct KeyEnumerator
    {
        private Enumerator m_Enumerator;
        public KeyEnumerator(List<MyJSON>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
        public KeyEnumerator(Dictionary<string, MyJSON>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
        public KeyEnumerator(Enumerator aEnumerator) { m_Enumerator = aEnumerator; }
        public string Current { get { return m_Enumerator.Current.Key; } }
        public bool MoveNext() { return m_Enumerator.MoveNext(); }
        public KeyEnumerator GetEnumerator() { return this; }
    }

    public class LinqEnumerator : IEnumerator<KeyValuePair<string, MyJSON>>, IEnumerable<KeyValuePair<string, MyJSON>>
    {
        private MyJSON m_Node;
        private Enumerator m_Enumerator;
        internal LinqEnumerator(MyJSON aNode)
        {
            m_Node = aNode;
            if (m_Node != null)
                m_Enumerator = m_Node.GetEnumerator();
        }
        public KeyValuePair<string, MyJSON> Current { get { return m_Enumerator.Current; } }
        object IEnumerator.Current { get { return m_Enumerator.Current; } }
        public bool MoveNext() { return m_Enumerator.MoveNext(); }

        public void Dispose()
        {
            m_Node = null;
            m_Enumerator = new Enumerator();
        }

        public IEnumerator<KeyValuePair<string, MyJSON>> GetEnumerator()
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

    public static bool forceASCII = false; // Use Unicode by default
#if false
    public static bool longAsString = false; // lazy creator creates a MyString instead of MyNumber
#endif
    public static bool allowLineComments = true; // allow "//"-style comments at the end of a line

    public abstract MyNodeType Tag { get; }

    public virtual MyJSON this[int aIndex] { get { return null; } set { } }

    public virtual MyJSON this[string aKey] { get { return null; } set { } }

    public virtual string Value { get { return ""; } set { } }

    public virtual int Count { get { return 0; } }

    public virtual bool IsNumber { get { return false; } }
    public virtual bool IsString { get { return false; } }
    public virtual bool IsBoolean { get { return false; } }
    public virtual bool IsNull { get { return false; } }
    public virtual bool IsArray { get { return false; } }
    public virtual bool IsObject { get { return false; } }

    public virtual bool Inline { get { return false; } set { } }

    public virtual void Add(string aKey, MyJSON aItem)
    {
    }
    public virtual void Add(MyJSON aItem)
    {
        Add("", aItem);
    }

    public virtual MyJSON Remove(string aKey)
    {
        return null;
    }

    public virtual MyJSON Remove(int aIndex)
    {
        return null;
    }

    public virtual MyJSON Remove(MyJSON aNode)
    {
        return aNode;
    }
    public virtual void Clear() { }

    public virtual MyJSON Clone()
    {
        return null;
    }

    public virtual IEnumerable<MyJSON> Children
    {
        get
        {
            yield break;
        }
    }

    public IEnumerable<MyJSON> DeepChildren
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

    public virtual MyJSON GetValueOrDefault(string aKey, MyJSON aDefault)
    {
        return aDefault;
    }

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
    internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode);

    public abstract Enumerator GetEnumerator();
    public IEnumerable<KeyValuePair<string, MyJSON>> Linq { get { return new LinqEnumerator(this); } }
    public KeyEnumerator Keys { get { return new KeyEnumerator(GetEnumerator()); } }
    public ValueEnumerator Values { get { return new ValueEnumerator(GetEnumerator()); } }

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

    public static implicit operator MyJSON(string s)
    {
        return (s == null) ? (MyJSON)MyNull.CreateOrGet() : new MyString(s);
    }
    public static implicit operator string(MyJSON d)
    {
        return (d == null) ? null : d.Value;
    }

    public static implicit operator MyJSON(double n)
    {
#if false
        return new MyNumber(n);
#else
        return new MyNumber((decimal)n);
#endif
    }
    public static implicit operator double(MyJSON d)
    {
        return (d == null) ? 0 : d.AsDouble;
    }

    public static implicit operator MyJSON(float n)
    {
#if false
        return new MyNumber(n);
#else
        return new MyNumber((decimal)n);
#endif
    }
    public static implicit operator float(MyJSON d)
    {
        return (d == null) ? 0 : d.AsFloat;
    }

    public static implicit operator MyJSON(int n)
    {
#if false
        return new MyNumber(n);
#else
        return new MyNumber((decimal)n);
#endif
    }
    public static implicit operator int(MyJSON d)
    {
        return (d == null) ? 0 : d.AsInt;
    }

    public static implicit operator MyJSON(long n)
    {
#if false
        if (longAsString)
            return new MyString(n.ToString(CultureInfo.InvariantCulture));
        return new MyNumber(n);
#else
        return new MyNumber((decimal)n);
#endif
    }
    public static implicit operator long(MyJSON d)
    {
        return (d == null) ? 0L : d.AsLong;
    }

    public static implicit operator MyJSON(ulong n)
    {
#if false
        if (longAsString)
            return new MyString(n.ToString(CultureInfo.InvariantCulture));
        return new MyNumber(n);
#else
        return new MyNumber((decimal)n);
#endif
    }
    public static implicit operator ulong(MyJSON d)
    {
        return (d == null) ? 0 : d.AsULong;
    }

    public static implicit operator MyJSON(bool b)
    {
        return new MyBool(b);
    }
    public static implicit operator bool(MyJSON d)
    {
        return (d == null) ? false : d.AsBool;
    }

    public static implicit operator MyJSON(KeyValuePair<string, MyJSON> aKeyValue)
    {
        return aKeyValue.Value;
    }

    public static bool operator ==(MyJSON a, object b)
    {
        if (ReferenceEquals(a, b))
            return true;
        bool aIsNull = a is MyNull || ReferenceEquals(a, null) || a is MyLazyCreator;
        bool bIsNull = b is MyNull || ReferenceEquals(b, null) || b is MyLazyCreator;
        if (aIsNull && bIsNull)
            return true;
        return !aIsNull && a.Equals(b);
    }

    public static bool operator !=(MyJSON a, object b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

#endregion operators

    [ThreadStatic]
    private static StringBuilder m_EscapeBuilder;
    internal static StringBuilder EscapeBuilder
    {
        get
        {
            if (m_EscapeBuilder == null)
                m_EscapeBuilder = new StringBuilder();
            return m_EscapeBuilder;
        }
    }
    internal static string Escape(string aText)
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
                    if (c < ' ' || (forceASCII && c > 127))
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

    public static MyJSON Parse(string aJSON)
    {
        if (String.IsNullOrEmpty(aJSON)) return null;
        var inputStream = new AntlrInputStream(aJSON);
        var lexer = new JSON5Lexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new JSON5Parser(commonTokenStream);
        var context = parser.json5();
        return JSON5ToObject(context);
    }
    private static MyJSON ParseElement(string token, bool quoted)
    {
        if (quoted)
            return token;
        if (token.Length <= 5)
        {
            string tmp = token.ToLower();
            if (tmp == "false" || tmp == "true")
                return tmp == "true";
            if (tmp == "null")
                return MyNull.CreateOrGet();
        }
        double val;
        if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
            return val;
        else
            return token;
    }

    private static MyJSON ParseAtom(string aJSON)
    {
        Stack<MyJSON> stack = new Stack<MyJSON>();
        MyJSON ctx = null;
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
                case '{':
                    if (QuoteMode)
                    {
                        Token.Append(aJSON[i]);
                        break;
                    }
                    stack.Push(new MyObject());
                    if (ctx != null)
                    {
                        ctx.Add(TokenName, stack.Peek());
                    }
                    TokenName = "";
                    Token.Length = 0;
                    ctx = stack.Peek();
                    HasNewlineChar = false;
                    break;

                case '[':
                    if (QuoteMode)
                    {
                        Token.Append(aJSON[i]);
                        break;
                    }

                    stack.Push(new MyArray());
                    if (ctx != null)
                    {
                        ctx.Add(TokenName, stack.Peek());
                    }
                    TokenName = "";
                    Token.Length = 0;
                    ctx = stack.Peek();
                    HasNewlineChar = false;
                    break;

                case '}':
                case ']':
                    if (QuoteMode)
                    {

                        Token.Append(aJSON[i]);
                        break;
                    }
                    if (stack.Count == 0)
                        throw new Exception("My Parse: Too many closing brackets");

                    stack.Pop();
                    if (Token.Length > 0 || TokenIsQuoted)
                        ctx.Add(TokenName, ParseElement(Token.ToString(), TokenIsQuoted));
                    if (ctx != null)
                        ctx.Inline = !HasNewlineChar;
                    TokenIsQuoted = false;
                    TokenName = "";
                    Token.Length = 0;
                    if (stack.Count > 0)
                        ctx = stack.Peek();
                    break;

                case ':':
                    if (QuoteMode)
                    {
                        Token.Append(aJSON[i]);
                        break;
                    }
                    TokenName = Token.ToString();
                    Token.Length = 0;
                    TokenIsQuoted = false;
                    break;

                case '"':
                    QuoteMode ^= true;
                    TokenIsQuoted |= QuoteMode;
                    break;

                case ',':
                    if (QuoteMode)
                    {
                        Token.Append(aJSON[i]);
                        break;
                    }
                    if (Token.Length > 0 || TokenIsQuoted)
                        ctx.Add(TokenName, ParseElement(Token.ToString(), TokenIsQuoted));
                    TokenIsQuoted = false;
                    TokenName = "";
                    Token.Length = 0;
                    TokenIsQuoted = false;
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
                case '/':
                    if (allowLineComments && !QuoteMode && i + 1 < aJSON.Length && aJSON[i + 1] == '/')
                    {
                        while (++i < aJSON.Length && aJSON[i] != '\n' && aJSON[i] != '\r') ;
                        break;
                    }
                    Token.Append(aJSON[i]);
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
        if (ctx == null)
            return ParseElement(Token.ToString(), TokenIsQuoted);
        return ctx;
    }

    private static MyJSON JSON5ToObject(ParserRuleContext x)
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
                    return ParseAtom(t);
                }

                if (t.StartsWith("'"))
                {
                    t = t.Substring(1, t.Length - 2).Replace("\\'", ",").Replace("\"", "\\\"");
                    t = "\"" + t + "\"";
                    return ParseAtom(t);
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
                    return ParseAtom(t);
                }

                if (t.StartsWith("'"))
                {
                    t = t.Substring(1, t.Length - 2).Replace("\\'", ",").Replace("\"", "\\\"");
                    t = "\"" + t + "\"";
                    return ParseAtom(t);
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
            return new MyNumber(JSON5Terminal(x.children[0]));
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

}
// End of MyNode

public partial class MyArray : MyJSON
{
    private List<MyJSON> m_List = new List<MyJSON>();
    private bool inline = false;
    public override bool Inline
    {
        get { return inline; }
        set { inline = value; }
    }

    public override MyNodeType Tag { get { return MyNodeType.Array; } }
    public override bool IsArray { get { return true; } }
    public override Enumerator GetEnumerator() { return new Enumerator(m_List.GetEnumerator()); }

    public override MyJSON this[int aIndex]
    {
        get
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return new MyLazyCreator(this);
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

    public override MyJSON this[string aKey]
    {
        get { return new MyLazyCreator(this); }
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

    public override void Add(string aKey, MyJSON aItem)
    {
        if (aItem == null)
            aItem = MyNull.CreateOrGet();
        m_List.Add(aItem);
    }

    public override MyJSON Remove(int aIndex)
    {
        if (aIndex < 0 || aIndex >= m_List.Count)
            return null;
        MyJSON tmp = m_List[aIndex];
        m_List.RemoveAt(aIndex);
        return tmp;
    }

    public override MyJSON Remove(MyJSON aNode)
    {
        m_List.Remove(aNode);
        return aNode;
    }

    public override void Clear()
    {
        m_List.Clear();
    }

    public override MyJSON Clone()
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

    public override IEnumerable<MyJSON> Children
    {
        get
        {
            foreach (MyJSON N in m_List)
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

public partial class MyObject : MyJSON
{
    private Dictionary<string, MyJSON> m_Dict = new Dictionary<string, MyJSON>();

    private bool inline = false;
    public override bool Inline
    {
        get { return inline; }
        set { inline = value; }
    }

    public override MyNodeType Tag { get { return MyNodeType.Object; } }
    public override bool IsObject { get { return true; } }

    public override Enumerator GetEnumerator() { return new Enumerator(m_Dict.GetEnumerator()); }


    public override MyJSON this[string aKey]
    {
        get
        {
            if (m_Dict.ContainsKey(aKey))
                return m_Dict[aKey];
            else
                return new MyLazyCreator(this, aKey);
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

    public override MyJSON this[int aIndex]
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

    public override void Add(string aKey, MyJSON aItem)
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

    public override MyJSON Remove(string aKey)
    {
        if (!m_Dict.ContainsKey(aKey))
            return null;
        MyJSON tmp = m_Dict[aKey];
        m_Dict.Remove(aKey);
        return tmp;
    }

    public override MyJSON Remove(int aIndex)
    {
        if (aIndex < 0 || aIndex >= m_Dict.Count)
            return null;
        var item = m_Dict.ElementAt(aIndex);
        m_Dict.Remove(item.Key);
        return item.Value;
    }

    public override MyJSON Remove(MyJSON aNode)
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

    public override MyJSON Clone()
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

    public override MyJSON GetValueOrDefault(string aKey, MyJSON aDefault)
    {
        MyJSON res;
        if (m_Dict.TryGetValue(aKey, out res))
            return res;
        return aDefault;
    }

    public override IEnumerable<MyJSON> Children
    {
        get
        {
            foreach (KeyValuePair<string, MyJSON> N in m_Dict)
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

public partial class MyString : MyJSON
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
    public override MyJSON Clone()
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

public partial class MyNumber : MyJSON
{
#if false
    private double m_Data;
#else
    decimal m_Data;
#endif

    public override MyNodeType Tag { get { return MyNodeType.Number; } }
    public override bool IsNumber { get { return true; } }
    public override Enumerator GetEnumerator() { return new Enumerator(); }

    public override string Value
    {
        get { return m_Data.ToString(CultureInfo.InvariantCulture); }
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
        get { return decimal.ToDouble(m_Data); }
        set { m_Data = Convert.ToDecimal(value); }
#endif
    }
    public override long AsLong
    {
        get { return (long)m_Data; }
        set { m_Data = value; }
    }
    public override ulong AsULong
    {
        get { return (ulong)m_Data; }
        set { m_Data = value; }
    }

#if false
#else
    public MyNumber(decimal aData)
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

    public override MyJSON Clone()
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
            return Convert.ToDecimal(obj) == m_Data;
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

public partial class MyBool : MyJSON
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

    public override MyJSON Clone()
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

public partial class MyNull : MyJSON
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

    public override MyJSON Clone()
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

internal partial class MyLazyCreator : MyJSON
{
    private MyJSON m_Node = null;
    private string m_Key = null;
    public override MyNodeType Tag { get { return MyNodeType.None; } }
    public override Enumerator GetEnumerator() { return new Enumerator(); }

    public MyLazyCreator(MyJSON aNode)
    {
        m_Node = aNode;
        m_Key = null;
    }

    public MyLazyCreator(MyJSON aNode, string aKey)
    {
        m_Node = aNode;
        m_Key = aKey;
    }

    private T Set<T>(T aVal) where T : MyJSON
    {
        if (m_Key == null)
            m_Node.Add(aVal);
        else
            m_Node.Add(m_Key, aVal);
        m_Node = null; // Be GC friendly.
        return aVal;
    }

    public override MyJSON this[int aIndex]
    {
        get { return new MyLazyCreator(this); }
        set { Set(new MyArray()).Add(value); }
    }

    public override MyJSON this[string aKey]
    {
        get { return new MyLazyCreator(this, aKey); }
        set { Set(new MyObject()).Add(aKey, value); }
    }

    public override void Add(MyJSON aItem)
    {
        Set(new MyArray()).Add(aItem);
    }

    public override void Add(string aKey, MyJSON aItem)
    {
        Set(new MyObject()).Add(aKey, aItem);
    }

    public static bool operator ==(MyLazyCreator a, object b)
    {
        if (b == null)
            return true;
        return System.Object.ReferenceEquals(a, b);
    }

    public static bool operator !=(MyLazyCreator a, object b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return true;
        return System.Object.ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override int AsInt
    {
#if false
        get { Set(new MyNumber(0)); return 0; }
        set { Set(new MyNumber(value)); }
#else
        get { Set(new MyNumber((decimal)0)); return 0; }
        set { Set(new MyNumber(Convert.ToDecimal(value))); }
#endif
    }

    public override float AsFloat
    {
#if false
        get { Set(new MyNumber(0.0f)); return 0.0f; }
        set { Set(new MyNumber(value)); }
#else
        get { Set(new MyNumber(0.0m)); return 0.0f; }
        set { Set(new MyNumber(Convert.ToDecimal(value))); }
#endif
    }

    public override double AsDouble
    {
#if false
        get { Set(new MyNumber(0.0)); return 0.0; }
        set { Set(new MyNumber(value)); }
#else
        get { Set(new MyNumber(0.0m)); return 0.0; }
        set { Set(new MyNumber(Convert.ToDecimal(value))); }
#endif
    }

    public override long AsLong
    {
        get
        {
#if false
            if (longAsString)
                Set(new MyString("0"));
            else
                Set(new MyNumber(0.0));
            return 0L;
#else
            Set(new MyNumber(0.0m));
            return 0L;
#endif
        }
        set
        {
#if false
            if (longAsString)
                Set(new MyString(value.ToString(CultureInfo.InvariantCulture)));
            else
                Set(new MyNumber(value));
#else
            Set(new MyNumber(Convert.ToDecimal(value)));
#endif
        }
    }

    public override ulong AsULong
    {
        get
        {
#if false
            if (longAsString)
                Set(new MyString("0"));
            else
                Set(new MyNumber(0.0));
            return 0L;
#else
            Set(new MyNumber(0.0m));
            return 0L;
#endif
        }
        set
        {
#if false
            if (longAsString)
                Set(new MyString(value.ToString(CultureInfo.InvariantCulture)));
            else
                Set(new MyNumber(value));
#else
            Set(new MyNumber(Convert.ToDecimal(value)));
#endif
        }
    }

    public override bool AsBool
    {
        get { Set(new MyBool(false)); return false; }
        set { Set(new MyBool(value)); }
    }

    public override MyArray AsArray
    {
        get { return Set(new MyArray()); }
    }

    public override MyObject AsObject
    {
        get { return Set(new MyObject()); }
    }
    internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, MyTextMode aMode)
    {
        aSB.Append("null");
    }
}
// End of MyLazyCreator
