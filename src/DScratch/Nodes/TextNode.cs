using DScratch.Nodes.Marks;

namespace DScratch.Nodes;

public class TextNode(NodeId id, DNode? origin, DNode? rightOrigin, string content = "") 
    : DNode(id, origin, rightOrigin)
{
    public override string TagName => "span";
    
    private readonly HashSet<Mark> marks = new HashSet<Mark>(new Mark.MarkTable());
    
    public int Length => TextContent.Length;

    public string TextContent { get; private set; } = content;

    public NodeId LastId => Length > 0 ? new NodeId(Id.Client, Id.Clock + Length - 1) : Id;

    public IReadOnlySet<Mark> Marks => marks;

    internal override void InsertChild(DNode node)
    {
        throw new InvalidOperationException("TextNodes do not have child nodes.");
    }

    internal void AddText(string value)
    {
        TextContent += value;
    }

    internal void SetMark(Mark mark)
    {
        marks.Remove(mark);
        marks.Add(mark);
    }

    internal void RemoveMark(MarkKey key)
    {
        marks.Remove(new Mark(key, string.Empty));
    }

    internal TextNode? Split(int offset, Func<NodeId> nextId)
    {
        if (offset is 0) return this;
        if (offset == Length) return null;

        var remainingText = TextContent[..offset];
        var otherText = TextContent[offset..];

        TextContent = remainingText;
        
        // TODO: copy over marks
        var newNode = new TextNode(nextId.Invoke(), this, RightOrigin)
        {
            TextContent = otherText
        };
        
        Parent?.InsertChild(newNode);
        return newNode;
    }
}