namespace DScratch.Nodes;

public class TextNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes)
{
    public int Length => ActiveChildNodes.Count();

    public string TextContent => ActiveChildNodes
        .Cast<CharNode>()
        .Aggregate(string.Empty, (text, node) => text + node.Value);

    internal override void InsertChild(DNode node)
    {
        if (node is not CharNode)
        {
            throw new InvalidOperationException("Can only insert DCharNode into TextNode.");
        }
        
        base.InsertChild(node);
    }

    internal TextNode Split(int offset, string nextId)
    {
        var remainingChildNodes = AllChildNodes.Take(offset);
        var otherChildNodes = AllChildNodes.Skip(offset).ToList();
        AllChildNodes = remainingChildNodes.ToList();
        
        var newNode = new TextNode(nextId, this, RightOrigin, otherChildNodes);
        Parent?.InsertChild(newNode);
        
        RightOrigin = newNode;
        return newNode;
    }
}