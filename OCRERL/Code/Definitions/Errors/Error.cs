namespace OCRERL.Code.Definitions.Errors;

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