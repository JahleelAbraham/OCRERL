using OCRERL.Code.Definitions;
using OCRERL.Code.Definitions.Errors;
using OCRERL.Code.Definitions.Nodes;

namespace OCRERL.Code;

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
            return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'", CurrentToken.Position));

        return res;
    }

    private ParserResult Numeral()
    {
        var res = new ParserResult();
        var token = CurrentToken;
        
        if (token!.Type is Tokens.Integer or Tokens.Float)
        {
            res.Register(Advance()!);
            return res.Success(new NumberNode(token));
        }

        if (token!.Type is Tokens.Identifier)
        {
            res.Register(Advance()!);
            return res.Success(new VariableAccessNode(token));
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

            return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                (token.Position.Start, token.Position.End)));
        }

        return res.Failure(token!.Type is Tokens.Eof
            ? new UnexpectedEOF($"Unexpected End of File",
                (token.Position.Start, token.Position.End))
            : new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                (token.Position.Start, token.Position.End)));
    }

    private ParserResult Indices() => BinOperation(Numeral, Factor, Tokens.Exponent);

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

        return Indices();
    }

    private ParserResult Term() => BinOperation(Factor, null, Tokens.Multiply, Tokens.Divide);

    private ParserResult Expression()
    {
        var res = new ParserResult();
        
        if (CurrentToken!.Matches(Tokens.Keyword, Keywords.Global, true))
        {
            res.Register(Advance()!);

            if (CurrentToken.Type is not Tokens.Identifier)
                return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                    CurrentToken.Position));

            var global = CurrentToken;
             
            res.Register(Advance()!);
            
            if (CurrentToken.Type is not Tokens.Equals)
                return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                    CurrentToken.Position));

            res.Register(Advance()!);

            var expression = res.Register(Expression());
            return res.Error is not null
                ? res
                : res.Success(new VariableAssignNode(Keywords.Global, global, expression.Node!));
        }

        if (CurrentToken!.Matches(Tokens.Keyword, Keywords.Const, true))
        {
            res.Register(Advance()!);

            if (CurrentToken.Type is not Tokens.Identifier)
                return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                    CurrentToken.Position));

            var constant = CurrentToken;
             
            res.Register(Advance()!);
            
            if (CurrentToken.Type is not Tokens.Equals)
                return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                    CurrentToken.Position));

            res.Register(Advance()!);

            var expression = res.Register(Expression());
            return res.Error is not null
                ? res
                : res.Success(new VariableAssignNode(Keywords.Const, constant, expression.Node!));
        }

        if (CurrentToken!.Matches(Tokens.Identifier))
        {
            var variable = CurrentToken;
            
            res.Register(Advance()!);

            if (CurrentToken.Type is not Tokens.Equals)
                return res.Failure(new InvalidSyntaxError($"Unexpected token '{CurrentToken}'",
                    CurrentToken.Position));

            res.Register(Advance()!);

            var expression = res.Register(Expression());
            return res.Error is not null
                ? res
                : res.Success(new VariableAssignNode(Keywords.None, variable, expression.Node!));
        }

        return BinOperation(Term, null, Tokens.Plus, Tokens.Subtract);
    }

    private ParserResult BinOperation(Func<ParserResult> funcA, Func<ParserResult>? funcB, params Tokens[] tokens)
    {
        funcB ??= funcA;
        
        var res = new ParserResult();
        var left = res.Register(funcA()).Node;

        if (res.Error != null) return res;

        while (tokens.Contains(CurrentToken!.Type))
        {
            var operation = CurrentToken;
            res.Register(Advance()!);

            var right = res.Register(funcB());

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
