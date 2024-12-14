using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace csherppickle
{
    public class Unpacker
    {
        code2op code2op = new code2op();
        
        public string dis(string pickle, int indentlevel= 4)
        {

            StringBuilder sb = new StringBuilder();
            Stack stack = new Stack();
            Dictionary<int, StackObject> memo = new Dictionary<int, StackObject>();
            int maxproto = -1;
            Stack markstack = new Stack();
            string indentChunk = new string(' ', 4);
            int annocol = 0;
            byte[] dataIn = Encoding.GetEncoding("ISO-8859-1").GetBytes(pickle);
            foreach (var (opcode, arg, pos) in genops(dataIn))
            {
                if (pos != null)
                {
                    sb.AppendFormat("{0,5}: ", pos);
                }
                sb.Append(string.Format("{0,-4} {1}{2}",
                opcode.code,
                new string(' ', indentChunk.Length * markstack.Count),
                opcode.name));

                maxproto = Math.Max(maxproto, opcode.proto);
                List<StackObject> before = new List<StackObject>(opcode.stack_before);
                List<StackObject> after = new List<StackObject>(opcode.stack_after);
                int numtopop = before.Count;
                string? errormsg = null;
                string? markmsg = null;
                //See whether a MARK should be popped.
                if (before.Contains(StackObjects.markobject) || (opcode.name == "POP" && stack.Count > 0 && stack.Peek() == StackObjects.markobject))
                {
                    if (after.Contains(StackObjects.markobject))
                    {
                        throw new InvalidOperationException("markobject should not be in the after collection");
                    }
                    if (markstack.Count > 0)
                    {
                        var markpos = markstack.Pop();
                        if (markpos == null)
                        {
                            markmsg = "(MARK at unknown opcode offset)";
                        }
                        else
                        {
                            markmsg = $"(MARK at {markpos})";
                        }
                        while (stack.Peek() != StackObjects.markobject) 
                        {
                            stack.Pop();
                        }
                        stack.Pop();
                        try
                        {
                            numtopop = before.IndexOf(StackObjects.markobject);
                        }
                        catch (Exception e)
                        {
                            if (opcode.name == "POP")
                            {
                                throw new InvalidOperationException("opcode.name is POP");
                            }
                            numtopop = 0;

                        }
                    }
                    else
                    {
                        errormsg = markmsg = "no MARK exists on stack";
                    }
                }

                //Check for correct memo usage.
                string[] validseter = { "PUT", "BINPUT", "LONG_BINPUT", "MEMOIZE" };
                string[] validgeter = { "GET", "BINGET", "LONG_BINGET" };
                int memo_idx = 0;
                if (validseter.Contains(opcode.name))
                {
                    if(opcode.name == "MEMOIZE")
                    {
                        memo_idx = memo.Count;
                        markmsg = $"(as {memo_idx})";
                    }
                    else
                    {
                        if (arg == null)
                        {
                            throw new InvalidOperationException("arg cant be null");
                        }
                        memo_idx = argtoint(arg);
                    }
                    if (memo.ContainsKey(memo_idx)) 
                    {
                        errormsg = $"memo key {arg} already defined";
                    }
                    else if (stack.Count == 0)
                    {
                        errormsg = "stack is empty -- can't store into memo";
                    }
                    else if(stack.Peek() == StackObjects.markobject)
                    {
                        errormsg = "can't store markobject in the memo";
                    }
                    else
                    {
                        memo[memo_idx] = stack.Peek() as StackObject;
                    }
                }
                else if (validgeter.Contains(opcode.name))
                {
                    
                    if (memo.ContainsKey(argtoint(arg)))
                    {
                        if (after.Count != 1)
                        {
                            throw new InvalidOperationException("Expected after.Count to be 1.");
                        }
                        if (memo.TryGetValue(argtoint(arg), out StackObject value))
                        {
                            // Create a new list containing the retrieved value as a StackObject
                            after = new List<StackObject>() { value };
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Key {arg} not found in the memo dictionary.");
                        }
                    }
                }

                if (arg != null || !string.IsNullOrWhiteSpace(markmsg))
                {
                    
                    sb.Append(new string(' ', 10 + (opcode.name.Length-1)));

                    // Append the argument if it's not null
                    if (arg != null)
                    {
                        sb.Append(" ").Append(arg);
                    }

                    // Append the mark message if it exists
                    if (!string.IsNullOrEmpty(markmsg))
                    {
                        sb.Append(" ").Append(markmsg);
                    }

                }
                if (!string.IsNullOrWhiteSpace(errormsg))
                {
                    throw new Exception(errormsg);
                }
                if(stack.Count < numtopop)
                {
                    throw new Exception($"tries to pop {numtopop} items from stack with only {stack.Count} items");
                }
                if(numtopop > 0)
                {
                    for (int i = 0; i < numtopop; i++)
                    {
                        stack.Pop();
                    }
                }
                if (after.Contains(StackObjects.markobject))
                {
                    if (before.Contains(StackObjects.markobject))
                    {
                        throw new InvalidOperationException("markobject should not be in the before collection.");
                    }
                    markstack.Push(pos);
                }
                foreach (var item in after)
                {
                    stack.Push(item);
                }
                sb.AppendLine();
            }
            sb.AppendLine($"highest protocol among opcodes = {maxproto}");
            if (stack.Count > 0)
            {
                throw new Exception("stack not empty after STOP");
            }
            return sb.ToString();
        }
        private int argtoint(string argvlaue)
        {
            int result;
            if (int.TryParse(argvlaue, out result))
            {
                return result;
            }
            else if (argvlaue.Length == 1) // Handle single-character inputs like '\0'
            {
                result = argvlaue[0]; // Convert the character to its Unicode code point
            }
            else
            {
                throw new InvalidOperationException("arg cant be read as int");
            }
            return result;
        }
        public IEnumerable<Tuple<OpcodeInfo, string?, long?>> genops(byte[] data_in)
        {
            using (MemoryStream data = new MemoryStream(data_in))
            {
                // Access the current position in the stream
                while (true){
                    long? pos = data.Position;
                    int? code = data.ReadByte();
                    OpcodeInfo? opcode =  code2op.get(code);
                    if (opcode == null) 
                    {
                        if (code == null)
                        {
                            throw new Exception("pickle exhausted before seeing STOP");
                        }
                        else 
                        {
                            throw new Exception("unknown element");
                        }
                    }

                    string? arg = null;
                    if (opcode.arg != null)
                    {
                        arg = opcode.arg.reader(data);
                    }
                    yield return Tuple.Create(opcode, arg, pos);
                    if(code == 46)
                    {
                        break;
                    }
                }
            }
        }
        
    }
}
