namespace DScratch.Nodes;

public class DParagraphNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent) 
    : DNode(id, origin, rightOrigin, parent, null)
{
    public override string TagName => "p";
    
    public string Value { get; private set; } = string.Empty;

    public DCharNode? Characters => (DCharNode?)FirstChild;

    internal override void InsertChild(DNode node)
    {
        if (node is not DCharNode charNode)
        {
            throw new InvalidOperationException("Can only insert DCharNode into Paragraph.");
        }
        
        base.InsertChild(node);
        var index = node.Origin is not null ? FindCharNodeIndex(node.Origin.Id) + 1 : 0;
        Value = Value.Insert(index, charNode.Value.ToString());
    }

    internal override void InsertChildRange(DNode first, DNode last)
    {
        if (first is not DCharNode firstChar)
        {
            throw new InvalidOperationException("Can only insert DCharNode into Paragraph.");
        }
        
        var str = "";
        var current = firstChar;
        while (current is not null)
        {
            str += current.Value;
            
            if (current.RightOrigin is not (null or DCharNode))
            {
                throw new InvalidOperationException("Can only insert DCharNode into Paragraph.");
            }
            
            current = current.NextChar;
        }
        
        base.InsertChildRange(first, last);
        
        var index = first.Origin is not null ? FindCharNodeIndex(first.Origin.Id) + 1 : 0;
        Value = Value.Insert(index, str);
    }

    internal override void DeleteChild(string id)
    {
        base.DeleteChild(id);
        var index = FindCharNodeIndex(id);
        Value = Value.Remove(index, 1);
    }

    private int FindCharNodeIndex(string id)
    {
        var idx = 0;
        var current = Characters;
        while (true)
        {
            if (current is null) throw new ArgumentException($"Could not find node with id \"{id}\".");
            if (current.Id == id) return idx;
            current = current.NextChar;
            idx++;
        }
    }
}