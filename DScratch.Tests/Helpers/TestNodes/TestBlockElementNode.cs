using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestBlockElementNode(NodeId id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IBlockElement
{
    public override string TagName => "testblock";
    
    public static TestBlockElementNode Empty()
    {
        return new TestBlockElementNode(new NodeId("", 0), null, null);
    }
}