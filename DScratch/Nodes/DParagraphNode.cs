namespace DScratch.Nodes;

public class DParagraphNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent) 
    : DNode(id, origin, rightOrigin, parent, null)
{
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
    
    // TODO: override nad test InsertChildRange

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