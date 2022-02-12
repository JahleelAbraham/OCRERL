using OCRERL.Code.Definitions.Errors;

namespace OCRERL.Code.Definitions.Types;

public class Number : Type<Number>
{
    public Number(object value) : base(value) { }
    
    // Arithmetic
    public (Number?, Error?) AddedTo(Type<Number> other) => (new Number(Value + other.Value).SetContext(Context), null);
    public (Number?, Error?) SubtractedBy(Type<Number> other) => (new Number(Value - other.Value).SetContext(Context), null);
    public (Number?, Error?) MultipliedBy(Type<Number> other) => (new Number(Value * other.Value).SetContext(Context), null);

    public (Number?, Error?) DividedBy(Type<Number> other)
    {
        if (other.Value == 0) return (null, new RuntimeError("Division by 0 (Zero)", other.Position!, Context));
        return (new Number(Value / other.Value).SetContext(Context), null);
    }
}