using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class ParagraphNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null)
    : DNode(id, origin, rightOrigin, childNodes), IElement, IText
{
    public string TagName => "p";
    
    public int Length => ChildNodes.Cast<IText>().Sum(t => t.Length);
    
    public string TextContent => ChildNodes.Cast<IText>().Aggregate(string.Empty, (text, node) => text + node.TextContent);

    internal override void InsertChild(DNode node)
    {
        if (node is not IText)
        {
            throw new InvalidOperationException("Can only insert TextNodes into Paragraph.");
        }
        
        base.InsertChild(node);
    }
}