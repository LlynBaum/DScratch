using DScratch.Nodes;

namespace DScratch;

public class DScratchDocument
{
    public DPage Page { get; }

    public DScratchDocument(DPage? page = null)
    {
        Page = page ?? DPage.Create(1);
    }
    
    internal DNode? FindNode(NodePath path)
    {
        var pathPartIndex = 0;
        var node = Page.Root;

        if (path.IsRoot) return Page.Root;
        
        while (true)
        {
            var id = path[pathPartIndex++];
            var current = node;

            while (current is not null && current.Id != id)
            {
                current = current.RightOrigin;
            }

            if (pathPartIndex >= path.Length) return current;
            node = current?.FirstChild;
        }
    }
}