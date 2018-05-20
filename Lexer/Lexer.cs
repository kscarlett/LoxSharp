using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LoxSharp.Tokens;
using LoxSharp.Error;

namespace LoxSharp.Lexing
{
    class Lexer
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
        private static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>()
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE },
        };

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Lexer(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // we are at the beginning of the next lexeme
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.ASTERISK); break;

                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS_THAN); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER_THAN); break;

                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    }
                    else if (Match('*'))
                    {
                        int commentDepth = 0;

                        while ((!(Peek() == '*' && PeekNext() == '/') && !IsAtEnd()) || commentDepth > 0)
                        {
                            if (Peek() == '\n')
                                _line++;

                            if (Peek() == '/' && PeekNext() == '*')
                                commentDepth++;

                            if (commentDepth > 0 && Peek() == '*' && PeekNext() == '/')
                                commentDepth--;

                            Advance();
                        }

                        // Eat the closing comment tokens
                        Advance();
                        Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n': _line++; break;

                case '"': ParseString(); break;

                default:
                    if (IsDigit(c))
                    {
                        ParseNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ParseIdentifier();
                    }
                    else
                    {
                        LoxError.ThrowError(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void ParseIdentifier()
        {
            // TODO: maybe add emojidentifier support? (UTF-8 for identifiers)
            while (IsAlphaNumeric(Peek()))
                Advance();

            string keywordText = _source.Substring(_start, _current - _start);
            TokenType type;

            if (!_keywords.TryGetValue(keywordText, out type))
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }

        private void ParseString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                    _line++;
                Advance();
            }

            // Unterminated string
            if (IsAtEnd())
            {
                LoxError.ThrowError(_line, "Unterminated string.");
                return;
            }

            // Closed string
            Advance();

            // Trim the surrounding quotes
            int length = _current - _start;
            string value = _source.Substring(_start + 1, length - 2);

            // Implementation specific: unescape string, so '\n' etc are supported
            value = Regex.Unescape(value);

            AddToken(TokenType.STRING, value);
        }

        private void ParseNumber()
        {
            while (IsDigit(Peek()))
                Advance();

            // Look for a fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume "."
                Advance();

                while (IsDigit(Peek()))
                    Advance();
            }

            AddToken(TokenType.NUMBER,
                double.Parse(_source.Substring(_start, _current - _start)));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
                return false;
            if (_source[_current] != expected)
                return false;

            _current++;
            return true;
        }

        private char Peek() => IsAtEnd() ? '\0' : _source[_current];

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length)
                return '\0';

            return _source[_current + 1];
        }

        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private bool IsAtEnd() => _current >= _source.Length;

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private void AddToken(TokenType type) => AddToken(type, null);

        private void AddToken(TokenType type, object literal)
        {
            string lexemeString = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, lexemeString, literal, _line));
        }
    }
}
