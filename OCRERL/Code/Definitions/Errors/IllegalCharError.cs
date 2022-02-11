namespace OCRERL.Code.Definitions.Errors;

public class IllegalCharError : Error
{
    public IllegalCharError(char character, (Position, Position) position) : base("Illegal Character",
        $"Expected 'NOT_IMPLEMENT_EXCEPTION', Found '{character}'", position) //TODO: Add expected characters
    { }
}