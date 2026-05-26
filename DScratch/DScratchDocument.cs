using DScratch.Nodes;

namespace DScratch;

public class DScratchDocument
{
    public DNode Root { get; }

    internal DScratchDocument(string initId)
    {
        Root = new RootNode();
        Root.InsertChild(new ParagraphNode(initId, null, null));
    }
    
    internal DScratchDocument(DNode root)
    {
        Root = root;
    }
    
    internal DNode? FindNode(NodePath path)
    {
        var pathPartIndex = 0;
        var node = Root;

        if (path.IsRoot) return Root;
        
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