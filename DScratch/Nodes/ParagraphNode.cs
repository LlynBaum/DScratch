using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class ParagraphNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null)
    : DNode(id, origin, rightOrigin, childNodes), IElement, IShowText
{
    public string TagName => "p";

    public int Length => ActiveChildNodes.OfType<IShowText>().Sum(e => e.Length);

    internal override void InsertChild(DNode node)
    {
        if (!node.IsInlineOrText())
        {
            throw new InvalidOperationException("Can only insert TextNodes or inline elements into block node.");
        }
        
        base.InsertChild(node);
    }
}