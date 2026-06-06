using Microsoft.AspNetCore.Components;

namespace DScratch.Client;

public class NodeIdGenerator : INodeIdGenerator
{
    [PersistentState] 
    private long CurrentId { get; set; } = 1;
    
    public NodeId GetNextId()
    {
        return new NodeId("Darki", CurrentId++);
    }

    public void TakeIds(int count)
    {
        CurrentId += count;
    }
}
