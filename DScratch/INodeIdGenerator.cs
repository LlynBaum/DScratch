namespace DScratch;

public interface INodeIdGenerator
{
    public NodeId GetNextId();
    
    void TakeIds(int count);
}