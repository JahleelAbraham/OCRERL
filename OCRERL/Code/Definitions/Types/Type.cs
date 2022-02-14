namespace OCRERL.Code.Definitions.Types;

public class Type<T> where T : Type<T>
{
    public Context? Context;
    public (Position? Start, Position? End) Position;
    public dynamic Value;
    
    public Type(dynamic value) => Value = value;

    public T SetPosition((Position? start, Position? end) position)
    {
        Position = position;
        return (T) this;
    }

    public T SetContext(Context? context = null)
    {
        Context = context;
        return (T) this;
    }

    public Type<T> Clone()
    {
        return new Type<T>(Value).SetPosition(Position).SetContext(Context);
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}