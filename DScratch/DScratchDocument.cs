using DScratch.Nodes;

namespace DScratch;

public class DScratchDocument
{
    private Dictionary<NodeId, DNode> nodes { get; set; } = new Dictionary<NodeId, DNode>();
    
    public DNode Root { get; }

    internal DScratchDocument(string initId)
    {
        var paragraph = new ParagraphNode(initId, null, null);
        
        Root = new RootNode();
        Root.InsertChild(paragraph);
        
        nodes.Add(Root.Id, Root);
        nodes.Add(paragraph.Id, paragraph);
    }
    
    internal DScratchDocument(DNode root)
    {
        Root = root;
    }
    
    internal DNode? FindNode(NodePath path) // TODO: use look up
    {
        var pathPartIndex = 0;
        var node = Root;

        if (path.IsRoot) return Root;
        
        while (true)
        {
            var id = path[pathPartIndex++];
            var current = node;

            while (current is not null && current.Id.Value != id)
            {
                current = current.RightOrigin;
            }

            if (pathPartIndex >= path.Length) return current;
            node = current?.FirstChild;
        }
    }
}