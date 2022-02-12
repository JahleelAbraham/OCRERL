namespace OCRERL.Code.Definitions.Nodes;

public class UnaryOp : Node
{
    public Token OpToken { get; set; }
    public Node Node { get; set; }

    public UnaryOp(Token opToken, Node node) :
        base(new Token(Tokens.UnOp), opToken.Pos.Start, node.Position.End) => (OpToken, Node) = (opToken, node);

    public override string ToString() => $"({OpToken}, {Node})";
}
