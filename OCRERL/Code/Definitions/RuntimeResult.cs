using OCRERL.Code.Definitions.Errors;

namespace OCRERL.Code.Definitions;

public class RuntimeResult
{
    public object? Value;
    public Error? Error;

    public RuntimeResult() => (Value, Error) = (null, null);

    public object? Register(dynamic res)
    {
        if (res.Error is not null) Error = res.Error;
        return res.Value;
    }

    public RuntimeResult Success(object value)
    {
        Value = value;
        return this;
    }

    public RuntimeResult Failure(Error error)
    {
        Error = error;
        return this;
    }
}