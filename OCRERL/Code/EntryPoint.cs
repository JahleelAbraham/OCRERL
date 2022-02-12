using OCRERL.Code.Definitions;
using OCRERL.Code.Definitions.Errors;
using OCRERL.Code.Definitions.Types;

namespace OCRERL.Code;

public static class EntryPoint
{
    /* Entry Point */
    public static (Number? result, Error? error) Run(string code)
    {
        var lexer = new Lexer("OCRERL/Index.erl", code); //-> Initializes a new Lexer
        var lResult = lexer.Tokenize();
        
        if(lResult.error != null) return (null, lResult.error);
        
        var parser = new Parser(lResult.tokens);
        var ast = parser.Parse();

        if (ast.Error != null) return (null, ast.Error);
        
        // Run Program
        var interpreter = new Interpreter();

        var context = new Context("<OCRERL>");
        
        var result = (RuntimeResult) interpreter.Visit(ast.Node!, context)!;

        return ((Number) result.Value!, result.Error);
    }
}