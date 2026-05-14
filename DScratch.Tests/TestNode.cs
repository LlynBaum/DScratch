using DScratch.Nodes;

namespace DScratch.Tests;

public class TestNode(string id, DNode? origin, DNode? rightOrigin, DNode? firstChild) 
    : DNode(id, origin, rightOrigin, firstChild)
{
        
}