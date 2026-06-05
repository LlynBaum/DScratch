namespace DScratch;

public readonly record struct NodeId(string Client, int Id)
{
    public string Value { get; } = Client + Id;

    public override string ToString()
    {
        return Value;
    }
}