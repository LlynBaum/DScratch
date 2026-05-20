namespace DScratch.Nodes;

public abstract class DNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, List<DNode>? childNodes = null)
{
    private readonly List<DNode> childNodes = childNodes ?? [];
    
    public string Id { get; } = id;
    
    public DNode? Origin { get; internal set; } = origin;

    public DNode? RightOrigin { get; internal set; } = rightOrigin;

    public bool IsDeleted { get; private set; }

    public DNode? Parent { get; set; } = parent;
    
    public IReadOnlyList<DNode> ChildNodes => childNodes;

    public DNode? FirstChild => childNodes.FirstOrDefault();

    public DNode? LastChild => childNodes.LastOrDefault();

    internal void Remove()
    {
        Origin?.RightOrigin = RightOrigin;
        RightOrigin?.Origin = Origin;
        Parent?.RemoveChild(this);
    }
    
    private void RemoveChild(DNode node)
    {
        var index = childNodes.FindIndex(n => n.Id == node.Id);
        childNodes.RemoveAt(index);
    }

    internal void Delete()
    {
        IsDeleted = true;
    }
    
    /// <summary>
    /// Insert node as a child. The insert will be based on the origin and rightOrigin of the given node.
    /// </summary>
    /// <param name="node">The node to insert.</param>
    internal virtual void InsertChild(DNode node)
    {
        if (node.Origin is null)
        {
            FirstChild?.Origin = node;
            childNodes.Insert(0, node);
        }
        else
        {
            var insert = node.Origin;
            insert.RightOrigin?.Origin = node;
            insert.RightOrigin = node;
            
            var index = childNodes.FindIndex(n => n.Id == insert.Id);
            childNodes.Insert(index + 1, node);
        }
        
        // TODO: try merge with Origin or RightOrigin
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