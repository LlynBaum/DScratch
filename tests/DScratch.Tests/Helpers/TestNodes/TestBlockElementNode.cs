using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestBlockElementNode(NodeId id, NodeId? origin, NodeId? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IBlockElement
{
    public static TestBlockElementNode Empty()
    {
        return new TestBlockElementNode(new NodeId("", 0), null, null);
    }
}