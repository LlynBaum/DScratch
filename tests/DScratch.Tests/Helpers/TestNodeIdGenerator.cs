namespace DScratch.Tests.Helpers;

public class TestNodeIdGenerator : INodeIdGenerator
{
    private int currentId;

    public int CurrentId => currentId;
    
    public NodeId GetNextId()
    {
        return new NodeId("Test", currentId++);
    }

    public NodeId TakeIds(int count)
    {
        var firstId = new NodeId("Test", currentId);
        currentId += count;
        return firstId;
    }
}