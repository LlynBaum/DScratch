using DScratch.Nodes;

namespace DScratch;

public class DTransaction(DScratchDocument document)
{
    public void InsertAt(DNode node, NodePath path, int offset)
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
        node.Parent = parent;
        
        parent.InsertChild(node);
    }
    
    public void DeleteNode(NodePath path, int selectionOffset)
    {
        var parent = FindNode(path);
        
        if (parent is null)
        {
            throw new ArgumentException("Could not find parent Node at the expected path.");
        }

        var nodeToDelete = parent.GetChild(selectionOffset);
        if (nodeToDelete is null)
        {
            throw new ArgumentException("Can not find node to delete.");
        }
        
        parent.DeleteChild(nodeToDelete.Id);
    }
    
    public DNode? FindNode(NodePath path)
    {
        return FindNodeInternal(
            node: document.Page.Root, 
            path: path);
    }
    
    public TNode? FindNode<TNode>(NodePath path) where TNode : DNode
    {
        var node = FindNodeInternal(
            node: document.Page.Root, 
            path: path);
        
        return node switch
        {
            TNode t => t,
            null => null,
            _ => throw new ArgumentException("Node was not of expected type.")
        };
    }
    
    internal static DNode? FindNodeInternal(DNode? node, NodePath path)
    {
        var pathPartIndex = 0;
        
        while (true)
        {
            if (node is null) return null;

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