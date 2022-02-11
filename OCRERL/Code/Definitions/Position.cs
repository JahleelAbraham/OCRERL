namespace OCRERL.Code.Definitions;

public class Position : ICloneable
{
    public int Index { get; set; }            //-> Character Index
    public int Line { get; set; }             //-> Current Line
    public int Column { get; set; }           //-> Current Column
    public string Filename { get; set; }      //-> Current File
    public string FileContent { get; set; }   //-> Contents of Current File

    public Position(int index, int line, int colum, string filename, string fileContent) =>
        (Index, Line, Column, Filename, FileContent) = (index, line, colum, filename, fileContent); //-> Initializer for Position

        
    /// <summary>
    /// Advances from current position in the lexer
    /// </summary>
    /// <param name="current">The current Character that's being analysed by the Lexer</param>
    /// <returns>The new Position</returns>
    public Position Advance(char current = '\0')
    {
        Index++;
        Column++;

        if (current != '\n') return this;
            
        Line++;
        Column = 0;
            
        return this;
    }

    /// <summary>
    /// Clones the current Position
    /// </summary>
    /// <returns>A Copy of the current Position</returns>
    public object Clone() => new Position(Index, Line, Column, Filename, FileContent);
}
