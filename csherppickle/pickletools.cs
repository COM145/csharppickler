using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace csherppickle
{
    
    public class StackObject
    {
        public string name { get; private set; }
        public List<object> obtype { get; private set; }
        public StackObject(string name, List<object> obtype)
        {
            this.name = name;
            this.obtype = obtype;
        }
    }
    public class StackObjects
    {
        public static StackObject pyint = new StackObject("int", new List<object>() { typeof(int) });
        public static StackObject pylong = pyint;
        public static StackObject pyinteger_or_bool = new StackObject("int_or_bool", new List<object>() { typeof(int), typeof(bool) });
        public static StackObject pybool = new StackObject("bool", new List<object>() { typeof(bool) });
        public static StackObject pyfloat = new StackObject("float", new List<object>() { typeof(float) });
        public static StackObject pybytes_or_str = new StackObject("bytes_or_str", new List<object>() { typeof(byte), typeof(string) });
        public static StackObject pybytes = new StackObject("bytes", new List<object>() { typeof(byte) });
        public static StackObject pybytearray = new StackObject("bytearray", new List<object>() { typeof(Array) });
        public static StackObject pyunicode = new StackObject("str", new List<object>() { typeof(string) });
        public static StackObject pynull = new StackObject("null", new List<object>() { typeof(Nullable) });
        public static StackObject pytuple = new StackObject("tuple", new List<object>() { typeof(Tuple) });
        public static StackObject pylist = new StackObject("list", new List<object>() { typeof(List<>) });
        public static StackObject pydict = new StackObject("dict", new List<object>() { typeof(Dictionary<,>) });
        public static StackObject pyset = new StackObject("set", new List<object>() { typeof(HashSet<>) });
        public static StackObject pyfrozenset = new StackObject("frozenset", new List<object>() { typeof(FrozenSet<>) });
        public static StackObject pybuffer = new StackObject("buffer", new List<object>() { typeof(object) });
        public static StackObject anyobject = new StackObject("any", new List<object>() { typeof(object) });
        public static StackObject markobject = new StackObject("mark", new List<object>() { typeof(StackObject) });
        public static StackObject stackslice = new StackObject("stackslice", new List<object>() { typeof(StackObject) });
    }
    public class OpcodeInfo<T>
    {

        public string name {get; private set;}
        public int code { get; private set; }
        public Args<T>? arg { get; private set; }
        public List<StackObject> stack_before { get; private set; }
        public List<StackObject> stack_after { get; private set; }
        public int proto { get; private set; }
        public OpcodeInfo(string name, char code, Args<T>? arg, List<StackObject> stack_before, List<StackObject> stack_after,int proto)
        {
            this.name = name;
            this.code = Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { code }).FirstOrDefault();
            this.arg = arg;
            this.stack_before = stack_before;
            this.stack_after = stack_after;
            this.proto = proto;
            
        }
    }
    public class code2op
    {
        const int UP_TO_NEWLINE = -1;
        const int TAKEN_FROM_ARGUMENT1  = -2;
        const int TAKEN_FROM_ARGUMENT4  = -3;
        const int TAKEN_FROM_ARGUMENT4U = -4;
        const int TAKEN_FROM_ARGUMENT8U = -5;



        Args<int> uint1 = new Args<int>("uint1", 1, Argsfunc.read_uint1);
        Args<ushort> uint2 = new Args<ushort>("uint2", 2, Argsfunc.read_uint2);
        Args<int[]> int4 = new Args<int[]>("int4", 4, Argsfunc.read_int4);
        Args<int> uint4 = new Args<int>("uint4", 4, Argsfunc.read_uint4);
        Args<int[]> uint8 = new Args<int[]>("uint4", 8, Argsfunc.read_uint8);
        Args<int[]> stringnl = new Args<int[]>("stringnl", UP_TO_NEWLINE, Argsfunc.read_stringnl);
        Args<int[]> stringnl_noescape = new Args<int[]>("stringnl_noescape", UP_TO_NEWLINE, Argsfunc.read_stringnl_noescape);
        Args<int[]> stringnl_noescape_pair = new Args<int[]>("stringnl_noescape_pair", UP_TO_NEWLINE, Argsfunc.read_stringnl_noescape_pair);
        Args<int[]> string1 = new Args<int[]>("string1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_string1);
        Args<int[]> string4 = new Args<int[]>("string4", TAKEN_FROM_ARGUMENT4, Argsfunc.read_string4);
        Args<int[]> bytes1 = new Args<int[]>("bytes1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_bytes1);
        Args<int[]> bytes4 = new Args<int[]>("bytes1", TAKEN_FROM_ARGUMENT4U, Argsfunc.read_bytes4);
        Args<int[]> bytes8 = new Args<int[]>("bytes8", TAKEN_FROM_ARGUMENT8U, Argsfunc.read_bytes8);
        Args<int[]> bytearray8 = new Args<int[]>("bytearray8", TAKEN_FROM_ARGUMENT8U, Argsfunc.read_bytearray8);
        Args<int[]> unicodestringnl = new Args<int[]>("unicodestringnl", UP_TO_NEWLINE, Argsfunc.read_unicodestringnl);
        Args<int[]> unicodestring1 = new Args<int[]>("unicodestring1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_unicodestring1);
        Args<string> unicodestring4 = new Args<string>("unicodestring4", TAKEN_FROM_ARGUMENT4U, Argsfunc.read_unicodestring4);
        Args<int[]> unicodestring8 = new Args<int[]>("unicodestring8", TAKEN_FROM_ARGUMENT8U, Argsfunc.read_unicodestring8);
        Args<int[]> decimalnl_short = new Args<int[]>("decimalnl_short", UP_TO_NEWLINE, Argsfunc.read_decimalnl_short);
        Args<int[]> decimalnl_long = new Args<int[]>("decimalnl_long", UP_TO_NEWLINE, Argsfunc.read_decimalnl_long);
        Args<int[]> floatnl = new Args<int[]>("floatnl", UP_TO_NEWLINE, Argsfunc.read_floatnl);
        Args<int[]> float8 = new Args<int[]>("float8", 8, Argsfunc.read_float8);
        Args<int[]> long1 = new Args<int[]>("long1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_long1);
        Args<int[]> long4 = new Args<int[]>("long4", TAKEN_FROM_ARGUMENT4, Argsfunc.read_long4);

        //List<OpcodeInfo> code2oplist = new List<OpcodeInfo>();
        public code2op()
        {

        }
        public object? get(int code)
        {
            if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'I' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("INT",
                  'I',
                  decimalnl_short,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyinteger_or_bool },
                  0);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'J' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BININT",
                  'J',
                  int4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  1);
            }

            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'K' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("BININT1",
                  'K',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  1);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'M' }).FirstOrDefault())
            {
                return new OpcodeInfo<ushort>("BININT2",
                  'M',
                  uint2,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  1);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'L' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("LONG",
                  'L',
                  decimalnl_long,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  0);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8a' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("LONG1",
                  '\x8a',
                  long1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  2);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8b' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("LONG4",
                  '\x8b',
                  long4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'S' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("STRING",
                  'S',
                  stringnl,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes_or_str },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'T' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BINSTRING",
                  'T',
                  string4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes_or_str },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'U' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("SHORT_BINSTRING",
                  'U',
                  string1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes_or_str },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'B' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BINBYTES",
                  'B',
                  bytes4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes },
                  3);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'C' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("SHORT_BINBYTES",
                  'C',
                  bytes1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes },
                  3);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8e' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BINBYTES8",
                  '\x8e',
                  bytes8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x96' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BYTEARRAY8",
                  '\x96',
                  bytearray8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytearray },
                  5);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x97' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("NEXT_BUFFER",
                  '\x97',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybuffer },
                  5);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x98' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("READONLY_BUFFER",
                  '\x98',
                  null,
                  new List<StackObject> { StackObjects.pybuffer },
                  new List<StackObject> { StackObjects.pybuffer },
                  5);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'N' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("null",
                  'N',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pynull },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x88' }).FirstOrDefault())
            {
                new OpcodeInfo<int[]>("NEWTRUE",
                  '\x88',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybool },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x89' }).FirstOrDefault())
            {
                new OpcodeInfo<int[]>("NEWFALSE",
                  '\x89',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybool },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'V' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("UNICODE",
                  'V',
                  unicodestringnl,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8c' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("SHORT_BINUNICODE",
                  '\x8c',
                  unicodestring1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'X' }).FirstOrDefault())
            {
                return new OpcodeInfo<string>("BINUNICODE",
                  'X',
                  unicodestring4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8d' }).FirstOrDefault())
            {
                new OpcodeInfo<int[]>("BINUNICODE8",
                  '\x8d',
                  unicodestring8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'F' }).FirstOrDefault())
            {
                new OpcodeInfo<int[]>("FLOAT",
                  'F',
                  floatnl,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyfloat },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'G' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BINFLOAT",
                  'G',
                  float8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyfloat },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { ']' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("EMPTY_LIST",
                  ']',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pylist },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'a' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("APPEND",
                  'a',
                  null,
                  new List<StackObject> { StackObjects.pylist, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pylist },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'e' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("APPENDS",
                  'e',
                  null,
                  new List<StackObject> { StackObjects.pylist, StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pylist },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'l' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("LIST",
                  'l',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pylist },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { ')' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("EMPTY_TUPLE",
                  ')',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pytuple },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 't' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("TUPLE",
                  't',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pytuple },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x85' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("TUPLE1",
                  '\x85',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pytuple },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x86' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("TUPLE2",
                  '\x86',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pytuple },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x87' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("TUPLE3",
                  '\x87',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pytuple },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '}' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("EMPTY_DICT",
                  '}',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pydict },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'd' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("DICT",
                  'd',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pydict },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 's' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("SETITEM",
                  's',
                  null,
                  new List<StackObject> { StackObjects.pydict, StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pydict },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'u' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("SETITEMS",
                  'u',
                  null,
                  new List<StackObject> { StackObjects.pydict, StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pydict },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8f' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("EMPTY_SET",
                  '\x8f',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyset },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x90' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("ADDITEMS",
                  '\x90',
                  null,
                  new List<StackObject> { StackObjects.pyset, StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pyset },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x91' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("FROZENSET",
                  '\x91',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pyfrozenset },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '0' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("POP",
                  '0',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject>(),
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '2' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("DUP",
                  '2',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '(' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("MARK",
                  '(',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.markobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '1' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("POP_MARK",
                  '1',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject>(),
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'g' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("GET",
                  'g',
                  decimalnl_short,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'h' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("BINGET",
                  'h',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'j' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("LONG_BINGET",
                  'j',
                  uint4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'p' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("PUT",
                  'p',
                  decimalnl_short,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'q' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("BINPUT",
                  'q',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'r' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("LONG_BINPUT",
                  'r',
                  uint4,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x94' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("MEMOIZE",
                  '\x94',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x82' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("EXT1",
                  '\x82',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x83' }).FirstOrDefault())
            {
                return new OpcodeInfo<ushort>("EXT2",
                  '\x83',
                  uint2,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x84' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("EXT4",
                  '\x84',
                  int4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'c' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("GLOBAL",
                  'c',
                  stringnl_noescape_pair,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x93' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("STACK_GLOBAL",
                  '\x93',
                  null,
                  new List<StackObject> { StackObjects.pyunicode, StackObjects.pyunicode },
                  new List<StackObject> { StackObjects.anyobject },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'R' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("REDUCE",
                  'R',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'b' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BUILD",
                  'b',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'i' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("INST",
                  'i',
                  stringnl_noescape_pair,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'o' }).FirstOrDefault())
            {
                new OpcodeInfo<int[]>("OBJ",
                  'o',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.anyobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x81' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("NEWOBJ",
                  '\x81',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x92' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("NEWOBJ_EX",
                  '\x92',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x80' }).FirstOrDefault())
            {
                return new OpcodeInfo<int>("PROTO",
                  '\x80',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '.' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("STOP",
                  '.',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject>(),
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x95' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("FRAME",
                  '\x95',
                  uint8,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'P' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("PERSID",
                  'P',
                  stringnl_noescape,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'Q' }).FirstOrDefault())
            {
                return new OpcodeInfo<int[]>("BINPERSID",
                  'Q',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else{
                return null;
            }
            return null;
        }

    }
    public class Args<T>
    {
        public string name { get; private set; }
        public int n { get; private set; }
        public Func<MemoryStream, T> reader { get; private set; }

        public Args(string name, int n, Func<MemoryStream, T> reader)
        {
            this.name = name;
            this.n = n;
            this.reader = reader;
        }
    }
    public class Argsfunc
    {
        public static int FromBytes(byte[] data, bool isLittleEndian, bool signed)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                Array.Reverse(data); // Reverse the byte array for the correct endianness
            }

            if (signed)
            {
                // Convert to signed integer
                return data.Length switch
                {
                    1 => (sbyte)data[0],
                    2 => BitConverter.ToInt16(data, 0),
                    4 => BitConverter.ToInt32(data, 0),
                    8 => (int)BitConverter.ToInt64(data, 0),
                    _ => throw new ArgumentException("Unsupported data length for signed integer conversion.")
                };
            }
            else
            {
                // Convert to unsigned integer
                return data.Length switch
                {
                    1 => data[0],
                    2 => BitConverter.ToUInt16(data, 0),
                    4 => (int)BitConverter.ToUInt32(data, 0),
                    8 => (int)BitConverter.ToUInt64(data, 0),
                    _ => throw new ArgumentException("Unsupported data length for unsigned integer conversion.")
                };
            }
        }
        public static int[] read_decimalnl_short(MemoryStream stream)
        {
            // Read the string until a newline character
            string s = ReadStringNl(stream);

            // Handle the special cases for "00" and "01"
            if (s == "00")
            {
                return [Convert.ToInt16(false)];
            }
            else if (s == "01")
            {
                return [Convert.ToInt16(true)];
            }

            // Convert to an integer, throwing an exception if invalid
            if (int.TryParse(s, out int result))
            {
                return [result];
            }
            else
            {
                throw new FormatException($"Invalid literal for int: {s}");
            }
        }
        public static int read_uint1(MemoryStream stream)
        {
            int data = stream.ReadByte();

            // Convert to an integer, throwing an exception if invalid
            if (data != null)
            {
                return data;
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static ushort read_uint2(MemoryStream stream)
        {
            int readnr = 2;
            int[] data = new int[readnr];
            for (int i = 0; i < readnr; i++)
            {
                data[i] = stream.ReadByte();
            }

            // Convert to an integer, throwing an exception if invalid
            if (data.Count() == 2)
            {
                byte[] byteData = Array.ConvertAll(data, i => (byte)i);
                byteData.Reverse();
                return BitConverter.ToUInt16(byteData, 0);

            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static int[] read_int4(MemoryStream stream)
        {
            int readnr = 4;
            int[] data = new int[readnr];
            for (int i = 0; i < readnr; i++)
            {
                data[i] = stream.ReadByte();
            }

            // Convert to an integer, throwing an exception if invalid
            if (data != null)
            {
                return data;
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static int read_uint4(MemoryStream stream)
        {
            int readnr = 4;
            int[] data = new int[readnr];
            for (int i = 0; i < readnr; i++)
            {
                data[i] = stream.ReadByte();
            }

            // Convert to an integer, throwing an exception if invalid
            if (data.Count() == 4)
            {
                return data[0];
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static int[] read_uint8(MemoryStream stream)
        {
            int readnr = 8;
            int[] data = new int[readnr];
            for (int i = 0; i < readnr; i++)
            {
                data[i] = stream.ReadByte();
            }

            // Convert to an integer, throwing an exception if invalid
            if (data != null)
            {
                return data;
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }

        public static int[] read_stringnl(MemoryStream stream)
        {
            List<int> intData = new List<int>();

            while (true)
            {
                int readByte = stream.ReadByte(); // Read a single byte from the stream
                if (readByte == -1) // End of stream
                {
                    throw new Exception("No newline found when trying to read stringnl");
                }

                if (readByte == '\n') // Stop at newline
                {
                    break;
                }

                intData.Add(readByte); // Add byte to the list
            }
            intData.RemoveAll(x => x == 34 || x == 39);
            return intData.ToArray(); // Convert to int[]
        }
        public static int[] read_stringnl(MemoryStream stream, bool stripquotes)
        {
            List<int> intData = new List<int>();

            while (true)
            {
                int readByte = stream.ReadByte(); // Read a single byte from the stream
                if (readByte == -1) // End of stream
                {
                    throw new Exception("No newline found when trying to read stringnl");
                }

                if (readByte == '\n') // Stop at newline
                {
                    break;
                }

                intData.Add(readByte); // Add byte to the list
            }

            return intData.ToArray(); // Convert to int[]
        }
        public static int[] read_stringnl_noescape(MemoryStream stream)
        {
            return read_stringnl(stream,false);
        }
        public static int[] read_stringnl_noescape_pair(MemoryStream stream)
        {
            
            string a = Encoding.GetEncoding("ascii").GetString(Array.ConvertAll(read_stringnl_noescape(stream), i => (byte)i));
            string b = Encoding.GetEncoding("ascii").GetString(Array.ConvertAll(read_stringnl_noescape(stream), i => (byte)i));
            string datastring = $"{a} {b}";
            byte[] byteData = Encoding.GetEncoding("ISO-8859-1").GetBytes(datastring);
            int[] data = Array.ConvertAll(byteData, b => (int)b);
            return data;
        }
        public static int[] read_string1(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_string4(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_bytes1(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_bytes4(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_bytes8(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_bytearray8(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_unicodestringnl(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_unicodestring1(MemoryStream stream)
        {
            return [];
        }
        public static string read_unicodestring4(MemoryStream stream)
        {
            int n = read_uint4(stream);
            int[] data = new int[n];
            for (int i = 0; i < n; i++)
            {
                data[i] = stream.ReadByte();
            }
            // Convert int[] to byte[]
            byte[] byteData = Array.ConvertAll(data, i => (byte)i);

            // Create UTF-8 encoding with a custom fallback for "surrogatepass" behavior
            Encoding utf8WithSurrogatePass = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);

            // Decode byte array to string
            string decodedString = utf8WithSurrogatePass.GetString(byteData);
            return decodedString;
        }
        public static int[] read_unicodestring8(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_decimalnl_long(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_floatnl(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_float8(MemoryStream stream)
        {
            int[] data = new int[8];
            for (int i = 0; i < 8; i++)
            {
                data[i] = stream.ReadByte();
            }
            if (data.Count() == 8)
            {
                Array.Reverse(data);
            }
            return data;
        }
        public static int[] read_long1(MemoryStream stream)
        {
            return [];
        }
        public static int[] read_long4(MemoryStream stream)
        {
            return [];
        }


        private static string ReadStringNl(MemoryStream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"), leaveOpen: true))
            {
                var sb = new StringBuilder();
                while (reader.Peek() != -1)
                {
                    char c = (char)reader.Read();
                    if (c == '\n') // Stop at newline
                        break;

                    sb.Append(c);
                }

                return sb.ToString();
            }
        }
    }
}
