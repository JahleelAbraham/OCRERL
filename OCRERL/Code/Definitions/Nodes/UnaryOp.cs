namespace OCRERL.Code.Definitions.Nodes;

public class UnaryOp : Node
{
    public readonly Token OpToken;
    public readonly Node Node;

    public UnaryOp(Token opToken, Node node) :
        base(new Token(Tokens.UnOp), opToken.Position) => (OpToken, Node) = (opToken, node);

    public override string ToString() => $"({OpToken}, {Node})";
}
