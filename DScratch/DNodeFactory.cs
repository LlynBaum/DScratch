using DScratch.Nodes;

namespace DScratch;

internal class DNodeFactory(INodeIdGenerator nodeIdGenerator) : INodeFactory
{
    public ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin)
    {
        return new ParagraphNode(nodeIdGenerator.GetNextId(), origin, rightOrigin);
    }
    
    public HeadingNode Heading(HeadingLevel headingLevel, DNode? origin, DNode? rightOrigin)
    {
        return new HeadingNode(headingLevel, nodeIdGenerator.GetNextId(), origin, rightOrigin);
    }
    
    public TextNode String(string value, DNode? origin, DNode? rightOrigin)
    {
        if (value.Length == 0)
        {
            throw new InvalidOperationException("A TextNode must have at least 1 character.");
        }
        
        var textNode = new TextNode(nodeIdGenerator.GetNextId(), origin, rightOrigin);
        nodeIdGenerator.TakeIds(value.Length - 1);
        textNode.AddText(value);
        return textNode;
    }
}