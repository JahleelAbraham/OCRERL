namespace OCRERL.Code.Definitions.Nodes;

public class VariableAccessNode : Node
{
    public VariableAccessNode(Token token) : base(token, token.Position) {}
}