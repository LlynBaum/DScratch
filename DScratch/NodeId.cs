namespace DScratch;

public readonly record struct NodeId
{
    public static readonly NodeId Root = new NodeId("Root");
    
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
    
    private readonly long? clock;
    
    public long Clock => clock!.Value;
    
    public string Client { get; }
    
    public string Value => clock.HasValue ? $"{Client}-{clock}" : Client;

    public bool IsRoot => this == Root;

    public override string ToString()
    {
        return Value;
    }

    public static NodeId FromString(string anchorId)
    {
        if (anchorId is "root") return Root;
        var parts = anchorId.Split('-');
        var id = Convert.ToInt64(parts[1]);
        return new NodeId(parts[0], id);
    }

    public bool IsContinuesTo(NodeId nodeId)
    {
        return nodeId.Client == Client && Clock + 1 == nodeId.Clock;
    }
}