using DScratch.Nodes;

namespace DScratch;

public class DScratchDocument
{
    public DPage Page { get; }

    public DScratchDocument(DPage? page = null)
    {
        Page = page ?? DPage.Create(1);
    }

    internal DNode? FindNode(DNode? node, string[] path, int pathPartIndex)
    {
        while (true)
        {
            if (node is null) return null;

            var id = path[pathPartIndex--];
            var current = node;

            while (current is not null && current.Id != id)
            {
                current = current.RightOrigin;
            }

            if (pathPartIndex < 0) return current;
            node = node.FirstChild;
        }
    }
}