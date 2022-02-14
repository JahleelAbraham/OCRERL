namespace OCRERL.Code.Definitions.Errors;

public class IllegalCharError : Error
{
    public IllegalCharError((Position, Position) position) : base("Illegal Character",
        $"Invalid Token Found", position) //TODO: Add expected characters
    { }
}