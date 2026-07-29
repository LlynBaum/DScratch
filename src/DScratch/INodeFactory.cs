using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch;

public interface INodeFactory
{
    INodeIdGenerator NodeIdGenerator { get; }
    
    ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin);

    ParagraphNode ParagraphFrom(DNode node);

    HeadingNode Heading(HeadingLevel headingLevel, DNode? origin, DNode? rightOrigin);

    HeadingNode HeadingFrom(DNode node, HeadingLevel headingLevel);
    
    TextNode String(string value, DNode? origin, DNode? rightOrigin, IReadOnlyDictionary<MarkKey, string>? initMarks = null);
}