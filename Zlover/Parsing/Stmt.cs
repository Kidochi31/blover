using Blover.Aux;
using Blover.Zlover.Scanning;

namespace Blover.Zlover.Parsing
{
    internal abstract record class Stmt
    {
        public abstract Token GetFirstToken();
        public abstract Token GetLastToken();

        public override abstract string ToString();

        public record class VariableDeclaration(Token Dec, IdentifierToken Variable, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Dec;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"dec {Variable.IdentifierName}";
        }

        public record class AssignVariable(IdentifierToken Target, Token Equal, IdentifierToken Value, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Target;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Target.IdentifierName} = {Value.IdentifierName}";
        }

        public record class AssignInt(IdentifierToken Target, Token Equal, Token IntToken, IntegerToken Value, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Target;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Target.IdentifierName} = int {Value.Value}";
        }

        public record class AssignBool(IdentifierToken Target, Token Equal, Token BoolToken, BoolToken Value, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Target;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Target.IdentifierName} = bool {Value.Value}";
        }

        public record class Call(IdentifierToken Variable, Token Equal, Token CallToken, IntegerToken OutputNumber, IdentifierToken Function, Token OpenParen, CallArgumentList? Arguments, Token CloseParen, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Variable;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Variable.IdentifierName} = call {OutputNumber.Value} {Function.IdentifierName}({Arguments})";
        }

        public record class Verification(IdentifierToken Variable, Token Equal, Token VerifyToken, IdentifierToken Function, Token OpenParen, CallArgumentList? Arguments, Token CloseParen, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Variable;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Variable.IdentifierName} = verify {Function.IdentifierName}({Arguments})";
        }

        public record class CallArgumentList(IdentifierToken Arg0, List<(Token Comma, IdentifierToken Arg)> OtherArgs)
        {
            public override string ToString() => $"{Arg0.IdentifierName}{string.Join("", from arg in OtherArgs select $", {arg.Arg.IdentifierName}")}";
        }

        public record class Assumption(Token AssumeToken, IdentifierToken Target, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => AssumeToken;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"assume {Target.IdentifierName}";
        }
        
        public record class Assertion(Token AssertToken, IdentifierToken Target, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => AssertToken;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"assert {Target.IdentifierName}";
        }
    }
}