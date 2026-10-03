using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch;

public class TreeWalker<TFilter>(DNode parent, bool includeDeleted = false) 
    : TreeWalkerBase(parent, includeDeleted) where TFilter : IDNode
{
    public TFilter? Node;
    
    public TFilter? NextNode(bool preventStepDown = false)
    {
        var next = Next(Current, preventStepDown);
        while (next is not null)
        {
            if (next is TFilter filteredNode)
            {
                Current = next;
                Node = filteredNode;
                return filteredNode;
            }

            next = Next(next, preventStepDown);
        }

        Node = default;
        Current = null;
        return default;
    }

    public TFilter? MovePrevious()
    {
        var next = Previous(Current);
        while (next is not null)
        {
            if (next is TFilter filteredNode)
            {
                Current = next;
                Node = filteredNode;
                return filteredNode;
            }

            next = Previous(next);
        }

        Node = default;
        Current = null;
        return default;
    }
    
    public TFilter? NextSibling()
    {
        var next = Current?.NextSibling();
        while (next is not null)
        {
            if (next is TFilter filteredNode)
            {
                Current = next;
                Node = filteredNode;
                return filteredNode;
            }

            next = next.NextSibling();
        }

        Current = null;
        return default;
    }

    public TFilter? FirstChild()
    {
        var next = Current?.FirstChild;
        while (next is not null)
        {
            if (next is TFilter filteredNode)
            {
                Current = next;
                Node = filteredNode;
                return filteredNode;
            }

            next = next.NextSibling();
        }

        Current = null;
        return default;
    }

    public static TreeWalker<T> StartFrom<T>(T node, DNode parent) where T : DNode, IDNode
    {
        return new TreeWalker<T>(parent)
        {
            Node = node,
            Current = node
        };
    }
}

public class TreeWalker<TFilter1, TFilter2>(DNode parent, bool includeDeleted = false) 
    : TreeWalkerBase(parent, includeDeleted) where TFilter1 : IDNode where TFilter2 : IDNode
{
    public (TFilter1?, TFilter2?) NextNode(bool preventStepDown = false)
    {
        var next = Next(Current, preventStepDown);
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
                    next = Next(next, preventStepDown);
                    break;
            }
        }

        Current = null;
        return default;
    }
    
    public (TFilter1?, TFilter2?) NextSibling()
    {
        var next = Current?.NextSibling();
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
                    next = next.NextSibling();
                    break;
            }
        }

        Current = null;
        return default;
    }
}

public abstract class TreeWalkerBase(DNode parent, bool includeDeleted = false)
{
    public readonly DNode Parent = parent;
    
    public DNode? Current { get; protected set; } = parent;
    
    protected DNode? Next(DNode? current, bool preventStepDown)
    {
        if (!preventStepDown && FirstChildOrDefault(current) is { } firstChild)
        {
            return NextIfDeleted(firstChild, preventStepDown);
        }
        
        if (current?.Id == Parent.Id)
        {
            return null;
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

            node = node.Parent;
            
            if (node?.Id == Parent.Id)
            {
                return null;
            }
        }

        return NextIfDeleted(node, preventStepDown);
    }
    
    protected DNode? Previous(DNode? current)
    {
        var prevSibling = current?.PreviousSibling();
        if (prevSibling is null)
        {
            return current?.Parent == Parent ? null : current?.Parent;
        }
        
        var node = prevSibling;
        while (node is not null)
        {
            node = LastChildOrDefault(node);
        }
        
        return PreviousIfDeleted(node);
    }

    private DNode? NextIfDeleted(DNode? node, bool preventStepDown)
    {
        if (includeDeleted)
        {
            return node;
        }
        return node?.IsDeleted ?? false ? Next(node, preventStepDown) : node;
    }
    
    private DNode? PreviousIfDeleted(DNode? node)
    {
        if (includeDeleted)
        {
            return node;
        }
        return node?.IsDeleted ?? false ? Previous(node) : node;
    }

    private DNode? FirstChildOrDefault(DNode? node)
    {
        return includeDeleted ? node?.FirstChild : node?.ActiveChildNodes.FirstOrDefault();
    }
    
    private DNode? LastChildOrDefault(DNode? node)
    {
        return includeDeleted ? node?.ChildNodes.LastOrDefault() : node?.LastChild;
    }
}