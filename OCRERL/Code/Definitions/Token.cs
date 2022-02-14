using OCRERL.Code.Definitions.Extensions;

namespace OCRERL.Code.Definitions;

    /* Object Class for Tokens */
public class Token
{
    public readonly Tokens Type;
    public readonly object? Value;

    public (Position Start, Position End) Position;

    public Token(Tokens type, object? value = null, (Position? start, Position? end) pos = default)
    {
        (Type, Value) = (type, value);
        //-> Initializer for Token
        var (start, end) = pos;
        if (start != null) Position = ((Position) start.Clone(), ((Position) start.Clone()).Advance());
        if (end != null) Position.End = (Position) end.Clone();
    }

    public bool Matches(Tokens type, object? value = null, bool ignoreCase = false)
    {
        if(ignoreCase && value != null && Value != null)
            return (Type, Value.ToString()!.Capitalize()) == (type, value.ToString()!.Capitalize());
        
        return value is not null ? (Type, Value) == (type, value) : Type == type;
    }

    /* Cleaner Output. [OCRERL.Code.Initializer.Token] --> Type:Value */
    public override string ToString() => Value == null ? Type.ToString() : $"{Type}:{Value}";
}

public enum Tokens
{
    // OCR Defined Types
    String,
    Integer,
    Float,
    Real,
    Boolean,
    
    // Variables
    Equals,
    Keyword,
    Identifier,

    // Arithmetic Operators
    Plus,
    Subtract,
    Multiply,
    Divide,
    Exponent,

    // Special Tokens
    LParenthesis,
    RParenthesis,
    Eof,
    BinOp,
    UnOp
}
