using OCRERL.Code.Definitions;
using OCRERL.Code.Definitions.Errors;
using OCRERL.Code.Definitions.Extensions;

namespace OCRERL.Code;

public class Lexer
{
    private static string _code = ""; //TODO: Output 'Help' prompt by default
    private static string _filename = "<stdin>"; //-> The current file name, Default: '<stdin>'

    private readonly Position _pos; // Initializes the position to the beginning of the file

    private char _current = '\0'; //Initializes the current Character to 'null'

    public Lexer(string filename, string text)
    {
        _filename = filename;
        _code = text;
        _pos = new Position(-1, 0, -1, _filename, _code);
        
        Advance(); // Begins file analysis
    }

    /// <summary>
    /// Advances the Lexer
    /// </summary>
    private void Advance()
    {
        _pos.Advance(_current); // Move the current Position
        _current = _pos.Index < _code.Length
            ? _code[_pos.Index]
            : '\0'; // Checks if we have reached the end of the file
    }

    /// <summary>
    /// Tokenizes the current file
    /// </summary>
    /// <returns><see cref="Tuple{Token, Error}"/></returns> //TODO: Fix Tuple Summery Output
    public (List<Token> tokens, Error? error) Tokenize()
    {
        var tokens = new List<Token>();

        // Once a '\0' (a.k.a null) is reached, the Advance method has determined this is the end of the file.
        // We can stop reading here.
        while (_current != '\0')
        {
            if (char.IsWhiteSpace(_current))
                Advance();
            else
            {
                switch (_current)
                {
                    case var a when char.IsDigit(a): // Checks if the current character is a Digit
                    {
                        tokens.Add(MakeNumber()); // Tokenizes and parses the Digit into a Int, Float or Real
                        break;
                    }
                    case var a when char.IsLetter(a):
                    {
                        tokens.Add(MakeIdentifier());
                        break;
                    }
                    case '+':
                    {
                        tokens.Add(new Token(Tokens.Plus, null, (_pos, null))); // Tokenizes the Plus
                        Advance();
                        break;
                    }
                    case '-':
                    {
                        tokens.Add(new Token(Tokens.Subtract, null, (_pos, null))); // Tokenizes the Minus
                        Advance();
                        break;
                    }
                    case '*':
                    {
                        tokens.Add(new Token(Tokens.Multiply, null, (_pos, null))); // Tokenizes the Astrix 
                        Advance();
                        break;
                    }
                    case '/':
                    {
                        tokens.Add(new Token(Tokens.Divide, null, (_pos, null))); // Tokenizes the Forward Slash
                        Advance();
                        break;
                    }
                    case '^':
                    {
                        tokens.Add(new Token(Tokens.Exponent, null, (_pos, null)));
                        Advance();
                        break;
                    }
                    case '=':
                    {
                        tokens.Add(new Token(Tokens.Equals, null, (_pos, null)));
                        Advance();
                        break;
                    }
                    case 'M':
                    {
                        Advance();
                        if (_current == 'O')
                        {
                            Advance();
                            if (_current == 'D')
                            {
                                tokens.Add(new Token(Tokens.Modulus, null, (_pos, null)));
                                Advance();
                            }
                        }
                        break;
                    }
                    case 'D':
                    {
                        Advance();
                        if (_current == 'I')
                        {
                            Advance();
                            if (_current == 'V')
                            {
                                tokens.Add(new Token(Tokens.Quotient, null, (_pos, null)));
                                Advance();
                            }
                        }
                        break;
                    }
                    case '(':
                    {
                        tokens.Add(new Token(Tokens.LParenthesis, null,
                            (_pos, null))); // Tokenizes the Left Parenthesis 
                        Advance();
                        break;
                    }
                    case ')':
                    {
                        tokens.Add(new Token(Tokens.RParenthesis, null,
                            (_pos, null))); // Tokenizes the Right Parenthesis 
                        Advance();
                        break;
                    }

                    default: // Triggers if an unexpected character was reached
                    {
                        var startPos = (Position)_pos.Clone(); // Clones the current position
                        Advance();
                        return (new List<Token>(), new IllegalCharError((startPos, _pos))); // Throws error
                    }
                }
            }
        }

        tokens.Add(new Token(Tokens.Eof, null, (_pos, null)));
        return (tokens, null);
    }

    /// <summary>
    /// Tokenizes and parses the Digit into a Int, Float or Real
    /// </summary>
    /// <returns>A Number Token</returns>
    private Token MakeNumber()
    {
        var str = ""; // Output
        var dotCount = 0; // Keeps Track of Decimal Places
        var startPos = (Position)_pos.Clone();

        while (_current != '\0' &&
               (char.IsDigit(_current) ||
                _current == '.')) // Will continue until we reach a Unexpected Character or Null
        {
            if (_current == '.')
            {
                if (dotCount == 1)
                    break; // If we have already processed a decimal place, an Unexpected Character was reached.

                dotCount++; // Process a decimal point
                str += "."; // Update Output

            }
            else
                str += _current; // Update Output

            Advance();
        }

        /* Create a new Number token and Parse the Output to a C# Number */
        return dotCount == 0
            ? new Token(Tokens.Integer, int.Parse(str), (startPos, _pos))
            : new Token(Tokens.Float, float.Parse(str), (startPos, _pos));
    }

    private Token MakeIdentifier()
    {
        var identifier = "";
        var posStart = (Position) _pos.Clone();

        while (_current is not '\0' && char.IsLetterOrDigit(_current))
        {
            identifier += _current;
            Advance();
        }

        return new Token(Enum.IsDefined(typeof(Keywords), identifier.Capitalize()) && identifier.IsAllLower() ? Tokens.Keyword : Tokens.Identifier, identifier, (posStart, _pos));
    }

}
