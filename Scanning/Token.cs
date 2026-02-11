using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Blover.Scanning
{
    internal class Token
    {
        internal readonly TokenType Type;
        internal readonly string Lexeme;
        internal readonly int StartLine;
        internal readonly int StartColumn;
        internal readonly int EndLine;
        internal readonly int EndColumn;

        public Token(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn)
        {
            Type = type;
            Lexeme = lexeme;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        public override string ToString()
        {
            return $"{Type} '{Lexeme}' of type {Type}, found at [{StartLine}:{StartColumn}, {EndLine}:{EndColumn}]";
        }
    }

    internal class IdentifierToken(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn, string name)
        : Token(type, lexeme, startLine, startColumn, endLine, endColumn)
    {
        public readonly string IdentifierName = name;

        public override string ToString()
        {
            return $"{Type} '{IdentifierName}' of type {Type}, found at [{StartLine}:{StartColumn}, {EndLine}:{EndColumn}]";
        }
    }

    internal abstract class LiteralToken(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn)
        : Token(type, lexeme, startLine, startColumn, endLine, endColumn)
    {
    }

    internal abstract class LiteralToken<T>(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn, T value)
        : LiteralToken(type, lexeme, startLine, startColumn, endLine, endColumn)
    {
        internal readonly T Value = value;

        public override string ToString()
        {
            return $"{Type} '{Lexeme}' (value:{Value}) of type {Type}, found at [{StartLine}:{StartColumn}, {EndLine}:{EndColumn}]";
        }
    }

    internal class BoolToken(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn, bool value)
        : LiteralToken<bool>(type, lexeme, startLine, startColumn, endLine, endColumn, value)
    {
    }

    internal class IntegerToken(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn, BigInteger value)
        : LiteralToken<BigInteger>(type, lexeme, startLine, startColumn, endLine, endColumn, value)
    {
    }

    internal class FloatToken(TokenType type, string lexeme, int startLine, int startColumn, int endLine, int endColumn, double value)
        : LiteralToken<double>(type, lexeme, startLine, startColumn, endLine, endColumn, value)
    {
    }
}
