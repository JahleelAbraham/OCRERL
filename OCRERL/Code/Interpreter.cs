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

    private static object NoVisitMethod(Node node, Context context) =>
        throw new Exception($"Visit Method Visit{node.GetType().Name} not found!");

        //-> Arithmetic
    private RuntimeResult VisitBinaryOp(Node rawNode, Context context)
    {
        var res = new RuntimeResult();
        
        var node = (BinaryOp)rawNode;
        
        var left = (Number)res.Register(Visit(node.LeftNode, context)!)!;
        if (res.Error is not null) return res;
        var right = (Number)res.Register(Visit(node.RightNode, context)!)!;
        if (res.Error is not null) return res;

        (Number? value, Error? error) opResult = node.OpToken.Type switch
        {
            Tokens.Plus => left.AddedTo(right)!,
            Tokens.Subtract => left.SubtractedBy(right)!,
            Tokens.Multiply => left.MultipliedBy(right)!,
            Tokens.Divide => left.DividedBy(right)!,
            Tokens.Exponent => left.Pow(right)!,
            _ => (null, new RuntimeError("Somehow, an Illegal Character was found!", node.OpToken.Position, context)) //TODO: Figure out what to do when invalid Token was found
        };

        return opResult.error is not null
            ? res.Failure(opResult.error)
            : res.Success(opResult.value!.SetPosition(node.Position));
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

        return opResult.error is not null
            ? res.Failure(opResult.error)
            : res.Success(opResult.value!.SetPosition(node.Position));
    }

    private RuntimeResult VisitNumberNode(Node node, Context context) =>
        new RuntimeResult().Success(
            new Number(node.Token.Value!).SetContext(context)
                .SetPosition(node.Token.Position));
    
     //-> Variables
     private RuntimeResult VisitVariableAccessNode(Node node, Context context)
     {
         var res = new RuntimeResult();
         var variable = node.Token.Value!.ToString();
         var result = context.Symbols!.Get(variable!);
        
         Console.WriteLine(typeof(object));

         
         if (result is null)
             return res.Failure(new RuntimeError($"Variable '{variable}' has not been defined in this scope!'",
                 node.Position!, context));
         
         return res.Success(result); //TODO: EP 4, Clone the Result
     }

     private RuntimeResult VisitVariableAssignNode(Node node, Context context)
     {
         var res = new RuntimeResult();
         var variable = node.Token.Value!.ToString();
         var value = res.Register(Visit(((VariableAssignNode) node).Value, context)!);

         if (res.Error is not null) return res;

         bool success;
         if(((VariableAssignNode) node).Type is Keywords.Global)
            success = context.Symbols!.SetGlobal(variable!, value!);
         else if(((VariableAssignNode) node).Type is Keywords.Const)
             success = context.Symbols!.Set(variable!, value!, true);
         else
             success = context.Symbols!.Set(variable!, value!);

         if (success is false)
             return res.Failure(new RuntimeError($"Variable '{variable}' has already been defined in this scope!",
                 node.Position!, context));
         return res.Success(value!);
     }
}
