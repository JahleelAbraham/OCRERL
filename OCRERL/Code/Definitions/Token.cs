namespace OCRERL.Code.Definitions;

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
    BinOp,
    UnOp
}
