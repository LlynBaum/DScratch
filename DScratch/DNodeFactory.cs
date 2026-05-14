using DScratch.Nodes;

namespace DScratch;

public class DNodeFactory(INodeIdGenerator nodeIdGenerator)
{
    public DCharNode Char(char value)
    {
        var id = nodeIdGenerator.GetNextId();
        return new DCharNode(value, id, null, null);
    }
}