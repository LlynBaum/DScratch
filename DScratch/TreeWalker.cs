using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch;

public class TreeWalker<TFilter>(DNode parent) : TreeWalkerBase(parent) where TFilter : IDNode
{
    public TFilter? NextNode()
    {
        var next = Next(Current);
        while (next is not null)
        {
            if (next is TFilter filteredNode)
            {
                Current = next;
                return filteredNode;
            }

            next = Next(next);
        }

        Current = null;
        return default;
    }
}

public class TreeWalker<TFilter1, TFilter2>(DNode parent) : TreeWalkerBase(parent) where TFilter1 : IDNode where TFilter2 : IDNode
{
    public (TFilter1?, TFilter2?) NextNode()
    {
        var next = Next(Current);
        while (next is not null)
        {
            switch (next)
            {
                case TFilter1 filter1:
                    Current = next;
                    return (filter1, default);
                case TFilter2 filter2:
                    Current = next;
                    return (default, filter2);
                default:
                    next = Next(next);
                    break;
            }
        }

        Current = null;
        return default;
    }
}

public abstract class TreeWalkerBase(DNode parent)
{
    private const bool EnableDebug = false;
    
    public DNode? Current { get; protected set; } = parent;
    
    protected static DNode? Next(DNode? current)
    {
        if (current?.FirstChild is not null)
        {
            if (EnableDebug) TreeVisualizer.TraceNextStep(current, current.FirstChild);
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

            node = node.Parent;
        }

        if (EnableDebug) TreeVisualizer.TraceNextStep(current, node);
        return node;
    }
}

internal static class TreeVisualizer
{
    public static void TraceNextStep(DNode? current, DNode? next)
    {
        if (next is null)
        {
            Console.WriteLine("END OF TREE");
            return;
        }

        var depth = GetDepth(next);
        var indent = new string(' ', depth * 4); // 4 spaces per tree level
        
        // Determine the action that was taken
        var action = "➡️ START";
        if (current != null)
        {
            if (next == current.FirstChild) action = "⬇️ DOWN ";
            else if (next == current.RightOrigin) action = "➡️ RIGHT";
            else action = "⬆️ UP   "; // Backtracked to a parent's sibling
        }

        var nodeName = next.Id; 
        
        Console.WriteLine($"{indent}[{action}] -> {nodeName}");
    }

    private static int GetDepth(DNode node)
    {
        var depth = 0;
        var p = node.Parent;
        while (p is not null)
        {
            depth++;
            p = p.Parent;
        }
        return depth;
    }
}