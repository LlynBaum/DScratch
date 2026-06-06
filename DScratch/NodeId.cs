namespace DScratch;

public readonly record struct NodeId
{
    private readonly long? id;
    
    public NodeId(string client, long id)
    {
        Client = client;
        this.id = id;
    }

    private NodeId(string client)
    {
        Client = client;
        id = null;
    }
    
    public static NodeId Root => new NodeId("Root");
    
    public long IdValue => id!.Value;
    
    public string Client { get; }
    
    public string Value => id.HasValue ? $"{Client}-{id}" : Client;

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
        return nodeId.Client == Client && IdValue + 1 == nodeId.IdValue;
    }
}