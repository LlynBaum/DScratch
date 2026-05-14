using DScratch.Nodes;

namespace DScratch;

public class DTransaction(DScratchDocument document)
{
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