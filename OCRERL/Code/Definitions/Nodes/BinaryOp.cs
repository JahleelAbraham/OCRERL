namespace OCRERL.Code.Definitions.Nodes;

public class BinaryOp : Node
{
    public Node LeftNode;
    public Node RightNode;

    public Token OpToken;

    public BinaryOp(Node leftNode, Token opToken, Node rightNode) : base(new Token(Tokens.BinOp)) =>
        (LeftNode, RightNode, OpToken) = (leftNode, rightNode, opToken);

    public override string ToString() => $"({LeftNode}, {OpToken}, {RightNode})";
}