using DScratch.Nodes;

namespace DScratch;

public class DTransaction(DScratchDocument document)
{
    public void InsertAt(DNode node, string[] path, int offset)
    {
        var parent = FindNode(path);
        
        if (parent is null)
        {
            throw new ArgumentException("Could not find parent Node at the expected path.");
        }
        
        var origin = parent.GetChild(offset - 1);
        var rightOrigin = parent.GetChild(offset);

        node.Origin = origin;
        node.RightOrigin = rightOrigin;
        
        parent.InsertChild(node);
    }
    
    public DNode? FindNode(string[] path)
    {
        return document.FindNode(
            node: document.Page.Root, 
            path: path, 
            pathPartIndex: path.Length - 1);
    }
    
    public TNode? FindNode<TNode>(string[] path) where TNode : DNode
    {
        var node = document.FindNode(
            node: document.Page.Root, 
            path: path, 
            pathPartIndex: path.Length - 1);
        
        return node switch
        {
            TNode t => t,
            null => null,
            _ => throw new ArgumentException("Node was not of expected type.")
        };
    }
}