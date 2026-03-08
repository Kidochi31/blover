

using System.Numerics;
using Blover.Zlover.Scanning;

namespace Blover.Zlover.Resolving
{
    internal abstract record class Stmt(Parsing.Stmt BaseStmt)
    {
        public Token GetFirstToken() => BaseStmt.GetFirstToken();
        public Token GetLastToken() => BaseStmt.GetLastToken();

        public override abstract string ToString();

        public record class VariableDeclaration(Variable Variable, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"dec {Variable}";
        }

        public record class AssignVariable(Variable Target, Variable Value, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"{Target} = {Value}";
        }

        public record class AssignInt(Variable Target, BigInteger Value, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"{Target} = int {Value}";
        }

        public record class AssignBool(Variable Target, bool Value, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"{Target} = bool {Value}";
        }

        public record class Call(Variable Variable, BigInteger OutputNumber, Variable Function, CallArgumentList? Arguments, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"{Variable} = call {OutputNumber} {Function}({Arguments})";
        }

        public record class Verification(Variable Variable, Variable Function, CallArgumentList? Arguments, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"{Variable} = verify {Function}({Arguments})";
        }

        public record class CallArgumentList(Variable Arg0, List<Variable> OtherArgs)
        {
            public override string ToString() => $"{Arg0}{string.Join("", from arg in OtherArgs select $", {arg}")}";
        }

        public record class Assumption(Variable Target, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"assume {Target}";
        }
        
        public record class Assertion(Variable Target, Parsing.Stmt BaseStmt) : Stmt(BaseStmt)
        {
            public override string ToString() => $"assert {Target}";
        }
    }
}