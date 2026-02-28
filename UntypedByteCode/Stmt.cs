using Blover.BC;

namespace Blover.UBC
{
    internal abstract record class Stmt
    {
        // Stmts can be:
        // Assignments (which in SSA are also variable declarations)
        // Assumption
        // Assertion
        public record class Assignment(Variable Variable, Expr Value) : Stmt;
        public record class Assumption(Variable Variable) : Stmt;
        public record class Assertion(Variable Variable) : Stmt;
        public record class FunctionCall(Variable Function, List<Variable> InArgs, List<Variable> OutArgs) : Stmt;
        public record class Z3Assumption(Z3AssumptionFunction AssumptionFunction, Variable CheckVar, List<Variable> Arguments) : Stmt
        {
            public override string ToString()
            {
                return $"Z3Assumption {{ {AssumptionFunction}, {CheckVar}, [{string.Join(", ",Arguments)}] }}";
            }
        }
        
    }

    internal record class Z3AssumptionFunction(string Name);
}