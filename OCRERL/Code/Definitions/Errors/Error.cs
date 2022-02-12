namespace OCRERL.Code.Definitions.Errors;

public class Error
{
    public string Name { get; set; }
    public string Details { get; set; }
    public (Position start, Position end) Position { get; set; }

    protected Error(string name, string details, (Position, Position) pos) => (Name, Details, Position) = (name, details, pos);

    public override string ToString()
    {
        var error  = $"{Name}: {Details}\n";
        error += $"\tat {Position.start.Filename}, Line {Position.start.Line + 1}:{Position.start.Column}";
        //error += $"\n\n\t{StringWithArrows(Pos.start.FileContent, Pos.start, Pos.end)}";
                
        return error;
    }
}