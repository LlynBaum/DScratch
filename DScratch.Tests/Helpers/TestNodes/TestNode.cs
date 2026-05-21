using DScratch.Nodes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, parent, childNodes)
{
    public static TestNode Empty()
    {
        return new TestNode("", null, null, null);
    }
}