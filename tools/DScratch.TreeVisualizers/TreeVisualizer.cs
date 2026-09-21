using DScratch.Nodes;

namespace DScratch.TreeVisualizers;

public class TreeVisualizer(DNode root)
{
    private const int IndentationIncrease = 4;
    private int indentation;

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
        
        var originId = node.Origin is not null ? node.Origin?.Value : "null";
        var rightOriginId = node.RightOrigin is not null ? node.RightOrigin?.Value : "null";
        switch (node)
        {
            case TextNode textNode:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}{deletedChar}) [{originId} - {rightOriginId}] --- \"{textNode.TextContent}\"");
                break;
            case LinkNode linkNode:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}{deletedChar}) [{originId} - {rightOriginId}] --- Href: \"{linkNode.Href}\"");
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
            var nextSibling = node.NextSibling();
            if (nextSibling is not null)
            {
                node = nextSibling;
                break;
            }

            indentation -= IndentationIncrease;
            node = node.Parent;
        }

        return node;
    }
}