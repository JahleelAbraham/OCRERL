namespace OCRERL.Code.Definitions.Types;

public class Number
{
    public Position Position;
    public int Value;

    public Number(int value)
    {
        Value = value;
    }
    
    public void SetPosition(Position newPos) {}
}