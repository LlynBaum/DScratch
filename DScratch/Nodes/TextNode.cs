namespace DScratch.Nodes;

public class TextNode(NodeId id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
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

    internal TextNode? Split(int offset, NodeId nextId)
    {
        if (offset is 0) return this;
        if (offset == Length) return null;
        
        var remainingChildNodes = AllChildNodes.Take(offset).ToList();
        var otherChildNodes = AllChildNodes.Skip(offset).ToList();

        remainingChildNodes.LastOrDefault()?.RightOrigin = null;
        otherChildNodes.FirstOrDefault()?.Origin = null;
        
        AllChildNodes = remainingChildNodes;
        
        var newNode = new TextNode(nextId, this, RightOrigin, otherChildNodes);
        Parent?.InsertChild(newNode);
        otherChildNodes.ForEach(n => n.Parent = newNode);
        
        return newNode;
    }
}