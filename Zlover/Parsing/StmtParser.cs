using Blover.Zlover.Scanning;
using Blover.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Blover.Zlover.Scanning.TokenType;

namespace Blover.Zlover.Parsing
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
                return Assignment();
            }
            else if (Check(ASSUME))
            {
                return AssumptionStatement();
            }
            else if (Check(ASSERT))
            {
                return AssertionStatement();
            }
            else if (Check(DEC))
            {
                return DeclarationStatement();
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
    
        // Assignment
        Stmt Assignment()
        {
            IdentifierToken target = (IdentifierToken)Consume(IDENTIFIER, "Expected identifier.");
            Token equal = Consume(EQUAL, "Expected '='.");
            if (Check(INT))
            {
                return AssignInt(target, equal);
            }
            else if (Check(BOOL))
            {
                return AssignBool(target, equal);
            }
            else if (Check(CALL))
            {
                return CallStatement(target, equal);
            }
            else if (Check(VERIFY))
            {
                return VerificationStatement(target, equal);
            }
            else
            {
                return AssignVariable(target, equal);
            }
        }

        // DeclarationStatement -> 'dec' Variable ;
        Stmt DeclarationStatement()
        {
            Token dec = Consume(DEC, "Expected 'dec'.");
            IdentifierToken variable = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.VariableDeclaration(dec, variable, terminator);
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

        // CallStatement -> Variable '=' 'call' Integer Variable '(' CallArgumentList? ')' Terminator ;
        Stmt CallStatement(IdentifierToken target, Token equal)
        {
            Token call = Consume(CALL, "Expected 'call'.");
            IntegerToken outputNum = (IntegerToken)Consume(INTEGER, "Expected output number.");
            IdentifierToken function = (IdentifierToken)Consume(IDENTIFIER, "Expected function.");
            Token openParen = Consume(LEFT_PAREN, "Expected '('.");
            Stmt.CallArgumentList? args = CallArgumentList();
            Token closeParen = Previous;
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Call(target, equal, call, outputNum, function, openParen, args, closeParen, terminator);
        }

        // VerificationStatement -> Variable '=' 'verify' Variable '(' CallArgumentList? ')' Terminator ;
        Stmt VerificationStatement(IdentifierToken target, Token equal)
        {
            Token verify = Consume(VERIFY, "Expected 'verify'.");
            IdentifierToken function = (IdentifierToken)Consume(IDENTIFIER, "Expected function.");
            Token openParen = Consume(LEFT_PAREN, "Expected '('.");
            Stmt.CallArgumentList? args = CallArgumentList();
            Token closeParen = Previous;
            Token terminator = ConsumeAny("Expected end of line.", null, EOF, NEWLINE);
            return new Stmt.Verification(target, equal, verify, function, openParen, args, closeParen, terminator);
        }

        // CallArgumentList -> CallArgument (',' CallArgument)* Terminator ;
        Stmt.CallArgumentList? CallArgumentList()
        {
            if (Match(RIGHT_PAREN))
            {
                // found the end argument
                return null;
            }
            IdentifierToken firstArg = (IdentifierToken)Consume(IDENTIFIER, "Expected argument.");
            List<(Token Comma, IdentifierToken Arg)> otherArgs = new();
            // find all arguments until the end paren
            while (!Match(RIGHT_PAREN))
            {
                Token comma = Consume(COMMA, "Expected comma.");
                IdentifierToken arg = (IdentifierToken)Consume(IDENTIFIER, "Expected argument.");
                otherArgs.Add((comma, arg));
            }
            return new Stmt.CallArgumentList(firstArg, otherArgs);
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
