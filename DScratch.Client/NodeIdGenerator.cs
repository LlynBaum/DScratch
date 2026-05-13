using Microsoft.AspNetCore.Components;

namespace DScratch.Client;

public class NodeIdGenerator : INodeIdGenerator
{
    [PersistentState] 
    private long CurrentId { get; set; } = 1;
    
    public string GetNextId()
    {
        return $"Darki-{CurrentId++}";
    }
}
