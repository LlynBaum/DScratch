namespace DScratch.Nodes;

public class TextNode(NodeId id, DNode? origin, DNode? rightOrigin) 
    : DNode(id, origin, rightOrigin)
{
    public int Length => TextContent.Length;

    public string TextContent { get; private set; } = string.Empty;

    internal override void InsertChild(DNode node)
    {
        throw new InvalidOperationException("TextNodes do not have child nodes.");
    }

    internal void AddText(string value)
    {
        TextContent += value;
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
        
        var newNode = new TextNode(nextId, this, RightOrigin);
        Parent?.InsertChild(newNode);
        otherChildNodes.ForEach(n => n.Parent = newNode);
        
        return newNode;
    }
}