using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestInlineElementNode(NodeId id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IInlineElement
{
    public override string TagName => "testinline";
    
    public static TestInlineElementNode Empty()
    {
        return new TestInlineElementNode(new NodeId("", 0), null, null);
    }
}