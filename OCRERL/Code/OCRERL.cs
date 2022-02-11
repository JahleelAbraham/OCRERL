using System;

namespace OCRERL.Code;

public static class Interpreter
{
    #region EntryPoint
    /* Entry Point */
    public static (Node? tokens, Error? error) Run(string code)
    {
        var lexer = new Lexer("OCRERL/Index.erl", code); //-> Initializes a new Lexer
        var lResult = lexer.Tokenize();
        
        if(lResult.error != null) return (null, lResult.error);
        
        var parser = new Parser(lResult.tokens);
        var ast = parser.Parse();

        return (ast.Node, ast.Error);
    }
    #endregion

    #region Definitions
    /* Object Class for Tokens */
    public class Token
    {
        public Tokens Type { get; set; }
        public object? Value { get; set; }

        public (Position Start, Position End) Pos;

        public Token(Tokens type, object? value = null, (Position? start, Position? end) pos = default)
        {
            (Type, Value) = (type, value);
            //-> Initializer for Token
            if (pos.start != null) Pos = ((Position) pos.start.Clone(), ((Position) pos.start.Clone()).Advance());
            if (pos.end != null) Pos.End = (Position) pos.end.Clone();
        }

        /* Cleaner Output. [OCRERL.Code.Initializer.Token] --> Type:Value */
        public override string ToString() => Value == null ? Type.ToString() : $"{Type}:{Value}";
    }
    public class Position : ICloneable
    {
        public int Index { get; set; }            //-> Character Index
        public int Line { get; set; }             //-> Current Line
        public int Column { get; set; }           //-> Current Column
        public string Filename { get; set; }      //-> Current File
        public string FileContent { get; set; }   //-> Contents of Current File

        public Position(int index, int line, int colum, string filename, string fileContent) =>
            (Index, Line, Column, Filename, FileContent) = (index, line, colum, filename, fileContent); //-> Initializer for Position

        
        /// <summary>
        /// Advances from current position in the lexer
        /// </summary>
        /// <param name="current">The current Character that's being analysed by the Lexer</param>
        /// <returns>The new Position</returns>
        public Position Advance(char current = '\0')
        {
            Index++;
            Column++;

            if (current != '\n') return this;
            
            Line++;
            Column = 0;
            
            return this;
        }

        /// <summary>
        /// Clones the current Position
        /// </summary>
        /// <returns>A Copy of the current Position</returns>
        public object Clone() => new Position(Index, Line, Column, Filename, FileContent);
    }

    public class Node
    {
        public Token Token { get; set; }
        public Node(Token token) => Token = token;

        public override string ToString() => Token.ToString();
    }
    public class NumberNode : Node
    {
        public NumberNode(Token token) : base(token)
        {
        }
    }

    public class BinOp : Node
    {
        private Node LeftNode { get; set; }
        private Node RightNode { get; set; }
        
        private Token OpToken { get; set; }

        public BinOp(Node leftNode, Token opToken, Node rightNode) : base(new Token(Tokens.BinOp)) =>
            (LeftNode, RightNode, OpToken) = (leftNode, rightNode, opToken);

        public override string ToString() => $"({LeftNode}, {OpToken}, {RightNode})";
    }

    #region ErrorDefinitions
    /* Allows for error categorisation */
    public class Error
    {
        private string Name { get; set; }
        private string Details { get; set; }
        public (Position start, Position end) Pos { get; set; }

        protected Error(string name, string details, (Position, Position) pos) => (Name, Details, Pos) = (name, details, pos);

        public override string ToString()
        {
            var error  = $"{Name}: {Details}\n";
                error += $"\tat {Pos.start.Filename}, Line {Pos.start.Line + 1}:{Pos.start.Column}";
                //error += $"\n\n\t{StringWithArrows(Pos.start.FileContent, Pos.start, Pos.end)}";
                
            return error;
        }
    }

    public class IllegalCharError : Error
    {
        public IllegalCharError(char character, (Position, Position) position) : base("Illegal Character",
            $"Expected 'NOT_IMPLEMENT_EXCEPTION', Found '{character}'", position) //TODO: Add expected characters
        { }
    }
    
    public class InvalidSyntaxError : Error
    {
        public InvalidSyntaxError(string details, (Position, Position) position) : base("Invalid Syntax",
            details, position) //TODO: Add expected characters
        { }
    }
    #endregion
    #endregion
    
    // Step 1: Tokenize the Code
    private class Lexer
    {
        private static string _code = ""; //TODO: Output 'Help' prompt by default
        private static string _filename = "<stdin>"; //-> The current file name, Default: '<stdin>'
        
        private readonly Position _pos = new(-1, 0, -1, _filename, _code); // Initializes the position to the beginning of the file
        private char _current = '\0'; //Initializes the current Character to 'null'
        
        public Lexer(string filename, string text)
        {
            _filename = filename;
            _code = text;
            Advance(); // Begins file analysis
        }

        /// <summary>
        /// Advances the Lexer
        /// </summary>
        private void Advance()
        {
            _pos.Advance(_current); // Move the current Position
            _current = _pos.Index < _code.Length ? _code[_pos.Index] : '\0'; // Checks if we have reached the end of the file
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
                            tokens.Add(MakeNumber());    // Tokenizes and parses the Digit into a Int, Float or Real
                            break;
                        }
                        case '+':
                        {
                            tokens.Add(new Token(Tokens.Plus, null, (_pos, null)));         // Tokenizes the Plus
                            Advance();
                            break;
                        }
                        case '-':
                        {
                            tokens.Add(new Token(Tokens.Subtract, null, (_pos, null)));     // Tokenizes the Minus
                            Advance();
                            break;
                        }
                        case '*':
                        {
                            tokens.Add(new Token(Tokens.Multiply, null, (_pos, null)));     // Tokenizes the Astrix 
                            Advance();
                            break;
                        }
                        case '/':
                        {
                            tokens.Add(new Token(Tokens.Divide, null, (_pos, null)));       // Tokenizes the Forward Slash
                            Advance();
                            break;
                        }
                        case '(':
                        {
                            tokens.Add(new Token(Tokens.LParenthesis, null, (_pos, null)));  // Tokenizes the Left Parenthesis 
                            Advance();
                            break;
                        }
                        case ')':
                        {
                            tokens.Add(new Token(Tokens.RParenthesis, null, (_pos, null)));  // Tokenizes the Right Parenthesis 
                            Advance();
                            break;
                        }

                        default: // Triggers if an unexpected character was reached
                        {
                            var startPos = (Position) _pos.Clone(); // Clones the current position
                            var iChar = _current; // Takes note of Unexpected character
                            return (new List<Token>(), new IllegalCharError(iChar, (startPos, _pos))); // Throws error
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
            var str = "";     // Output
            var dotCount = 0; // Keeps Track of Decimal Places
            var startPos = (Position) _pos.Clone();

            while (_current != '\0' && (char.IsDigit(_current) || _current == '.')) // Will continue until we reach a Unexpected Character or Null
            {
                if (_current == '.')
                {
                    if (dotCount == 1) break; // If we have already processed a decimal place, an Unexpected Character was reached.

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

    }
    
    // Step 2: Parse the Tokens and check for Validity
    private class Parser
    {
        public Token? CurrentToken;
        public readonly List<Token> PTokens;
        public int Index = -1;

        public Parser(List<Token> tokens)
        {
            PTokens = tokens;
            Advance();
        }

        private Token? Advance()
        {
            Index++;
            if (Index < PTokens.Count) CurrentToken = PTokens[Index];

            return CurrentToken;
        }

        public ParserResult Parse()
        {
            var res = Expression();

            if (res.Error is not null && CurrentToken!.Type is not Tokens.Eof)
                return res.Failure(new InvalidSyntaxError("Expected '+', '-', '*' or '/'", CurrentToken.Pos));

            return res;
        }
        
        private ParserResult Factor()
        {
            var res = new ParserResult();
            var token = CurrentToken;

            if (token!.Type is not (Tokens.Integer or Tokens.Float))
                return res.Failure(new InvalidSyntaxError($"Expected Integer or Float. Found {token}",
                    (token.Pos.Start, token.Pos.End)));
            
            res.Register(Advance()!);
            return res.Success(new NumberNode(token));

        }
        
        private ParserResult Term() => BinOperation(Factor, Tokens.Multiply, Tokens.Divide);

        private ParserResult Expression() => BinOperation(Term, Tokens.Plus, Tokens.Subtract);

        private ParserResult BinOperation(Func<ParserResult> func, params Tokens[] tokens)
        {
            var res = new ParserResult();
            var left = res.Register(func()).Node;

            if (res.Error != null) return res;

            while (tokens.Contains(CurrentToken!.Type))
            {
                var operation = CurrentToken;
                res.Register(Advance()!);

                var right =  res.Register(func());
                
                if (res.Error != null) return res;

                left = new BinOp(left!, operation, right.Node!);
            }

            return res.Success(left!);
        }
    }

    private class ParserResult
    {
        public Token? Token;
        public Node? Node;
        public Error? Error;

        public ParserResult() => (Node, Error) = (null, null);

        public ParserResult Register(dynamic result)
        {
            if (result.GetType() == typeof(ParserResult))
            {
                var res = (ParserResult) result;
                if (res.Error != null) Error = res.Error;
                Node = res.Node;
                return result;
            }
            if (result.GetType() == typeof(Token))
            {
                Token = (Token) result;
                return this;
            }

            return this; //TODO: Something went wrong??
        }
        
        public ParserResult Success(Node node)
        {
            Node = node;
            return this;
        }

        public ParserResult Failure(Error error)
        {
            Error = error;
            return this;
        }
    }

    public enum Tokens
    {
        // OCR Defined Types
        String,
        Integer,
        Float,
        Real,
        Boolean,

        // Arithmetic Operators
        Plus,
        Subtract,
        Multiply,
        Divide,

        // Special Tokens
        LParenthesis,
        RParenthesis,
        Eof,
        BinOp
    }
    
    //-> Extensions
    public static string StringWithArrows(string text, Position start, Position end) //TODO: Find out why this crashes
    {
        var result = "";
 
        // Calculate indices
        var iStart = Math.Max(text.LastIndexOf('\n', 0, start.Index), 0);
        var iEnd = text.IndexOf('\n', iStart + 1);
        
        if (iEnd < 0) iEnd = text.Length;
    
        // Generate each line
        var lCount = end.Line - start.Line + 1;

        for (var i = 0; i < lCount; i++)
        { 
            // Calculate line columns
            var line = text.Substring(iStart, iEnd);
            var cStart = i == 0 ? start.Column : 0;
            var cEnd = i == lCount - 1 ? end.Column : line.Length - 1;

            // Append to result
            result += line + '\n';
            result += ' ' * cStart + '^' * (cStart - cEnd);

            // Re-calculate indices
            iStart = iEnd;
            iEnd = text.IndexOf('\n', iStart + 1);
            if (iEnd < 0) iEnd = text.Length;
        }

        return result.Replace("\t", "");
    }
}