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
    public class OpcodeInfo
    {

        public string name {get; private set;}
        public int code { get; private set; }
        public Args? arg { get; private set; }
        public List<StackObject> stack_before { get; private set; }
        public List<StackObject> stack_after { get; private set; }
        public int proto { get; private set; }
        public OpcodeInfo(string name, char code, Args? arg, List<StackObject> stack_before, List<StackObject> stack_after,int proto)
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



        Args uint1 = new Args("uint1", 1, Argsfunc.read_uint1);
        Args uint2 = new Args("uint2", 2, Argsfunc.read_uint2);
        Args int4 = new Args("int4", 4, Argsfunc.read_int4);
        Args uint4 = new Args("uint4", 4, Argsfunc.read_uint4);
        Args uint8 = new Args("uint4", 8, Argsfunc.read_uint8);
        Args stringnl = new Args("stringnl", UP_TO_NEWLINE, Argsfunc.read_stringnl);
        Args stringnl_noescape = new Args("stringnl_noescape", UP_TO_NEWLINE, Argsfunc.read_stringnl_noescape);
        Args stringnl_noescape_pair = new Args("stringnl_noescape_pair", UP_TO_NEWLINE, Argsfunc.read_stringnl_noescape_pair);
        Args string1 = new Args("string1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_string1);
        Args string4 = new Args("string4", TAKEN_FROM_ARGUMENT4, Argsfunc.read_string4);
        Args bytes1 = new Args("bytes1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_bytes1);
        Args bytes4 = new Args("bytes1", TAKEN_FROM_ARGUMENT4U, Argsfunc.read_bytes4);
        Args bytes8 = new Args("bytes8", TAKEN_FROM_ARGUMENT8U, Argsfunc.read_bytes8);
        Args bytearray8 = new Args("bytearray8", TAKEN_FROM_ARGUMENT8U, Argsfunc.read_bytearray8);
        Args unicodestringnl = new Args("unicodestringnl", UP_TO_NEWLINE, Argsfunc.read_unicodestringnl);
        Args unicodestring1 = new Args("unicodestring1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_unicodestring1);
        Args unicodestring4 = new Args("unicodestring4", TAKEN_FROM_ARGUMENT4U, Argsfunc.read_unicodestring4);
        Args unicodestring8 = new Args("unicodestring8", TAKEN_FROM_ARGUMENT8U, Argsfunc.read_unicodestring8);
        Args decimalnl_short = new Args("decimalnl_short", UP_TO_NEWLINE, Argsfunc.read_decimalnl_short);
        Args decimalnl_long = new Args("decimalnl_long", UP_TO_NEWLINE, Argsfunc.read_decimalnl_long);
        Args floatnl = new Args("floatnl", UP_TO_NEWLINE, Argsfunc.read_floatnl);
        Args float8 = new Args("float8", 8, Argsfunc.read_float8);
        Args long1 = new Args("long1", TAKEN_FROM_ARGUMENT1, Argsfunc.read_long1);
        Args long4 = new Args("long4", TAKEN_FROM_ARGUMENT4, Argsfunc.read_long4);

        //List<OpcodeInfo> code2oplist = new List<OpcodeInfo>();
        public code2op()
        {

        }
        public OpcodeInfo? get(int? code)
        {
            if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'I' }).FirstOrDefault())
            {
                return new OpcodeInfo("INT",
                  'I',
                  decimalnl_short,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyinteger_or_bool },
                  0);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'J' }).FirstOrDefault())
            {
                return new OpcodeInfo("BININT",
                  'J',
                  int4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  1);
            }

            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'K' }).FirstOrDefault())
            {
                return new OpcodeInfo("BININT1",
                  'K',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  1);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'M' }).FirstOrDefault())
            {
                return new OpcodeInfo("BININT2",
                  'M',
                  uint2,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  1);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'L' }).FirstOrDefault())
            {
                return new OpcodeInfo("LONG",
                  'L',
                  decimalnl_long,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  0);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8a' }).FirstOrDefault())
            {
                return new OpcodeInfo("LONG1",
                  '\x8a',
                  long1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  2);
            }
            else if (code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8b' }).FirstOrDefault())
            {
                return new OpcodeInfo("LONG4",
                  '\x8b',
                  long4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyint },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'S' }).FirstOrDefault())
            {
                return new OpcodeInfo("STRING",
                  'S',
                  stringnl,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes_or_str },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'T' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINSTRING",
                  'T',
                  string4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes_or_str },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'U' }).FirstOrDefault())
            {
                return new OpcodeInfo("SHORT_BINSTRING",
                  'U',
                  string1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes_or_str },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'B' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINBYTES",
                  'B',
                  bytes4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes },
                  3);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'C' }).FirstOrDefault())
            {
                return new OpcodeInfo("SHORT_BINBYTES",
                  'C',
                  bytes1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes },
                  3);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8e' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINBYTES8",
                  '\x8e',
                  bytes8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytes },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x96' }).FirstOrDefault())
            {
                return new OpcodeInfo("BYTEARRAY8",
                  '\x96',
                  bytearray8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybytearray },
                  5);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x97' }).FirstOrDefault())
            {
                return new OpcodeInfo("NEXT_BUFFER",
                  '\x97',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybuffer },
                  5);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x98' }).FirstOrDefault())
            {
                return new OpcodeInfo("READONLY_BUFFER",
                  '\x98',
                  null,
                  new List<StackObject> { StackObjects.pybuffer },
                  new List<StackObject> { StackObjects.pybuffer },
                  5);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'N' }).FirstOrDefault())
            {
                return new OpcodeInfo("null",
                  'N',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pynull },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x88' }).FirstOrDefault())
            {
                return new OpcodeInfo("NEWTRUE",
                  '\x88',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybool },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x89' }).FirstOrDefault())
            {
                return new OpcodeInfo("NEWFALSE",
                  '\x89',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pybool },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'V' }).FirstOrDefault())
            {
                return new OpcodeInfo("UNICODE",
                  'V',
                  unicodestringnl,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8c' }).FirstOrDefault())
            {
                return new OpcodeInfo("SHORT_BINUNICODE",
                  '\x8c',
                  unicodestring1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'X' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINUNICODE",
                  'X',
                  unicodestring4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8d' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINUNICODE8",
                  '\x8d',
                  unicodestring8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyunicode },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'F' }).FirstOrDefault())
            {
                return new OpcodeInfo("FLOAT",
                  'F',
                  floatnl,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyfloat },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'G' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINFLOAT",
                  'G',
                  float8,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyfloat },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { ']' }).FirstOrDefault())
            {
                return new OpcodeInfo("EMPTY_LIST",
                  ']',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pylist },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'a' }).FirstOrDefault())
            {
                return new OpcodeInfo("APPEND",
                  'a',
                  null,
                  new List<StackObject> { StackObjects.pylist, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pylist },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'e' }).FirstOrDefault())
            {
                return new OpcodeInfo("APPENDS",
                  'e',
                  null,
                  new List<StackObject> { StackObjects.pylist, StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pylist },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'l' }).FirstOrDefault())
            {
                return new OpcodeInfo("LIST",
                  'l',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pylist },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { ')' }).FirstOrDefault())
            {
                return new OpcodeInfo("EMPTY_TUPLE",
                  ')',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pytuple },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 't' }).FirstOrDefault())
            {
                return new OpcodeInfo("TUPLE",
                  't',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pytuple },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x85' }).FirstOrDefault())
            {
                return new OpcodeInfo("TUPLE1",
                  '\x85',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pytuple },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x86' }).FirstOrDefault())
            {
                return new OpcodeInfo("TUPLE2",
                  '\x86',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pytuple },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x87' }).FirstOrDefault())
            {
                return new OpcodeInfo("TUPLE3",
                  '\x87',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pytuple },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '}' }).FirstOrDefault())
            {
                return new OpcodeInfo("EMPTY_DICT",
                  '}',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pydict },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'd' }).FirstOrDefault())
            {
                return new OpcodeInfo("DICT",
                  'd',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pydict },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 's' }).FirstOrDefault())
            {
                return new OpcodeInfo("SETITEM",
                  's',
                  null,
                  new List<StackObject> { StackObjects.pydict, StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.pydict },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'u' }).FirstOrDefault())
            {
                return new OpcodeInfo("SETITEMS",
                  'u',
                  null,
                  new List<StackObject> { StackObjects.pydict, StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pydict },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x8f' }).FirstOrDefault())
            {
                return new OpcodeInfo("EMPTY_SET",
                  '\x8f',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.pyset },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x90' }).FirstOrDefault())
            {
                return new OpcodeInfo("ADDITEMS",
                  '\x90',
                  null,
                  new List<StackObject> { StackObjects.pyset, StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pyset },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x91' }).FirstOrDefault())
            {
                return new OpcodeInfo("FROZENSET",
                  '\x91',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.pyfrozenset },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '0' }).FirstOrDefault())
            {
                return new OpcodeInfo("POP",
                  '0',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject>(),
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '2' }).FirstOrDefault())
            {
                return new OpcodeInfo("DUP",
                  '2',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '(' }).FirstOrDefault())
            {
                return new OpcodeInfo("MARK",
                  '(',
                  null,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.markobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '1' }).FirstOrDefault())
            {
                return new OpcodeInfo("POP_MARK",
                  '1',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject>(),
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'g' }).FirstOrDefault())
            {
                return new OpcodeInfo("GET",
                  'g',
                  decimalnl_short,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'h' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINGET",
                  'h',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'j' }).FirstOrDefault())
            {
                return new OpcodeInfo("LONG_BINGET",
                  'j',
                  uint4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'p' }).FirstOrDefault())
            {
                return new OpcodeInfo("PUT",
                  'p',
                  decimalnl_short,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'q' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINPUT",
                  'q',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'r' }).FirstOrDefault())
            {
                return new OpcodeInfo("LONG_BINPUT",
                  'r',
                  uint4,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x94' }).FirstOrDefault())
            {
                return new OpcodeInfo("MEMOIZE",
                  '\x94',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x82' }).FirstOrDefault())
            {
                return new OpcodeInfo("EXT1",
                  '\x82',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x83' }).FirstOrDefault())
            {
                return new OpcodeInfo("EXT2",
                  '\x83',
                  uint2,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x84' }).FirstOrDefault())
            {
                return new OpcodeInfo("EXT4",
                  '\x84',
                  int4,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'c' }).FirstOrDefault())
            {
                return new OpcodeInfo("GLOBAL",
                  'c',
                  stringnl_noescape_pair,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x93' }).FirstOrDefault())
            {
                return new OpcodeInfo("STACK_GLOBAL",
                  '\x93',
                  null,
                  new List<StackObject> { StackObjects.pyunicode, StackObjects.pyunicode },
                  new List<StackObject> { StackObjects.anyobject },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'R' }).FirstOrDefault())
            {
                return new OpcodeInfo("REDUCE",
                  'R',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'b' }).FirstOrDefault())
            {
                return new OpcodeInfo("BUILD",
                  'b',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'i' }).FirstOrDefault())
            {
                return new OpcodeInfo("INST",
                  'i',
                  stringnl_noescape_pair,
                  new List<StackObject> { StackObjects.markobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'o' }).FirstOrDefault())
            {
                return new OpcodeInfo("OBJ",
                  'o',
                  null,
                  new List<StackObject> { StackObjects.markobject, StackObjects.anyobject, StackObjects.stackslice },
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x81' }).FirstOrDefault())
            {
                return new OpcodeInfo("NEWOBJ",
                  '\x81',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x92' }).FirstOrDefault())
            {
                return new OpcodeInfo("NEWOBJ_EX",
                  '\x92',
                  null,
                  new List<StackObject> { StackObjects.anyobject, StackObjects.anyobject, StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x80' }).FirstOrDefault())
            {
                return new OpcodeInfo("PROTO",
                  '\x80',
                  uint1,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  2);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '.' }).FirstOrDefault())
            {
                return new OpcodeInfo("STOP",
                  '.',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject>(),
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { '\x95' }).FirstOrDefault())
            {
                return new OpcodeInfo("FRAME",
                  '\x95',
                  uint8,
                  new List<StackObject>(),
                  new List<StackObject>(),
                  4);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'P' }).FirstOrDefault())
            {
                return new OpcodeInfo("PERSID",
                  'P',
                  stringnl_noescape,
                  new List<StackObject>(),
                  new List<StackObject> { StackObjects.anyobject },
                  0);
            }
            else if ( code == Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { 'Q' }).FirstOrDefault())
            {
                return new OpcodeInfo("BINPERSID",
                  'Q',
                  null,
                  new List<StackObject> { StackObjects.anyobject },
                  new List<StackObject> { StackObjects.anyobject },
                  1);
            }
            else{
                return null;
            }
            
        }

    }
    public class Args
    {
        public string name { get; private set; }
        public int n { get; private set; }
        public Func<MemoryStream, string> reader { get; private set; }

        public Args(string name, int n, Func<MemoryStream, string> reader)
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
        public static string read_decimalnl_short(MemoryStream stream)
        {
            return "";
        }
        public static string read_uint1(MemoryStream stream)
        {
            int data = stream.ReadByte();

            // Convert to an integer, throwing an exception if invalid
            if (data != null)
            {

                return data.ToString();
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static string read_uint2(MemoryStream stream)
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
                int val = BitConverter.ToUInt16(byteData, 0);
                return val.ToString();

            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static string read_int4(MemoryStream stream)
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
                Int32 val = BitConverter.ToInt32(Array.ConvertAll(data, i => (byte)i), 0);
                return val.ToString();
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static string read_uint4(MemoryStream stream)
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
                UInt32 val =  BitConverter.ToUInt32(Array.ConvertAll(data, i => (byte)i), 0);
                return val.ToString();
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }
        public static string read_uint8(MemoryStream stream)
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
                return Encoding.GetEncoding("ISO-8859-1").GetString(Array.ConvertAll(data, i => (byte)i));
            }
            else
            {
                throw new FormatException($"not enough data in stream to read uint1");
            }
        }

        public static string read_stringnl(MemoryStream stream)
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
            return Encoding.GetEncoding("ISO-8859-1").GetString(Array.ConvertAll(intData.ToArray(), i => (byte)i));
        }
        public static string read_stringnl(MemoryStream stream, bool stripquotes)
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

            return Encoding.GetEncoding("ISO-8859-1").GetString(Array.ConvertAll(intData.ToArray(), i => (byte)i));
        }
        public static string read_stringnl_noescape(MemoryStream stream)
        {
            return read_stringnl(stream,false);
        }
        public static string read_stringnl_noescape_pair(MemoryStream stream)
        {
            string a = Encoding.GetEncoding("ascii").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(read_stringnl_noescape(stream)));
            string b = Encoding.GetEncoding("ascii").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(read_stringnl_noescape(stream)));
            string datastring = $"{a} {b}";
            return datastring;
        }
        public static string read_string1(MemoryStream stream)
        {
            return "";
        }
        public static string read_string4(MemoryStream stream)
        {
            return "";
        }
        public static string read_bytes1(MemoryStream stream)
        {
            return "";
        }
        public static string read_bytes4(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_bytes8(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_bytearray8(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_unicodestringnl(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_unicodestring1(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_unicodestring4(MemoryStream stream)
        {
            UInt32 n = Convert.ToUInt32(read_uint4(stream));
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
        public static string read_unicodestring8(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_decimalnl_long(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_floatnl(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_float8(MemoryStream stream)
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
            byte[] byteData = Array.ConvertAll(data, i => (byte)i);
            double value = BitConverter.ToDouble(byteData, 0);
            return value.ToString();
        }
        public static string read_long1(MemoryStream stream)
        {
            return "[]";
        }
        public static string read_long4(MemoryStream stream)
        {
            return "[]";
        }
    }
}
