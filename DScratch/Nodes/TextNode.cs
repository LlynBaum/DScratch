namespace DScratch.Nodes;

public class TextNode(NodeId id, DNode? origin, DNode? rightOrigin, string content = "") 
    : DNode(id, origin, rightOrigin)
{
    public int Length => TextContent.Length;

    public string TextContent { get; private set; } = content;

    public NodeId LastId => Length > 0 ? new NodeId(Id.Client, Id.IdValue + Length - 1) : Id;

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

        var remainingText = TextContent[..offset];
        var otherText = TextContent[offset..];

        TextContent = remainingText;
        
        var newNode = new TextNode(nextId, this, RightOrigin)
        {
            TextContent = otherText
        };
        
        Parent?.InsertChild(newNode);
        return newNode;
    }
}