namespace OCRERL.Code.Definitions;

public class Context
{
    public readonly string Name;
    public readonly Context? Parent;
    public readonly Position? ParentEntry;
    public SymbolTable? Symbols;

    public Context(string name, Context? parent = null, Position? parentEntry = null) =>
        (Name, Parent, ParentEntry, Symbols) = (name, parent, parentEntry, null);

    public void SetSymbolTable(SymbolTable table) => Symbols = table;
}