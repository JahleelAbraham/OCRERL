using OCRERL.Code.Definitions.Extensions;

namespace OCRERL.Code.Definitions.Errors;

// ReSharper disable once InconsistentNaming
public class UnexpectedEOF : Error
{
    public UnexpectedEOF(string details, (Position, Position) position) : base("Unexpected EOF",
        details, position)
    { }
}