using DScratch.Nodes;

namespace DScratch.Tests.Helpers.Visualizers;

public class TreeVisualizer(DNode root)
{
    private const int IndentationIncrease = 4;
    private int indentation = 0;

    private string IndentationChars => string.Join("", Enumerable.Repeat(' ', indentation));

    public void Print()
    {
        var current = root;
        while (current is not null)
        {
            PrintNode(current);
            current = Next(current);
        }
    }
    
    private void PrintNode(DNode node)
    {
        switch (node)
        {
            case CharNode charNode:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}) - '{charNode.Value}'");
                break;
            case TextNode textNode:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id}) - \"{textNode.TextContent}\"");
                break;
            default:
                Console.WriteLine($"{IndentationChars}{node.GetType().Name}(ID: {node.Id})");
                break;
        }
        
        
    }
    
    private DNode? Next(DNode? current)
    {
        if (current?.FirstChild is not null)
        {
            indentation += IndentationIncrease;
            return current.FirstChild;
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