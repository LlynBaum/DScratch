using DScratch.Nodes;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Models;

public readonly struct NodeOffset
{
    public int Offset { get; init; }

    public bool HasFound { get; init; }

    public int OffsetOrDefault => HasFound ? Offset : 0;

    public static NodeOffset Found(int offset)
    {
        return new NodeOffset
        {
            Offset = offset,
            HasFound = true
        };
    }
    
    public static NodeOffset Empty()
    {
        return new NodeOffset
        {
            Offset = -1,
            HasFound = false
        };
    }
    
    public static NodeOffset From(DNode? node, int offset)
    {
        return node is not null ? NodeOffset.Found(offset) : NodeOffset.Empty();
    }
}