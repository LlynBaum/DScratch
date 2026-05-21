using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestInlineElementNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, parent, childNodes), IElement, IText
{
    public string TagName => "test";
    public int Length => ChildNodes.Cast<IText>().Sum(t => t.Length);
    
    public string TextContent => ChildNodes.Cast<IText>().Aggregate(string.Empty, (text, node) => text + node.TextContent);
}