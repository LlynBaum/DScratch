namespace DScratch.Tests.Helpers;

public class TestNodeIdGenerator : INodeIdGenerator
{
    private int currentId;
    
    public NodeId GetNextId()
    {
        return new NodeId("Test", currentId++);
    }
}