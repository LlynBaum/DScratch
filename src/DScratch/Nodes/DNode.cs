using DScratch.Marks;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public abstract class DNode(NodeId id, NodeId? origin, NodeId? rightOrigin, List<DNode>? childNodes = null) : IDNode
{
    private readonly List<DNode> allChildNodes = childNodes ?? [];
    
    private readonly Dictionary<MarkKey, string> marks = new Dictionary<MarkKey, string>();
    
    public NodeId Id { get; } = id;
    
    public NodeId? Origin { get; } = origin;

    public NodeId? RightOrigin { get; } = rightOrigin;

    public bool IsDeleted { get; private set; }

    public DNode? Parent { get; private set; }

    public DNode? ParentElement => Parent is IElement or null ? Parent : Parent.ParentElement;
    
    public IReadOnlyList<DNode> ChildNodes => allChildNodes;
    
    public IEnumerable<DNode> ActiveChildNodes => allChildNodes.Where(c => !c.IsDeleted);

    public DNode? FirstChild => ActiveChildNodes.FirstOrDefault();

    public DNode? LastChild => ActiveChildNodes.LastOrDefault();

    public IReadOnlyDictionary<MarkKey, string> Marks => marks;
    
    internal void RemoveChild(DNode node)
    {
        var index = allChildNodes.FindIndex(n => n.Id == node.Id);
        allChildNodes.RemoveAt(index);
    }

    internal void Delete()
    {
        IsDeleted = true;
        allChildNodes.ForEach(n => n.Delete());
    }

    internal virtual void InsertChild(DNode node)
    {
        node.Parent = this;

        var originIdx = node.Origin is not null ? allChildNodes.FindIndex(n => n.Id == node.Origin) : -1;
        var rightOriginIdx = node.RightOrigin is not null ? allChildNodes.FindIndex(n => n.Id == node.RightOrigin) : allChildNodes.Count;

        var index = originIdx + 1;
        while (index < rightOriginIdx)
        {
            var currentNode = allChildNodes[index];
            var currentOriginIdx = currentNode.Origin is not null ? allChildNodes.FindIndex(n => n.Id == currentNode.Id) : -1;

            if (currentOriginIdx < originIdx)
            {
                allChildNodes.Insert(index, node);
                return;
            }
            
            if (currentOriginIdx == originIdx)
            {
                if (node.Id.IsBefore(currentNode.Id))
                {
                    allChildNodes.Insert(index, node);
                    return;
                }
            }
            
            index++;
        }
        
        allChildNodes.Insert(index, node);
    }

    public DNode? NextSibling()
    {
        var idx = Parent!.allChildNodes.FindIndex(n => n.Id == Id);
        return idx + 1 < Parent.allChildNodes.Count ? Parent.allChildNodes[idx + 1] : null;
    }

    public DNode? PreviousSibling()
    {
        var idx = Parent!.allChildNodes.FindIndex(n => n.Id == Id);
        return idx > 0 ? Parent.allChildNodes[idx - 1] : null;
    }
    
    public DNode? PreviousActiveSibling()
    {
        var previous = PreviousSibling();
        if (previous is null) return null;
        return !previous.IsDeleted 
            ? previous 
            : previous.PreviousActiveSibling();
    }
    
    internal void CopyMarks(IEnumerable<KeyValuePair<MarkKey, string>> initMarks)
    {
        marks.Clear();
        foreach (var initMark in initMarks)
        {
            marks[initMark.Key] = initMark.Value;
        }
    }
    
    internal void SetMark(MarkKey key, string value)
    {
        marks[key] = value;
    }

    internal void RemoveMark(MarkKey key)
    {
        marks.Remove(key);
    }

    public override string ToString()
    {
        return $"[{Id}] - {GetType().Name}";
    }

    public DNode GetNearestBlock()
    {
        var current = ParentElement;
        while (current is not null and not IBlockElement)
        {
            current = current.ParentElement;
        }

        return current ?? throw new InvalidOperationException("Node does not have a Block Parent.");
    }

    public IReadOnlyDictionary<MarkKey, string> GetComputedMarks()
    {
        var result = marks.ToDictionary();
        
        var current = Parent;
        while (current is not null)
        {
            foreach (var m in current.Marks)
            {
                if (!result.ContainsKey(m.Key))
                {
                    result.Add(m.Key, m.Value);
                }
            }
            
            current = current.Parent;
        }

        return result;
    }
    
    public void ReplaceChild(DNode node, DNode newNode)
    {
        var index = allChildNodes.FindIndex(n => n.Id == node.Id);
        allChildNodes[index] = newNode;
        newNode.Parent = this;
    }

    public void ClaimChildNodes()
    {
        foreach (var childNode in ChildNodes)
        {
            childNode.Parent = this;
        }
    }

    public virtual Dictionary<string, string>? GetAttributes() => null;
}