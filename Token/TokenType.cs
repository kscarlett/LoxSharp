using System;

namespace LoxSharp.Tokens
{
    enum TokenType
    {
        /* Single-character tokens */
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        COMMA,
        DOT,
        MINUS,
        PLUS,
        SEMICOLON,
        SLASH,
        ASTERISK,

        /* One or two character tokens */
        BANG,
        BANG_EQUAL,
        EQUAL,
        EQUAL_EQUAL,
        GREATER_THAN,
        GREATER_EQUAL,
        LESS_THAN,
        LESS_EQUAL,

        /* Literals */
        IDENTIFIER,
        STRING,
        NUMBER,

        /* Keywords */
        AND,
        CLASS,
        ELSE,
        FALSE,
        FUN,
        FOR,
        IF,
        NIL,
        OR,
        PRINT,
        RETURN,
        SUPER,
        THIS,
        TRUE,
        VAR,
        WHILE,

        /* Logical */
        EOF,
    }
}
