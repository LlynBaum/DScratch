namespace DScratch.Nodes;

public abstract class DNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, DNode? firstChild)
{
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
    
    /// <summary>
    /// Returns the child node of this node with the index. The index is 1 based.
    /// When child can not be found or index is 0, it returns null.
    /// </summary>
    /// <param name="index">Index of the child node</param>
    /// <returns>The child node or null when not found</returns>
    public DNode? GetChild(int index)
    {
        var current = FirstChild;
        for (var i = 0; i < index; i++)
        {
            if(current is null) break;
            current = current.RightOrigin;
        }

        return current?.Origin;
    }
    
    /// <summary>
    /// Returns the child node of this node with the index. The index is 1 based.
    /// When child can not be found or index is 0, it returns null.
    /// </summary>
    /// <param name="index">Index of the child node</param>
    /// <typeparam name="TNode">The expected type of the child node.</typeparam>
    /// <returns>The child node or null when not found</returns>
    /// <exception cref="ArgumentException">When the child node at index is not of this time.</exception>
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