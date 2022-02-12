namespace OCRERL.Code.Definitions.Errors;

public class RuntimeError : Error
{
    public Context? Context;

    public RuntimeError(string details, (Position, Position) position, Context context) : base("Runtime Error",
        details, position) => Context = context;

    private string GenerateTraceback()
    {
        var result = "";
        var position = Position.start;
        var ctx = Context;

        while (ctx is not null)
        {
            result = $"\t at {position.Filename}, Line {position.Line + 1}, in {ctx.Name}\n" + result;
            position = ctx.ParentEntry!;
            ctx = ctx.Parent;
        }
        
        return "Traceback (most recent call last):\n" + result;
    }

    public override string ToString()
    {
        var error  = $"{Name}: {Details}\n\n";
            error += GenerateTraceback();
            //  error += $"\n\n\t{StringWithArrows(Pos.start.FileContent, Pos.start, Pos.end)}";
                
        return error;
    }
}