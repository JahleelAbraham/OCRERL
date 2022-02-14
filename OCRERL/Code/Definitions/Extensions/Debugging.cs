namespace OCRERL.Code.Definitions.Extensions;

public static class Debugging
{
    public static string StringWithArrows(this string text, (Position start, Position end) position) //TODO: Find out why this crashes
    {
        var (start, end) = position;
        var result = "";

        text += "\n";

        var lines = text.Split("\n");

        // Generate each line
        var lCount = end.Line - start.Line + 1;
        
        var line = lines[start.Line];
            
        // Append to result
        result += line + '\n';

        for (var a = 0; a < start.Column + 3; a++)
            result += ' ';
        for (var a = 0; a < (end.Column - start.Column); a++)
            result += '^';

        return result.Replace("\t", "");
    }
}