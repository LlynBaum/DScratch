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
        var index = node.Origin is not null ? FindCharNodeIndex(node.Origin) : 0;
        Value = Value.Insert(index, charNode.Value.ToString());
    }
    
    private int FindCharNodeIndex(DNode node)
    {
        var idx = 1;
        var current = Characters;
        while (true)
        {
            if (current is null) throw new ArgumentException("Could not find origin node index to insert the char value.");
            if (current.Id == node.Id) return idx;
            current = current.NextChar;
            idx++;
        }
    }
}