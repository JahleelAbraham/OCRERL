namespace OCRERL.Code.Definitions.Nodes;

public class Node
{
    public Token Token;
    public (Position? Start, Position? End) Position;

    public Node(Token token, Position? start = null, Position? end = null) => (Token, Position) = (token, (start, end));
    
    public override string ToString() => Token.ToString();
}
