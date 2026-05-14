namespace DScratch.Nodes;

public abstract class DNode(string id, DNode? origin, DNode? rightOrigin, DNode? firstChild)
{
    public string Id { get; } = id;
    
    public DNode? Origin { get; internal set; } = origin;

    public DNode? RightOrigin { get; internal set; } = rightOrigin;

    public DNode? FirstChild { get; internal set; } = firstChild;

    public bool IsDeleted { get; protected set; } = false;
}