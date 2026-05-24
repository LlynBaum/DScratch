using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.Tests.Helpers.TestNodes;

public class TestInlineElementNode(string id, DNode? origin, DNode? rightOrigin, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, childNodes), IInlineElement
{
    public string TagName => "testinline";
    
    public static TestInlineElementNode Empty()
    {
        return new TestInlineElementNode("", null, null);
    }
}