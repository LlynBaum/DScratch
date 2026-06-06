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
    
    public static NodeId Root => new NodeId("root");
    
    public long Id => id!.Value;
    
    public string Client { get; }
    
    public string Value => id.HasValue ? $"{Client}-{id}" : Client;

    public override string ToString()
    {
        return Value;
    }
}