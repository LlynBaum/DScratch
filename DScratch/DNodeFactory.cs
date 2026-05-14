using DScratch.Nodes;

namespace DScratch;

public class DNodeFactory(INodeIdGenerator nodeIdGenerator)
{
    public DCharNode Char(char value, DCharNode? origin, DCharNode? rightOrigin)
    {
        var id = nodeIdGenerator.GetNextId();
        return new DCharNode(value, id, origin, rightOrigin);
    }
}