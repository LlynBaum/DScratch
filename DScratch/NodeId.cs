namespace DScratch;

public readonly record struct NodeId
{
    private readonly long? clock;
    
    public NodeId(string client, long clock)
    {
        Client = client;
        this.clock = clock;
    }

    private NodeId(string client)
    {
        Client = client;
        clock = null;
    }
    
    public static NodeId Root => new NodeId("Root");
    
    public long Clock => clock!.Value;
    
    public string Client { get; }
    
    public string Value => clock.HasValue ? $"{Client}-{clock}" : Client;

    public override string ToString()
    {
        return Value;
    }

    public static NodeId FromString(string anchorId)
    {
        var parts = anchorId.Split('-');
        var id = Convert.ToInt64(parts[1]);
        return new NodeId(parts[0], id);
    }

    public bool IsContinuesTo(NodeId nodeId)
    {
        return nodeId.Client == Client && Clock + 1 == nodeId.Clock;
    }
}