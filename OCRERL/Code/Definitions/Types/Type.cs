namespace OCRERL.Code.Definitions.Types;

public class Type<T> where T : Type<T>
{
    public Context? Context;
    public (Position? Start, Position? End) Position;
    public dynamic Value;
    
    public Type(dynamic value) => Value = value;

    public T SetPosition(Position? start = null, Position? end = null)
    {
        Position = (start, end);
        return (T) this;
    }

    public T SetContext(Context? context = null)
    {
        Context = context;
        return (T) this;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}