namespace OCRERL.Code.Definitions.Extensions;

public class Debugging
{
    public static string StringWithArrows(string text, Position start, Position end) //TODO: Find out why this crashes
    {
        var result = "";
 
        // Calculate indices
        var iStart = Math.Max(text.LastIndexOf('\n', 0, start.Index), 0);
        var iEnd = text.IndexOf('\n', iStart + 1);
        
        if (iEnd < 0) iEnd = text.Length;
    
        // Generate each line
        var lCount = end.Line - start.Line + 1;

        for (var i = 0; i < lCount; i++)
        { 
            // Calculate line columns
            var line = text.Substring(iStart, iEnd);
            var cStart = i == 0 ? start.Column : 0;
            var cEnd = i == lCount - 1 ? end.Column : line.Length - 1;

            // Append to result
            result += line + '\n';
            result += ' ' * cStart + '^' * (cStart - cEnd);

            // Re-calculate indices
            iStart = iEnd;
            iEnd = text.IndexOf('\n', iStart + 1);
            if (iEnd < 0) iEnd = text.Length;
        }

        return result.Replace("\t", "");
    }
}