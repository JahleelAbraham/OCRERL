namespace OCRERL.Code.Definitions.Nodes;

public class VariableAssignNode : Node
{
    public Keywords Type;
    public Node Value;

    public VariableAssignNode(Keywords type, Token token, Node value) : base(token, (token.Position.Start, value.Position.End)) =>
        (Type, Value) = (type, value);
}