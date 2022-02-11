namespace OCRERL.Code.Definitions.Errors;

public class InvalidSyntaxError : Error
{
    public InvalidSyntaxError(string details, (Position, Position) position) : base("Invalid Syntax",
        details, position) //TODO: Add expected characters
    { }
}