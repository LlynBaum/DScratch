using DScratch.Nodes;

namespace DScratch.TreeVisualizers;

public class TreeVisualizer(DNode root)
{
    private const int IndentationIncrease = 4;
    private int indentation = 0;

    private string IndentationChars => string.Join("", Enumerable.Repeat(' ', indentation));

    public void Print()
    {
        Console.WriteLine("===================");
        
        var current = root;
        while (current is not null)
        {
            PrintNode(current);
            current = Next(current);
        }
        
        Console.WriteLine("=== End of Tree ===");
    }
    
    private void PrintNode(DNode node)
    {
        var deletedChar = node.IsDeleted ? " X" : "";
        
        var originId = node.Origin is not null ? node.Origin.Id : "null";
        var rightOriginId = node.RightOrigin is not null ? node.RightOrigin.Id : "null";
        switch (node)
        {
            case CharNode charNode:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}{deletedChar}) [{originId} - {rightOriginId}] --- '{charNode.Value}'");
                break;
            case TextNode textNode:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}{deletedChar}) [{originId} - {rightOriginId}] --- \"{textNode.TextContent}\"");
                break;
            default:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}{deletedChar}) [{originId} - {rightOriginId}]");
                break;
        }
    }
    
    private DNode? Next(DNode? current)
    {
        if (current?.ChildNodes.Count > 0)
        {
            indentation += IndentationIncrease;
            return current.ChildNodes[0];
        }

        var node = current;
        while (node is not null)
        {
            if (node.RightOrigin is not null)
            {
                node = node.RightOrigin;
                break;
            }

            indentation -= IndentationIncrease;
            node = node.Parent;
        }

        return node;
    }
}