
namespace Blover.Zlover.Scanning
{
    internal enum TokenType
    {
        // Keyword tokens
        TRUE, FALSE,
        INT, BOOL,
        ASSUME, ASSERT,
        FUN,
        DEC,
        CALL, VERIFY,

        // Punctuator tokens
        EQUAL,
        COMMA,
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        NEWLINE,

        // Literal tokens
        IDENTIFIER,
        INTEGER,
        FLOAT,

        // End of file
        EOF
    }
}