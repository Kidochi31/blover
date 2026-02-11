using Blover.Debugging;
using System.Numerics;
using System.Text;
using static Blover.Scanning.TokenType;

namespace Blover.Scanning
{
    internal class ScanError(int startLine, int startColumn, int endLine, int endColumn, string message, List<ErrorSuggestion> suggestions)
    {
        public readonly int StartLine = startLine;
        public readonly int StartColumn = startColumn;
        public readonly int EndLine = endLine;
        public readonly int EndColumn = endColumn;
        public readonly string Message = message;
        public readonly List<ErrorSuggestion> Suggestions = suggestions;

        public string VisualMessage(List<string> source)
        {
            return Debug.CreateErrorMessage(source, [new ErrorMessage(StartLine, StartColumn, Message)], [],
                [new ErrorUnderline(StartLine, StartColumn, EndLine, EndColumn, '~')], [new ErrorPointer(StartLine, StartColumn, [Message])], new ErrorSettings(1, 1, 1, 1), Suggestions);
        }
    }

    internal class Scanner
    {
        internal readonly string Source;
        internal readonly List<string> Lines = new();
        internal readonly List<Token> Tokens = new List<Token>();

        int Start = 0;
        int Current = 0;

        int StartLine = 1;
        int StartColumn = 1;

        int Line = 1;
        int Column = 1;

        public List<ScanError> Errors = new(); 

        public Scanner(string source)
        {
            Source = source;
        }

        public List<Token> ScanTokens()
        {
            List<Token> tokens = new List<Token>();
            while (!IsAtEnd())
            {
                Token token = ScanNextToken();
                tokens.Add(token);
            }
            if (tokens[^1].Type != EOF)
                tokens.Add(ScanNextToken()); //EOF
            return tokens;
        }

        public Token ScanNextToken()
        {
            while (!IsAtEnd())
            {
                Start = Current;
                StartLine = Line;
                StartColumn = Column;

                Token? possibleToken = ScanToken();
                if (possibleToken is Token)
                {
                    return possibleToken;
                }
            }
            if(Column != 1 && Line > Lines.Count)
            {
                Lines.Add(Source[(Current - Column + 1)..(Current)]);
            }
            
            return new Token(EOF, "", Line, Column, Line, Column);
        }

        Token? ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': return CreateToken(LEFT_PAREN);
                case ')': return CreateToken(RIGHT_PAREN);
                case '[': return CreateToken(LEFT_SQUARE);
                case ']': return CreateToken(RIGHT_SQUARE);
                case '{': return CreateToken(LEFT_BRACE);
                case '}': return CreateToken(RIGHT_BRACE);
                case ',': return CreateToken(COMMA);
                case ';':
                    {
                        // semicolon begins a comment until the end of the line
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                        // because it is at the end of the line
                        return null;
                    }
                case ':': return CreateToken(COLON);
                case '=': return CreateToken(EQUAL);
                case ' ':
                case '\r':
                case '\t':
                    //Ignore whitespace
                    return null;
                case '\n':
                    // new lines are tokens
                    return CreateToken(NEWLINE);
                case '@':return EscapedIdentifier();
                default:
                    if (IsDigit(c)) return Number();
                    else if (IsAlpha(c)) return SimpleIdentifier();

                    ReportError(StartLine, StartColumn, StartLine, StartColumn, $"Unexpected character '{c}'.",
                        new ErrorSuggestion([new ErrorDeleteSuggestion(StartLine, StartColumn, 1)], $"Remove '{c}' at {StartLine}:{StartColumn}."));
                    return null;
            }
        }

        Token EscapedIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance(); 

            //Trim @...
            string text = Source[(Start + 1)..Current];
            return CreateIdentifierToken(IDENTIFIER, text);
        }

        Token SimpleIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source[Start..Current];
            TokenType type = Keywords.ContainsKey(text) ? Keywords[text] : IDENTIFIER;

            if (type == TRUE) return CreateBoolToken(type, true);
            if (type == FALSE) return CreateBoolToken(type, false);

            return type == IDENTIFIER ? CreateIdentifierToken(type, text) : CreateToken(type);
        }

        Token Number()
        {
            //Find the rest of the digits, allowing _
            while (IsMiddleDigit(Peek())) Advance();

            //find decimal point, with a digit after it
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                //It is a floating point number
                //consume '.'
                Advance();

                while (IsMiddleDigit(Peek())) Advance();//find the rest of the digits

                string numberText = Source[Start..Current];
                
                return CreateFloatToken(FLOAT, double.Parse(numberText));
            }
            else //It is an integer number
            {
                string numberText = Source[Start..Current];
                return CreateIntegerToken(INTEGER, BigInteger.Parse(numberText.Replace("_", "")));
            }
        }

        char PeekNext()
        {
            if (Current + 1 >= Source.Length) return '\0';
            return Source[Current + 1];
        }

        //Lookahead at the current character
        char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source[Current];
        }

        //Will check the next char and conditionally eat it if expected
        bool Match(char expected)
        {
            if (Peek() != expected) return false;
            Advance();
            return true;
        }

        bool Consume(char expected, string message)
        {
            if (Match(expected))
            {
                return true;
            }

            ReportError(message, new ErrorSuggestion([new ErrorInsertSuggestion(Line, Column, $"{expected}")], $"Add in '{expected}'."));
            return false;
        }

        // returns current char and moves to next
        //Also increases line count if necessary
        char Advance()
        {
            char current = Source[Current++];
            Column++;
            if (current == '\n')
            {
                // add current line to lines
                Lines.Add(Source[(Current-Column+1)..(Current-1)]);
                // increment line and reset column
                Line++;
                Column = 1;
            }
            return current;
        }

        IdentifierToken CreateIdentifierToken(TokenType type, string value)
        {
            string text = Source[Start..Current];
            IdentifierToken newToken = new IdentifierToken(type, text, StartLine, StartColumn, Line, Column-1, value);
            Tokens.Add(newToken);
            return newToken;
        }

        BoolToken CreateBoolToken(TokenType type, bool value)
        {
            string text = Source[Start..Current];
            BoolToken newToken = new BoolToken(type, text, StartLine, StartColumn, Line, Column-1, value);
            Tokens.Add(newToken);
            return newToken;
        }

        FloatToken CreateFloatToken(TokenType type, double value)
        {
            string text = Source[Start..Current];
            FloatToken newToken = new FloatToken(type, text, StartLine, StartColumn, Line, Column-1, value);
            Tokens.Add(newToken);
            return newToken;
        }

        IntegerToken CreateIntegerToken(TokenType type, BigInteger value)
        {
            string text = Source[Start..Current];
            IntegerToken newToken = new IntegerToken(type, text, StartLine, StartColumn, Line, Column-1, value);
            Tokens.Add(newToken);
            return newToken;
        }


        Token CreateToken(TokenType type)
        {
            string text = Source[Start..Current];
            Token newToken = new Token(type, text, StartLine, StartColumn, Line, Column-1);
            Tokens.Add(newToken);
            return newToken;
        }

        void ReportError(string message, ErrorSuggestion? suggestion=null)
        {
            Errors.Add(new ScanError(StartLine, StartColumn, Line, Column, message, suggestion is null ? [] : [suggestion]));
        }

        void ReportError(int startLine, int startColumn, string message, ErrorSuggestion? suggestion = null)
        {
            Errors.Add(new ScanError(startLine, startColumn, Line, Column, message, suggestion is null ? [] : [suggestion]));
        }
            void ReportError(int startLine, int startColumn, int endLine, int endColumn, string message, ErrorSuggestion? suggestion = null)
            {
                Errors.Add(new ScanError(startLine, startColumn, endLine, endColumn, message, suggestion is null ? [] : [suggestion]));
            }


            bool IsAtEnd() => Current >= Source.Length;

        bool IsDigit(char c) => char.IsDigit(c);
        bool IsMiddleDigit(char c) => char.IsDigit(c) || c == '_';
        bool IsAlpha(char c) => char.IsAsciiLetter(c) || c == '_';
        bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"true", TRUE}, {"false", FALSE},
            {"if", IF}, {"else", ELSE},
            {"int", INT}, {"bool", BOOL},
            {"in", IN}, {"out", OUT}, {"param", PARAM},
            
            {"ref", REF}, {"mut", MUT},
            {"assume", ASSUME}, {"assert", ASSERT}, {"confirm", CONFIRM}, {"pre", PRE}, {"post", POST},
            {"fun", FUN}, {"ret", RET},
        };
    }
}
