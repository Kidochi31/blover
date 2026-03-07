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
        public Decl? ParseDeclaration()
        {
            try
            {
                return Declaration();
            }
            catch (ParseException) {RecoverFromError(); return null; }
        }


        // Declaration can begin with TYPE or FUN
        Decl? Declaration()
        {
            // certain keywords to look for:
            // fun -> function definition
            if (Check(FUN))
            {
                return FunctionDefinition();
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

        Decl FunctionDefinition()
        {
            Token fun = Consume(FUN, "Expected 'fun'.");
            IdentifierToken funName = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            (Token openBrace, List<Stmt> body, Token closeBrace, Token terminator) = Block();
            Decl.Block block = new Decl.Block(openBrace, body, closeBrace);
            return new Decl.Function(fun, funName, block, terminator);
        }

        (Token OpenBrace, List<Stmt> Body, Token CloseBrace, Token Terminator) Block()
        {
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
            return (openBrace, body, closeBrace, terminator);
        }
    }
}
