using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public abstract class DNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) : IDNode
{
    protected List<DNode> AllChildNodes = childNodes ?? [];
    
    public string Id { get; } = id;
    
    public DNode? Origin { get; internal set; } = origin;

    public DNode? OriginElement => Origin?.IsElement() ?? true ? Origin : Origin.OriginElement;

    public DNode? RightOrigin { get; internal set; } = rightOrigin;

    public bool IsDeleted { get; private set; }

    public DNode? Parent { get; internal set; }

    public DNode? ParentElement => Parent?.IsElement() ?? true ? Parent : Parent.ParentElement;
    
    public IReadOnlyList<DNode> ChildNodes => AllChildNodes;
    
    public IEnumerable<DNode> ActiveChildNodes => AllChildNodes.Where(c => !c.IsDeleted);

    public DNode? FirstChild => ActiveChildNodes.FirstOrDefault();

    public DNode? LastChild => ActiveChildNodes.LastOrDefault();

    internal void Remove()
    {
        Origin?.RightOrigin = RightOrigin;
        RightOrigin?.Origin = Origin;
        RightOrigin = null;
        Origin = null;
        Parent?.RemoveChild(this);
    }
    
    private void RemoveChild(DNode node)
    {
        var index = AllChildNodes.FindIndex(n => n.Id == node.Id);
        AllChildNodes.RemoveAt(index);
    }

    internal void Delete()
    {
        IsDeleted = true;
        AllChildNodes.ForEach(n => n.Delete());
        // TODO: Notify parent, if all child nodes are deleted, we can assume the whole node is deleted
    }

    internal void AppendChild(DNode node)
    {
        node.Parent = this;
        AllChildNodes.Add(node);
    }
    
    internal virtual void InsertChild(DNode node)
    {
        node.Parent = this;
        
        if (node.Origin is null)
        {
            FirstChild?.Origin = node;
            AllChildNodes.Insert(0, node);
        }
        else
        {
            var origin = node.Origin;
            origin.RightOrigin?.Origin = node;
            origin.RightOrigin = node;
            
            var index = AllChildNodes.FindIndex(n => n.Id == origin.Id);
            AllChildNodes.Insert(index + 1, node);
        }
        
        // TODO: try merge with Origin or RightOrigin
        /*
         * When merging new with another. Maybe this can work: just set the id of the new node to the node,
         * where the content was merged into. I think ToInsertStep actually would handle that, since the op is based on id
         */
    }
    
    public int IndexOf(IDNode node)
    {
        return ActiveChildNodes.ToList().FindIndex(n => n.Id == node.Id);
    }

    public DNode? ChildAt(int index)
    {
        return index < 0 
            ? null 
            : ActiveChildNodes.Skip(index).FirstOrDefault();
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

        result.Reverse();
        return new NodePath(result);
    }

    public NodePath GetElementPath()
    {
        List<string> result = [];

        var current = this;
        while (current is not null)
        {
            if (current is IElement)
            {
                result.Add(current.Id);
            }
            current = current.Parent;
        }
        
        result.Reverse();
        return new NodePath(result);
    }

    public override string ToString()
    {
        return $"[{GetPath()}] - {GetType().Name}(ID: {Id})";
    }
}