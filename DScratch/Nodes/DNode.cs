using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public abstract class DNode(NodeId id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) : IDNode
{
    private readonly List<DNode> allChildNodes = childNodes ?? [];
    
    public NodeId Id { get; } = id;
    
    public DNode? Origin { get; internal set; } = origin;

    public DNode? OriginElement => Origin is IElement or null ? Origin : Origin.OriginElement;

    public DNode? RightOrigin { get; internal set; } = rightOrigin;

    public DNode? RightOriginElement => RightOrigin is IElement or null ? RightOrigin : RightOrigin.RightOriginElement;

    public bool IsDeleted { get; private set; }

    public DNode? Parent { get; internal set; }

    public DNode? ParentElement => Parent is IElement or null ? Parent : Parent.ParentElement;
    
    public IReadOnlyList<DNode> ChildNodes => allChildNodes;
    
    public IEnumerable<DNode> ActiveChildNodes => allChildNodes.Where(c => !c.IsDeleted);

    public DNode? FirstChild => ActiveChildNodes.FirstOrDefault();

    public DNode? LastChild => ActiveChildNodes.LastOrDefault();

    internal void Remove()
    {
        Origin?.RightOrigin = RightOrigin;
        RightOrigin?.Origin = Origin;
        Parent?.RemoveChild(this);
        
        Parent = null;
        RightOrigin = null;
        Origin = null;
    }
    
    private void RemoveChild(DNode node)
    {
        var index = allChildNodes.FindIndex(n => n.Id == node.Id);
        allChildNodes.RemoveAt(index);
    }

    internal void Delete()
    {
        IsDeleted = true;
        allChildNodes.ForEach(n => n.Delete());
        // TODO: some elements need to be deleted when no child is active anymore (like strong) but others not (like paragraph) how to handle?
    }

    internal void AppendChild(DNode node)
    {
        node.Parent = this;
        allChildNodes.Add(node);
    }
    
    internal virtual void InsertChild(DNode node)
    {
        node.Parent = this;
        
        if (node.Origin is null)
        {
            FirstChild?.Origin = node;
            allChildNodes.Insert(0, node);
        }
        else
        {
            var origin = node.Origin;
            origin.RightOrigin?.Origin = node;
            origin.RightOrigin = node;
            
            var index = allChildNodes.FindIndex(n => n.Id == origin.Id);
            allChildNodes.Insert(index + 1, node);
        }
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

    public override string ToString()
    {
        return $"[{Id}] - {GetType().Name}";
    }

    public DNode? GetFirstActiveOriginElement()
    {
        return OriginElement?.IsDeleted is false 
            ? OriginElement 
            : OriginElement?.GetFirstActiveOriginElement();
    }
}