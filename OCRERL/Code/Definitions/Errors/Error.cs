using OCRERL.Code.Definitions.Extensions;

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
        error += $"\n\n\t{Position.start.FileContent.StringWithArrows(Position)}";
                
        return error;
    }
}