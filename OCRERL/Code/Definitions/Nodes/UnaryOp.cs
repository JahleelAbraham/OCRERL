namespace OCRERL.Code.Definitions.Nodes;

public class UnaryOp : Node
{
    private Token OpToken { get; set; }
    public Node Node { get; set; }

    public UnaryOp(Token opToken, Node node) : base(new Token(Tokens.UnOp)) => (OpToken, Node) = (opToken, node);

    public override string ToString() => $"({OpToken}, {Node})";
}
