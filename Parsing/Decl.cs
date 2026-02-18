using Blover.Aux;
using Blover.Scanning;

namespace Blover.Parsing
{
    internal abstract record class Decl
    {
        public abstract Token GetFirstToken();
        public abstract Token GetLastToken();

        public override abstract string ToString();

        public record class Function(Token FunToken, IdentifierToken Name, Token OpenBrace, List<FunctionBlock> Blocks, Token CloseBrace, Token Terminator) : Decl
        {
            public override Token GetFirstToken() => FunToken;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"fun {Name.IdentifierName}\n{{\n{string.Join("\n", from block in Blocks select $"{block}".Indent())}\n}}";
        }

        public record class FunctionBlock(Token StartToken, Token OpenBrace, List<Stmt> Body, Token CloseBrace, Token Terminator)
        {
            public Token GetFirstToken() => StartToken;
            public Token GetLastToken() => Terminator;

            public override string ToString() => $"{StartToken.Lexeme}\n{{\n{string.Join("\n", from stmt in Body select $"{stmt}".Indent())}\n}}";
        }
    }
}