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
            catch (ParseException) { AdvanceUntil(NEWLINE); return null; }
        }


        // Statements can begin with IDENTIFIER, ref, assume, assert, param, pre, post, mut, ret
        Stmt? Statement()
        {
            // certain keywords to look for:
            // identifier -> assignment
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
            else if (CheckAny(EOF, NEWLINE))
            {
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
            return new Stmt.Declaration(variable, colon, targetType, terminator);
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
    }
}
