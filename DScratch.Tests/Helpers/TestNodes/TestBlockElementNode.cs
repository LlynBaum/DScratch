using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestBlockElementNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IBlockElement
{
    public string TagName => "testblock";
}