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
            // type -> type definition
            // fun -> function definition
            if (Check(TYPE))
            {
                return TypeDefinition();
            }
            else if (Check(FUN))
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
            while(Match(NEWLINE)) {}
            Token openBrace = Consume(LEFT_BRACE, "Expected '{'.");
            Consume(NEWLINE, "Expected newline.");
            while(Match(NEWLINE)) {}

            List<Decl.FunctionBlock> blocks = new();

            while (!Match(RIGHT_BRACE) && !IsAtEnd())
            {
                Decl.FunctionBlock block = FunctionBlock();
                if(block is not null)
                {
                    blocks.Add(block);
                }
            }
            Token closeBrace = Previous;
            if(closeBrace.Type != RIGHT_BRACE)
            {
                throw PanicError(closeBrace, closeBrace, "Expected '}'.");
            }
            Token terminator = ConsumeAny("Expected newline.", null, NEWLINE, EOF);
            return new Decl.Function(fun, funName, openBrace, blocks, closeBrace, terminator);
        }
        
        Decl.FunctionBlock FunctionBlock()
        {
            Token start = ConsumeAny("Expected 'pre', 'post', or 'body'.", null, PRE, POST, BODY);
            (Token openBrace, List<Stmt> body, Token closeBrace, Token Terminator) = Block();
            return new Decl.FunctionBlock(start, openBrace, body, closeBrace, Terminator);

        }
        
        // TypeDefinition -> TypeRefinement;
        // TypeDefinition -> 'type' TypeVariable '=' ('refine') ... ;
        
        Decl TypeDefinition()
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
        Decl TypeRefinement(Token typeToken, IdentifierToken newType, Token equal)
        {
            Token refine = Consume(REFINE, "Expected 'refine'.");
            IdentifierToken oldType = (IdentifierToken)Consume(IDENTIFIER, "Expected type variable.");
            IdentifierToken value = (IdentifierToken)Consume(IDENTIFIER, "Expected variable.");
            (Token openBrace, List<Stmt> body, Token closeBrace, Token Terminator) = Block();
            return new Decl.TypeRefinement(typeToken, newType, equal, refine, oldType, value, openBrace, body, closeBrace, Terminator);
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
