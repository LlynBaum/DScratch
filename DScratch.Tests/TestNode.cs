using DScratch.Nodes;

namespace DScratch.Tests;

public class TestNode(string id, DNode? origin, DNode? rightOrigin, DNode? parent, DNode? firstChild) 
    : DNode(id, origin, rightOrigin, parent, firstChild)
{
        
}