using DScratch.Nodes;

namespace DScratch;

public class DNodeFactory(INodeIdGenerator nodeIdGenerator)
{
    public (DCharNode first, DCharNode last) String(string value)
    {
        var id = nodeIdGenerator.GetNextId();
        var firstNode = new DCharNode(value[0], id, null, null, null);
        
        var current = firstNode;
        for (var i = 1; i < value.Length; i++)
        {
            id = nodeIdGenerator.GetNextId();
            var newNode = new DCharNode(value[i], id, current, null, null);
            current.RightOrigin = newNode;
            current = newNode;
        }

        return (firstNode, current);
    }
    
    public DCharNode Char(char value)
    {
        var id = nodeIdGenerator.GetNextId();
        return new DCharNode(value, id, null, null, null);
    }
}