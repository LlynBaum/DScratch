namespace DScratch.Nodes;

public abstract class DNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, DNode? firstChild)
{
    public abstract string TagName { get; }
    
    public string Id { get; } = id;
    
    public DNode? Origin { get; internal set; } = origin;

    public DNode? RightOrigin { get; internal set; } = rightOrigin;

    public DNode? Parent { get; set; } = parent;

    public DNode? FirstChild { get; internal set; } = firstChild;

    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// Insert node as a child. The insert will be based on the origin and rightOrigin of the given node.
    /// </summary>
    /// <param name="node">The node to insert.</param>
    internal virtual void InsertChild(DNode node)
    {
        if (FirstChild is null)
        {
            FirstChild = node;
            return;
        }

        if (node.Origin is null)
        {
            FirstChild.Origin = node;
            FirstChild = node;
        }
        else
        {
            var insert = node.Origin;
            insert.RightOrigin?.Origin = node;
            insert.RightOrigin = node;
        }
    }
    
    internal virtual void InsertChildRange(DNode first, DNode last)
    {
        if (FirstChild is null)
        {
            FirstChild = first;
            return;
        }
        
        if (first.Origin is null)
        {
            FirstChild.Origin = last;
            FirstChild = first;
        }
        else
        {
            var insert = first.Origin;
            insert.RightOrigin?.Origin = last;
            insert.RightOrigin = first;
        }
    }

    internal virtual void DeleteChild(string id)
    {
        var current = FirstChild;
        while (current != null)
        {
            if (current.Id == id)
            {
                current.IsDeleted = true;
                break;
            }
            
            current = current.RightOrigin;
        }
    }
    
    public DNode? GetChild(int index)
    {
        if (index < 0) return null;
        
        var current = FirstChild;
        for (var i = 0; i < index; i++)
        {
            if(current is null) break;
            current = current.RightOrigin;
        }

        return current;
    }

    public TNode? GetChild<TNode>(int index) where TNode : DNode
    {
        var node = GetChild(index);
        return node switch
        {
            TNode t => t,
            null => null,
            _ => throw new ArgumentException("Node was not of expected type.")
        };
    }
    
    public NodePath GetPath()
    {
        List<string> result = [];

        var current = this;
        while (current is not null)
        {
            result.Add(current.Id);
            current = current.Parent;
        }
        
        return new NodePath(result);
    }
}