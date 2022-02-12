using System.Reflection;
using OCRERL.Code.Definitions;
using OCRERL.Code.Definitions.Errors;
using OCRERL.Code.Definitions.Nodes;
using OCRERL.Code.Definitions.Types;

namespace OCRERL.Code;

public class Interpreter
{
    public object? Visit(Node node, Context context)
    {
        var methodName = $"Visit{node.GetType().Name}";
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null,
            new [] { typeof(Node), typeof(Context) }, null);
            
        return method is not null ? method.Invoke(this, new object?[]{node, context}) : NoVisitMethod(node, context);
    }

    private static object NoVisitMethod(Node node, Context context)
    {
        throw new Exception($"Visit Method Visit{node.GetType().Name} not found!");
    }

    private RuntimeResult VisitBinaryOp(Node rawNode, Context context)
    {
        var res = new RuntimeResult();
        
        var node = (BinaryOp)rawNode;
        
        var left = (Number)res.Register(Visit(node.LeftNode, context)!)!;
        if (res.Error is not null) return res;
        var right = (Number)res.Register(Visit(node.RightNode, context)!)!;

        (Number? value, Error? error) opResult = node.OpToken.Type switch
        {
            Tokens.Plus => left.AddedTo(right)!,
            Tokens.Subtract => left.SubtractedBy(right)!,
            Tokens.Multiply => left.MultipliedBy(right)!,
            Tokens.Divide => left.DividedBy(right)!,
            _ => (null, new RuntimeError("Somehow, an Illegal Character was found!", node.OpToken.Pos, context)) //TODO: Figure out what to do when invalid Token was found
        };

        if (opResult.error is not null) return res.Failure(opResult.error);
        else return res.Success(opResult.value!.SetPosition(node.Position.Start, node.Position.End));
    }

    private RuntimeResult VisitUnaryOp(Node rawNode, Context context)
    {
        var res = new RuntimeResult();

        var node = (UnaryOp)rawNode;

        var number = (Number)res.Register(Visit(node.Node, context)!)!;
        if (res.Error is not null) return res;

        (Number? value, Error? error) opResult;

        if (node.OpToken.Type is Tokens.Subtract) opResult = number.MultipliedBy(new Number(-1))!;
        else opResult = (number, null);
        
        if (opResult.error is not null) return res.Failure(opResult.error);
        else return res.Success(opResult.value!.SetPosition(node.Position.Start, node.Position.End));
    }

    private RuntimeResult VisitNumberNode(Node node, Context context) =>
        new RuntimeResult().Success(
            new Number(node.Token.Value!).SetContext(context).SetPosition(node.Token.Pos.Start, node.Token.Pos.End));
}
