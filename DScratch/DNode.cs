namespace DScratch;

public abstract class DNode(DNode? origin = null, DNode? rightOrigin = null)
{
    public DNode? Origin { get; } = origin;

    public DNode? RightOrigin { get; } = rightOrigin;

    public bool IsDeleted { get; protected set; } = false;
}