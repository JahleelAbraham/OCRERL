namespace OCRERL.Code.Definitions;

public class Context
{
    public string Name;
    public Context? Parent;
    public Position? ParentEntry;

    public Context(string name, Context? parent = null, Position? parentEntry = null) => (Name, Parent, ParentEntry) = (name, parent, parentEntry);
}