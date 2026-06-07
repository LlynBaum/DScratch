namespace DScratch.Tests.Helpers;

public class TestNodeIdGenerator : INodeIdGenerator
{
    private int currentId;
    
    public NodeId GetNextId()
    {
        return new NodeId("Test", currentId++);
    }

    public void TakeIds(int count)
    {
        currentId += count;
    }
}