namespace DScratch;

public readonly record struct NodeId
{
    private readonly int? id;
    
    public NodeId(string client, int id)
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
    
    public int Id => id!.Value;
    
    public string Client { get; }
    
    public string Value => Client + id;

    public override string ToString()
    {
        return Value;
    }
}