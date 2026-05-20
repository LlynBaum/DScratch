using DScratch.Nodes;

namespace DScratch.Tests.Helpers;

public class TestNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, List<DNode>? childNodes = null) 
    : DNode(id, origin, rightOrigin, parent, childNodes)
{
        
}