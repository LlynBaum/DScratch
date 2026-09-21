using DScratch.Nodes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestNode(NodeId id, NodeId? origin, NodeId? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes)
{
    public static TestNode Empty()
    {
        return new TestNode(new NodeId("", 0), null, null);
    }
}