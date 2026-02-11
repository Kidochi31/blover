using Blover.Scanning;

namespace Blover.Parsing
{
    internal abstract record class Stmt
    {
        public abstract Token GetFirstToken();
        public abstract Token GetLastToken();

        public override abstract string ToString();

        public record class Declaration(IdentifierToken Variable, Token Colon, IdentifierToken TargetType, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Variable;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Variable.IdentifierName} : {TargetType.IdentifierName}";
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

        public record class Call(IdentifierToken Function, Token OpenParen, CallArgumentList? Arguments, Token CloseParen, Token Terminator) : Stmt
        {
            public override Token GetFirstToken() => Function;
            public override Token GetLastToken() => Terminator;

            public override string ToString() => $"{Function.IdentifierName}({Arguments})";
        }

        public record class CallArgumentList(CallArgument Arg0, List<(Token Comma, CallArgument Arg)> OtherArgs)
        {
            public override string ToString() => $"{Arg0}{string.Join("", from arg in OtherArgs select $", {arg.Arg}")}";
        }
        
        public abstract record class CallArgument(IdentifierToken Arg)
        {
            public override abstract string ToString();
            public record class In(IdentifierToken Arg) : CallArgument(Arg)
            {
                public override string ToString() => Arg.IdentifierName;
            }

            public record class Out(Token OutToken, IdentifierToken Arg) : CallArgument(Arg)
            {
                public override string ToString() => $"out {Arg.IdentifierName}";
            }
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