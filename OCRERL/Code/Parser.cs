using OCRERL.Code.Definitions.Errors;
using OCRERL.Code.Definitions.Nodes;

namespace OCRERL.Code.Definitions;

public class Parser
{
    public Token? CurrentToken;
    public readonly List<Token> PTokens;
    public int Index = -1;

    public Parser(List<Token> tokens)
    {
        PTokens = tokens;
        Advance();
    }

    private Token? Advance()
    {
        Index++;
        if (Index < PTokens.Count) CurrentToken = PTokens[Index];

        return CurrentToken;
    }

    public ParserResult Parse()
    {
        var res = Expression();

        if (res.Error is null && CurrentToken!.Type is not Tokens.Eof)
            return res.Failure(new InvalidSyntaxError("Expected '+', '-', '*' or '/'", CurrentToken.Pos));

        return res;
    }

    private ParserResult Factor()
    {
        var res = new ParserResult();
        var token = CurrentToken;

        if (token!.Type is Tokens.Plus or Tokens.Subtract)
        {
            res.Register(Advance()!);

            var factor = res.Register(Factor());

            if (res.Error is not null) return res;

            var resS = res.Success(new UnaryOp(token, factor.Node!));

            return resS;
        }

        if (token!.Type is Tokens.Integer or Tokens.Float)
        {
            res.Register(Advance()!);
            return res.Success(new NumberNode(token));
        }

        if (token!.Type is Tokens.LParenthesis)
        {
            res.Register(Advance()!);
            var expression = res.Register(Expression());

            if (res.Error is not null) return res;

            if (CurrentToken!.Type is Tokens.RParenthesis)
            {
                res.Register(Advance()!);
                return res.Success(expression.Node!);
            }

            return res.Failure(new InvalidSyntaxError($"Expected ')'. Found {token}",
                (token.Pos.Start, token.Pos.End)));
        }

        return res.Failure(new InvalidSyntaxError($"Expected Integer or Float. Found {token}",
            (token.Pos.Start, token.Pos.End)));
    }

    private ParserResult Term() => BinOperation(Factor, Tokens.Multiply, Tokens.Divide);

    private ParserResult Expression() => BinOperation(Term, Tokens.Plus, Tokens.Subtract);

    private ParserResult BinOperation(Func<ParserResult> func, params Tokens[] tokens)
    {
        var res = new ParserResult();
        var left = res.Register(func()).Node;

        if (res.Error != null) return res;

        while (tokens.Contains(CurrentToken!.Type))
        {
            var operation = CurrentToken;
            res.Register(Advance()!);

            var right = res.Register(func());

            if (res.Error != null) return res;

            left = new BinaryOp(left!, operation, right.Node!);
        }

        return res.Success(left!);
    }
}


public class ParserResult
{
    public Token? Token;
    public Node? Node;
    public Error? Error;

    public ParserResult() => (Node, Error) = (null, null);

    public ParserResult Register(dynamic result)
    {
        if (result.GetType() == typeof(ParserResult))
        {
            var res = (ParserResult) result;
            if (res.Error != null) Error = res.Error;
            Node = res.Node;
            return result;
        }
        if (result.GetType() == typeof(Token))
        {
            Token = (Token) result;
            return this;
        }

        return this; //TODO: Something went wrong??
    }
        
    public ParserResult Success(Node node)
    {
        Node = node;
        return this;
    }

    public ParserResult Failure(Error error)
    {
        Error = error;
        return this;
    }
}
