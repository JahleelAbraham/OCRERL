using System.Reflection;
using OCRERL.Code.Definitions.Nodes;

namespace OCRERL.Code.Definitions;

public class Interpreter
{
    public void Visit(Node node)
    {
        var methodName = $"Visit{node.GetType().Name}";
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null,
            new Type[] { typeof(Node) }, null);
            
        if (method is null) NoVisitMethod(node);
        else method.Invoke(this, new object?[]{node}); //TODO: Figure out return value
    }

    private static void NoVisitMethod(Node node)
    {
        throw new Exception($"Visit Method Visit{node.GetType().Name} not found!");
    }

    private void VisitBinaryOp(Node rawNode)
    {
        var node = (BinaryOp)rawNode;
            
        Console.WriteLine("Found BinaryOp Node!");
        Visit(node.LeftNode);
        Visit(node.RightNode);
    }

    private void VisitUnaryOp(Node rawNode)
    {
        var node = (UnaryOp)rawNode;
            
        Console.WriteLine("Found UnaryOp Node!");
        Visit(node.Node);
    }
    private void VisitNumberNode(Node node) { Console.WriteLine("Found Number Node!"); }
}
