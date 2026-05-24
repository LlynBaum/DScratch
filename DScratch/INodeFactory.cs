using DScratch.Nodes;

namespace DScratch;

public interface INodeFactory
{
    ParagraphNode Paragraph(DNode? origin, DNode? rightOrigin);
    
    TextNode String(string value, DNode? origin, DNode? rightOrigin);

    CharNode Char(char value, CharNode? origin, CharNode? rightOrigin);
}