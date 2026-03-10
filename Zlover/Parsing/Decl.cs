using Blover.Aux;
using Blover.Zlover.Scanning;

namespace Blover.Zlover.Parsing
{
    internal abstract record class Decl
    {
        public abstract Token GetFirstToken();
        public abstract Token GetLastToken();

        public override abstract string ToString();

        public record class Function(Token FunToken, IdentifierToken Name, Block Body, Token Terminator) : Decl
        {
            public override Token GetFirstToken() => FunToken;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"fun {Name.IdentifierName}\n{Body}";
        }

        public record class Block(Token OpenBrace, List<Stmt> Body, Token CloseBrace)
        {
            public Token GetFirstToken() => OpenBrace;
            public Token GetLastToken() => CloseBrace;

            public override string ToString() => $"{{\n{string.Join("\n", from stmt in Body select $"{stmt}".Indent())}\n}}";
        }

        public record class VariableDeclaration(Token Dec, IdentifierToken Variable, Token Terminator) : Decl
        {
            public override Token GetFirstToken() => Dec;
            public override Token GetLastToken() => Terminator;
            public override string ToString() => $"dec {Variable.IdentifierName}";
        }
    }
}