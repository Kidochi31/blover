namespace Blover.Scanning
{
    internal enum TokenType
    {
        // Keyword tokens
        TRUE, FALSE,
        IF, ELSE,
        INT, BOOL,
        IN, OUT, PARAM,
        REF, MUT,
        ASSUME, ASSERT, CONFIRM, PRE, POST,
        FUN, RET,
        TYPE, REFINE,

        // Punctuator tokens
        EQUAL,
        COMMA,
        LEFT_PAREN, RIGHT_PAREN, LEFT_SQUARE, RIGHT_SQUARE, LEFT_BRACE, RIGHT_BRACE,
        COLON,
        NEWLINE,

        // Literal tokens
        IDENTIFIER,
        INTEGER,
        FLOAT,

        // End of file
        EOF
    }
}
