using Blover.Scanning;
using Blover.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Blover.Scanning.TokenType;

namespace Blover.Parsing
{
    internal partial class Parser
    {
        public Stmt? ParseStatement()
        {
            try
            {
                return Statement();
            }
            catch (ParseException) {RecoverFromError(); return null; }
        }


        // Statements can begin with IDENTIFIER, ref, assume, assert, param, pre, post, mut, ret
        Stmt? Statement()
        {
            // certain keywords to look for:
            // identifier -> assignment
            // assume -> assumption statement
            // assert -> assertion statement
            // confirm -> confirmation statement
            // type -> type definition
            // param -> param statement
            // ret -> return statement
            if (Check(IDENTIFIER))
            {
                return AssignmentOrCall();
            }
            else if (Check(ASSUME))
            {
                return AssumptionStatement();
            }
            else if (Check(ASSERT))
            {
                return AssertionStatement();
            }
            else if (Check(CONFIRM))
            {
                return ConfirmationStatement();
            }
            else if (Check(TYPE))
            {
                return TypeDefinition();
            }
            else if (Check(PARAM))
            {
                return ParamStatement();
            }
            else if (Check(RET))
            {
                return ReturnStatement();
            }
            else if (CheckAny(EOF, NEWLINE))
            {
                Advance(); // eat the token
                return null;
            }
            // otherwise, the statement is invalid
            Token nextToken = Peek();
            throw PanicError(nextToken, nextToken, "Expected statement.");
        }
        
        
    
        // AssignmentOrCall -> Identifier ('(' | ':' | '=') ... ;
        // The next token can be a '(', ':', or '='
        Stmt AssignmentOrCall()
        {
            IdentifierToken target = (IdentifierToken)Consume(IDENTIFIER, "Expected identifier.");
            if (Check(LEFT_PAREN))
            {
                return CallStatement(target);
            }
            else if (Check(COLON))
            {
                return DeclarationStatement(target);
            }
            else if (Check(EQUAL))
            {
                return Assignment(target);
            }
            else
            {
                Token nextToken = Peek();
                throw PanicError(nextToken, nextToken, "Expected statement.");
            }
        }

        // DeclarationStatement -> Variable ':' TypeVariable Terminator ;
        Stmt DeclarationStatement(IdentifierToken variable)
        {
            Token colon = Consume(COLON, "Expected ':'.");
            IdentifierToken targetType = (IdentifierToken)Consume(IDENTIFIER, "Expected type.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.VariableDeclaration(variable, colon, targetType, terminator);
        }

        // CallStatement -> Variable '(' CallArgumentList? ')' Terminator ;
        Stmt CallStatement(IdentifierToken function)
        {
            Token openParen = Consume(LEFT_PAREN, "Expected '('.");
            Stmt.CallArgumentList? args = CallArgumentList();
            Token closeParen = Previous;
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Call(function, openParen, args, closeParen, terminator);
        }

        // CallArgumentList -> CallArgument (',' CallArgument)* Terminator ;
        Stmt.CallArgumentList? CallArgumentList()
        {
            if (Match(RIGHT_PAREN))
            {
                // found the end argument
                return null;
            }
            Stmt.CallArgument firstArg = CallArgument();
            List<(Token Comma, Stmt.CallArgument Arg)> otherArgs = new();
            // find all arguments until the end paren
            while (!Match(RIGHT_PAREN))
            {
                Token comma = Consume(COMMA, "Expected comma.");
                Stmt.CallArgument arg = CallArgument();
                otherArgs.Add((comma, arg));
            }
            return new Stmt.CallArgumentList(firstArg, otherArgs);
        }


        // CallArgument -> Variable | 'out' Variable ;
        Stmt.CallArgument CallArgument()
        {
            if (Match(OUT))
            {
                Token outToken = Previous;
                IdentifierToken arg = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
                return new Stmt.CallArgument.Out(outToken, arg);
            }
            else
            {
                IdentifierToken arg = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
                return new Stmt.CallArgument.In(arg);
            }
            
        }

        // Assignment -> Identifier '=' ('int' | 'bool')? ... ;
        Stmt Assignment(IdentifierToken target)
        {
            Token equal = Consume(EQUAL, "Expected '='.");

            if (Check(INT))
            {
                return AssignInt(target, equal);
            }
            else if (Check(BOOL))
            {
                return AssignBool(target, equal);
            }
            else
            {
                return AssignVariable(target, equal);
            }
        }

        // AssignInt -> Variable '=' 'int' Integer Terminator ;
        Stmt AssignInt(IdentifierToken target, Token equal)
        {
            Token intToken = Consume(INT, "Expected 'int'.");
            IntegerToken value = (IntegerToken)Consume(INTEGER, "Expected an integer.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.AssignInt(target, equal, intToken, value, terminator);
        }

        // AssignBool -> Variable '=' 'bool' Boolean Terminator ;
        Stmt AssignBool(IdentifierToken target, Token equal)
        {
            Token boolToken = Consume(BOOL, "Expected 'bool'.");
            BoolToken value = (BoolToken)ConsumeAny("Expected true or false.", null, TRUE, FALSE);
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.AssignBool(target, equal, boolToken, value, terminator);
        }

        // AssignVariable -> Variable '=' Variable Terminator ;
        Stmt AssignVariable(IdentifierToken target, Token equal)
        {
            IdentifierToken value = (IdentifierToken)Consume(IDENTIFIER, "Expected value.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.AssignVariable(target, equal, value, terminator);
        }

        // AssumptionStatement -> 'assume' Variable Terminator ;
        Stmt AssumptionStatement()
        {
            Token assume = Consume(ASSUME, "Expected 'assume'.");
            IdentifierToken target = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Assumption(assume, target, terminator);
        }

        // AssertionStatement -> 'assert' Variable Terminator ;
        Stmt AssertionStatement()
        {
            Token assert = Consume(ASSERT, "Expected 'assert'.");
            IdentifierToken target = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Assertion(assert, target, terminator);
        }

        // ConfirmationStatement -> 'confirm' Variable Terminator ;
        Stmt ConfirmationStatement()
        {
            Token assert = Consume(CONFIRM, "Expected 'confirm'.");
            IdentifierToken target = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Confirmation(assert, target, terminator);
        }

        // TypeDefinition -> TypeRefinement;
        // TypeDefinition -> 'type' TypeVariable '=' ('refine') ... ;
        
        Stmt TypeDefinition()
        {
            Token type = Consume(TYPE, "Expected 'type'.");
            IdentifierToken newType = (IdentifierToken)Consume(IDENTIFIER, "Expected type variable.");
            Token equal = Consume(EQUAL, "Expected '='.");
            if (Check(REFINE))
            {
                return TypeRefinement(type, newType, equal);
            }
            Token nextToken = Peek();
            throw PanicError(nextToken, nextToken, "Expected type definition.");
        }
        
        // TypeRefinement -> 'type' TypeVariable '=' 'refine' TypeVariable Variable NewLines?
        //                   '{' NewLines Statement* NewLines? '}' NewLines ;
        Stmt TypeRefinement(Token typeToken, IdentifierToken newType, Token equal)
        {
            Token refine = Consume(REFINE, "Expected 'refine'.");
            IdentifierToken oldType = (IdentifierToken)Consume(IDENTIFIER, "Expected type variable.");
            IdentifierToken value = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            while(Match(NEWLINE)) {}
            Token openBrace = Consume(LEFT_BRACE, "Expected '{'.");
            Consume(NEWLINE, "Expected newline.");
            while(Match(NEWLINE)) {}
            List<Stmt> body = [];
            while (!Match(RIGHT_BRACE) && !IsAtEnd())
            {
                Stmt? stmt = Statement();
                if(stmt is not null)
                {
                    body.Add(stmt);
                }
            }
            Token closeBrace = Previous;
            if(closeBrace.Type != RIGHT_BRACE)
            {
                throw PanicError(closeBrace, closeBrace, "Expected '}'.");
            }
            Token terminator = ConsumeAny("Expected newline.", null, NEWLINE, EOF);
            return new Stmt.TypeRefinement(typeToken, newType, equal, refine, oldType, value, openBrace, body, closeBrace, terminator);
        }
        // `ParameterStatement -> InParameterStatement | OutParameterStatement ;`
        // `InParameterStatement -> 'param' 'in' Variable ':' TypeVariable Terminator ;`
        // `OutParameterStatement -> 'param' 'out' Variable ':' TypeVariable Terminator ;`
        Stmt ParamStatement()
        {
            Token param = Consume(PARAM, "Expected 'param'.");
            if (!CheckAny(IN, OUT))
            {
                Token nextToken = Peek();
                throw PanicError(nextToken, nextToken, "Expected either 'in' or 'out' after 'param'.");
            }
            Token inOutToken = Advance();
            IdentifierToken variable = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            Token colon = Consume(COLON, "Expected ':'.");
            IdentifierToken type = (IdentifierToken)Consume(IDENTIFIER, "Expected type.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            if(inOutToken.Type == IN)
            {
                return new Stmt.InParam(param, inOutToken, variable, colon, type, terminator);
            }
            else
            {
                return new Stmt.OutParam(param, inOutToken, variable, colon, type, terminator);
            }
        }

        // ReturnStatement -> 'ret' Terminator ;
        Stmt ReturnStatement()
        {
            Token ret = Consume(RET, "Expected 'ret'.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Return(ret, terminator);
        }
    }
}
