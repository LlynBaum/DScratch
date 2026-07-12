using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch;

public interface INodeFactory
{
    ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin);

    ParagraphNode ParagraphFrom(DNode node);

    HeadingNode Heading(HeadingLevel headingLevel, DNode? origin, DNode? rightOrigin);

    HeadingNode HeadingFrom(DNode node, HeadingLevel headingLevel);
    
    TextNode String(string value, DNode? origin, DNode? rightOrigin, IReadOnlySet<Mark>? initMarks = null);
}