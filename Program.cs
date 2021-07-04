using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace FirstAntlr4Example
{
    class Program
    {
        static void Main(string[] args)
        {
            String input = "hello will";

            var stream = new AntlrInputStream(input);
            var lexer = new HelloLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new HelloParser(tokens);
            var compileUnit = parser.r();

            Console.WriteLine(compileUnit.ToStringTree());
        }
    }
}
