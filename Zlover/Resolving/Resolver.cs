

using Blover.Zlover.Scanning;

namespace Blover.Zlover.Resolving
{
    internal class Resolver
    {
        public Stmt ResolveStatment(Parsing.Stmt stmt, Environment environment)
        {
            switch (stmt)
            {
                case Parsing.Stmt.Assertion pstmt:
                    {
                        // find the variable in the context
                        Variable? variable = environment.GetVariable(pstmt.Target.IdentifierName);
                        if (variable is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Target.IdentifierName}");
                        }
                        return new Stmt.Assertion(variable, pstmt);
                    }
                case Parsing.Stmt.Assumption pstmt:
                    {
                        // find the variable in the context
                        Variable? variable = environment.GetVariable(pstmt.Target.IdentifierName);
                        if (variable is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Target.IdentifierName}");
                        }
                        return new Stmt.Assumption(variable, pstmt);
                    }
                case Parsing.Stmt.AssignBool pstmt:
                    {
                        // find the variable in the context
                        Variable? variable = environment.GetVariable(pstmt.Target.IdentifierName);
                        if (variable is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Target.IdentifierName}");
                        }
                        return new Stmt.AssignBool(variable, pstmt.Value.Value, pstmt);
                    }
                case Parsing.Stmt.AssignInt pstmt:
                    {
                        // find the variable in the context
                        Variable? variable = environment.GetVariable(pstmt.Target.IdentifierName);
                        if (variable is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Target.IdentifierName}");
                        }
                        return new Stmt.AssignInt(variable, pstmt.Value.Value, pstmt);
                    }
                case Parsing.Stmt.AssignVariable pstmt:
                    {
                        // find the variable in the context
                        Variable? target = environment.GetVariable(pstmt.Target.IdentifierName);
                        if (target is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Target.IdentifierName}");
                        }
                        Variable? value = environment.GetVariable(pstmt.Value.IdentifierName);
                        if (value is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Value.IdentifierName}");
                        }
                        return new Stmt.AssignVariable(target, value, pstmt);
                    }
                
                case Parsing.Stmt.VariableDeclaration pstmt:
                    {
                        // declare the variable in the context
                        Variable? variable = environment.DeclareVariable(pstmt.Variable.IdentifierName, pstmt.Variable);
                        if(variable is null)
                        {
                            throw new Exception($"Could not create a variable with name: {pstmt.Variable.IdentifierName}");
                        }
                        return new Stmt.VariableDeclaration(variable, pstmt);
                    }
                case Parsing.Stmt.Call pstmt:
                    {
                        // find the variable in the context
                        Variable? variable = environment.GetVariable(pstmt.Variable.IdentifierName);
                        if (variable is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Variable.IdentifierName}");
                        }
                        Variable? function = environment.GetVariable(pstmt.Function.IdentifierName);
                        if (function is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Function.IdentifierName}");
                        }
                        Stmt.CallArgumentList? args = null;
                        if(pstmt.Arguments is not null)
                        {
                            args = GetCallArgumentList(pstmt.Arguments, environment);
                        }
                        return new Stmt.Call(variable, pstmt.OutputNumber.Value, function, args, pstmt);
                    }
                default:
                    throw new Exception($"Cannot resolve statement of type: {stmt.GetType()}");
            }
        }

        Stmt.CallArgumentList GetCallArgumentList(Parsing.Stmt.CallArgumentList args, Environment environment)
        {
            throw new NotImplementedException();
        }
    }
}