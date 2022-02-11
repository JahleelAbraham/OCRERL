namespace OCRERL.Code.Definitions.Nodes;

public class Node
{
    public Token Token;
    public Node(Token token) => Token = token;

    public override string ToString() => Token.ToString();
}
