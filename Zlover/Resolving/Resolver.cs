

using Blover.Zlover.Scanning;

namespace Blover.Zlover.Resolving
{
    internal class Resolver
    {
        public Decl ResolveDeclaration(Parsing.Decl decl, Environment environment, VerifyFunEnvironment verifyFuns)
        {
            switch (decl)
            {
                case Parsing.Decl.Function pdecl:
                    {
                        // firstly, declare the function in scope
                        Variable? function = environment.DeclareVariable(pdecl.Name.IdentifierName, pdecl.Name);
                        if(function is null)
                        {
                            throw new Exception($"Could not create a function variable with name: {pdecl.Name.IdentifierName}");
                        }
                        // need to start a new environment...
                        Environment functionEnvironment = new Environment(environment);
                        List<Stmt> stmts = (from stmt in pdecl.Body.Body select ResolveStatement(stmt, functionEnvironment, verifyFuns)).ToList();
                        Decl.Block body = new(stmts, pdecl.Body);
                        return new Decl.Function(function, body, pdecl);
                    }
                case Parsing.Decl.VariableDeclaration pdecl:
                    {
                        Variable? variable = environment.DeclareVariable(pdecl.Variable.IdentifierName, pdecl.Variable);
                        if(variable is null)
                        {
                            throw new Exception($"Could not create a variable with name: {pdecl.Variable.IdentifierName}");
                        }
                        return new Decl.VariableDeclaration(variable, pdecl);
                    }
                default:
                    throw new Exception($"Cannot resolve declaration of type: {decl.GetType()}");
            }
        }

        public Stmt ResolveStatement(Parsing.Stmt stmt, Environment environment, VerifyFunEnvironment verifyFuns)
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
                case Parsing.Stmt.Verification pstmt:
                    {
                        // find the variable in the context
                        Variable? variable = environment.GetVariable(pstmt.Variable.IdentifierName);
                        if (variable is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Variable.IdentifierName}");
                        }
                        VerifyFunction? function = verifyFuns.GetVerifyFunction(pstmt.Function.IdentifierName);
                        if (function is null)
                        {
                            throw new Exception($"Could not find variable with name: {pstmt.Function.IdentifierName}");
                        }
                        Stmt.CallArgumentList? args = null;
                        if(pstmt.Arguments is not null)
                        {
                            args = GetCallArgumentList(pstmt.Arguments, environment);
                        }
                        return new Stmt.Verification(variable, function, args, pstmt);
                    }
                default:
                    throw new Exception($"Cannot resolve statement of type: {stmt.GetType()}");
            }
        }

        Stmt.CallArgumentList GetCallArgumentList(Parsing.Stmt.CallArgumentList args, Environment environment)
        {
            List<Variable> Args = new();
            Variable? arg0 = environment.GetVariable(args.Arg0.IdentifierName);
            if (arg0 is null)
            {
                throw new Exception($"Could not find variable with name: {args.Arg0.IdentifierName}");
            }
            Args.Add(arg0);
            foreach((_, IdentifierToken token) in args.OtherArgs)
            {
                Variable? arg = environment.GetVariable(token.IdentifierName);
                if (arg is null)
                {
                    throw new Exception($"Could not find variable with name: {token.IdentifierName}");
                }
                Args.Add(arg);
            }

            return new Stmt.CallArgumentList(Args);
        }
    }
}