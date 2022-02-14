namespace OCRERL.Code.Definitions;

public class SymbolTable
{
    private readonly Dictionary<string, (object value, bool readOnly)> _symbols; 
    public readonly SymbolTable? Parent;

    public SymbolTable() => (_symbols, Parent) = (new Dictionary<string, (object value, bool readOnly)> { { "null", (0, true) } }, null);

    public object? Get(string name)
    {
        try
        {
            return _symbols[name];
        }
        catch (KeyNotFoundException)
        {
            return Parent?.Get(name);
        }
    }

    public bool Set(string name, object value, bool readOnly = false)
    {
        if (Get(name) is not null) return false;
        _symbols.Add(name, (value, readOnly));
        return true;
    }
    
    public bool SetGlobal(string name, object value) => Parent?.SetGlobal(name, value) ?? Set(name, value);

    public bool Remove(string name) => _symbols.Remove(name);
}