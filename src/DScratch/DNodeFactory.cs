using DScratch.Marks;
using DScratch.Nodes;

namespace DScratch;

internal class DNodeFactory(INodeIdGenerator nodeIdGenerator) : INodeFactory
{
    public INodeIdGenerator NodeIdGenerator => nodeIdGenerator;
    
    public ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin)
    {
        return new ParagraphNode(nodeIdGenerator.GetNextId(), origin, rightOrigin);
    }
    
    public ParagraphNode ParagraphFrom(DNode node)
    {
        return new ParagraphNode(node.Id, node.Origin, node.RightOrigin, [..node.ChildNodes]);
    }
    
    public HeadingNode Heading(HeadingLevel headingLevel, DNode? origin, DNode? rightOrigin)
    {
        return new HeadingNode(headingLevel, nodeIdGenerator.GetNextId(), origin, rightOrigin);
    }

    public HeadingNode HeadingFrom(DNode node, HeadingLevel headingLevel)
    {
        return new HeadingNode(headingLevel, node.Id, node.Origin, node.RightOrigin, [..node.ChildNodes]);
    }

    public TextNode String(string value, DNode? origin, DNode? rightOrigin, IReadOnlyDictionary<MarkKey, string>? initMarks = null)
    {
        if (value.Length == 0)
        {
            throw new InvalidOperationException("A TextNode must have at least 1 character.");
        }
        
        var nodeId = nodeIdGenerator.TakeIds(value.Length);
        var textNode = new TextNode(nodeId, origin, rightOrigin);
        textNode.AddText(value);
        if(initMarks is not null) textNode.CopyMarks(initMarks);
        return textNode;
    }
}