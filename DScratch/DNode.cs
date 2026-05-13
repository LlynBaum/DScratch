namespace DScratch;

public abstract class DNode(string id, DNode? origin, DNode? rightOrigin)
{
    public string Id { get; } = id;
    
    public DNode? Origin { get; internal set; } = origin;

    public DNode? RightOrigin { get; internal set; } = rightOrigin;

    public bool IsDeleted { get; protected set; } = false;
}