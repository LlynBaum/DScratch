namespace DScratch;

public interface INodeIdGenerator
{
    NodeId GetNextId();
    
    NodeId TakeIds(int count);
}