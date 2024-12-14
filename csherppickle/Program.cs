// See https://aka.ms/new-console-template for more information
using csherppickle;
using System.Text;

Console.WriteLine("Hello, World!");
string content = File.ReadAllText(@"log", Encoding.GetEncoding("ISO-8859-1"));
Unpacker unp = new Unpacker();
string result = unp.dis(content);
Console.WriteLine(result);