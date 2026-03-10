using Blover.Aux;
using Blover.Zlover.Scanning;

namespace Blover.Zlover.Resolving
{
    internal abstract record class Decl(Parsing.Decl BaseDecl)
    {
        public Token GetFirstToken() => BaseDecl.GetFirstToken();
        public Token GetLastToken() => BaseDecl.GetLastToken();

        public override abstract string ToString();

        public record class Function(Variable Name, Block Body, Parsing.Decl BaseDecl) : Decl(BaseDecl)
        {
            public override string ToString() => $"fun {Name}\n{Body}";
        }

        public record class VariableDeclaration(Variable Name, Parsing.Decl BaseDecl) : Decl(BaseDecl)
        {
            public override string ToString() => $"dec {Name}";
        }

        public record class Block(List<Stmt> Body, Parsing.Decl.Block BaseDecl)
        {
            public override string ToString() => $"{{\n{string.Join("\n", from stmt in Body select $"{stmt}".Indent())}\n}}";
        }
    }
}