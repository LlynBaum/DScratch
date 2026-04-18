using System.Collections.Generic;

namespace DScratch.SourceGenerator;

public class Scanner(string source)
{
    private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "null", TokenType.Null },
        { "export", TokenType.Export },
        { "const", TokenType.Const }
    };

    private readonly List<Token> tokens = [];
    private int current;

    private int start;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token
        {
            Type = TokenType.Eof,
            Text = ""
        });
        return tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        
        switch (c)
        {
            case '{':
                AddToken(TokenType.BraceOpen);
                break;
            case '}':
                AddToken(TokenType.BraceClose);
                break;
            case '[':
                AddToken(TokenType.ArrayOpen);
                break;
            case ']':
                AddToken(TokenType.ArrayClose);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case ':':
                AddToken(TokenType.Colon);
                break;
            case '"':
                ConsumeString();
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else if (Match('*'))
                {
                    while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd())
                    {
                        Advance();
                    }

                    Advance();
                    Advance();
                }
                break;
            case ' ' or '\r' or '\t':
                break;
            default:
                if (IsDigit(c))
                {
                    ConsumeNumber();
                    break;
                }

                if (IsAlpha(c))
                {
                    ConsumeIdentifier();
                    break;
                }

                break;
        }
    }

    private void AddToken(TokenType tokenType, object? literal = null)
    {
        var text = source.Substring(start, current - start);
        tokens.Add(new Token
        {
            Type = tokenType,
            Text = text,
            Literal = literal
        });
    }

    private char Advance()
    {
        return source[current++];
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : source[current];
    }

    private char PeekNext()
    {
        return current + 1 >= source.Length ? '\0' : source[current + 1];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private void ConsumeString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            Advance();
        }

        if (IsAtEnd())
        {
            return;
        }

        Advance();

        var value = source.Substring(start + 1, current - 1 - (start + 1));
        AddToken(TokenType.String, value);
    }

    private void ConsumeNumber()
    {
        while (IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
            while (IsDigit(Peek())) Advance();
        }

        if (IsAtEnd())
        {
            return;
        }

        var value = source.Substring(start, current - start);
        AddToken(TokenType.Number, double.Parse(value));
    }

    private void ConsumeIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var identifier = source.Substring(start, current - start);
        var tokenType = GetValueOrDefault(identifier, TokenType.Identifier);
        AddToken(tokenType);
    }

    private TokenType GetValueOrDefault(string identifier, TokenType fallback)
    {
        return keywords.TryGetValue(identifier, out var type) ? type : fallback;
    }

    private static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private static bool IsAlpha(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }

    private static bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }
}