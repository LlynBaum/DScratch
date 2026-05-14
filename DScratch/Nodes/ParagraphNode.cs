namespace DScratch.Nodes;

public class ParagraphNode(string id, DNode? origin = null, DNode? rightOrigin = null) : DNode(id, origin, rightOrigin, null)
{
    public string Value { get; private set; } = string.Empty;

    public DCharNode? Characters => (DCharNode?)FirstChild;

    public void Insert(DCharNode node)
    {
        if (Characters is null)
        {
            FirstChild = node;
            return;
        }

        if (node.Origin is null)
        {
            // TODO: insert at beginning
        }
        else
        {
            var insert = node.Origin;
            insert.RightOrigin?.Origin = node;
            insert.RightOrigin = node;
        }
        
        var index = node.Origin is not null ? FindCharNodeIndex(node.Origin) : 0;
        Value = Value.Insert(index, node.Value.ToString());
    }

    private int FindCharNodeIndex(DNode node)
    {
        var idx = 0;
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