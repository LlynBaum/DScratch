namespace DScratch.Client;

public class NodeIdGenerator : INodeIdGenerator
{
    private long CurrentId { get; set; } = 1;
    
    public NodeId GetNextId()
    {
        return new NodeId("Darki", CurrentId++);
    }

    public NodeId TakeIds(int count)
    {
        var firstId = new NodeId("Darki", CurrentId);
        CurrentId += count;
        return firstId;
    }
}
