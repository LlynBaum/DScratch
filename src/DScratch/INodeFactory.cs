using DScratch.Marks;
using DScratch.Nodes;

namespace DScratch;

public interface INodeFactory
{
    INodeIdGenerator NodeIdGenerator { get; }
    
    ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin, IReadOnlyDictionary<MarkKey, string>? initMarks = null);

    ParagraphNode ParagraphFrom(DNode node);

    HeadingNode Heading(HeadingLevel headingLevel, DNode? origin, DNode? rightOrigin);

    HeadingNode HeadingFrom(DNode node, HeadingLevel headingLevel);

    LinkNode LinkNode(DNode? origin, DNode? rightOrigin, string href, string target, IReadOnlyDictionary<MarkKey, string>? initMarks = null);
    
    TextNode String(string value, DNode? origin, DNode? rightOrigin, IReadOnlyDictionary<MarkKey, string>? initMarks = null);
}