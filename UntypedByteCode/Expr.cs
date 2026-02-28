using Blover.BC;

namespace Blover.UBC
{
    internal abstract record class Expr
    {
        public record class VariableRead(Variable Variable) : Expr;
        public record class IntConstant(int Value) : Expr;
        public record class BoolConstant(bool Value) : Expr;
    }
}