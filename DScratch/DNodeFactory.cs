using DScratch.Nodes;

namespace DScratch;

internal class DNodeFactory(INodeIdGenerator nodeIdGenerator) : INodeFactory
{
    public ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin)
    {
        return new ParagraphNode(nodeIdGenerator.GetNextId(), origin, rightOrigin);
    }
    
    public TextNode String(string value, DNode? origin, DNode? rightOrigin)
    {
        var textNode = new TextNode(nodeIdGenerator.GetNextId(), origin, rightOrigin);
        nodeIdGenerator.TakeIds(value.Length);
        textNode.AddText(value);
        return textNode;
    }
}