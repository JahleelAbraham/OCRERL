namespace OCRERL.Code.Definitions.Nodes;

public class Node
{
    public Token Token;
    public (Position? Start, Position? End) Position;

    public Node(Token token, (Position? start, Position? end) position) => (Token, Position) = (token, position);
    
    public override string ToString() => Token.ToString();
}
