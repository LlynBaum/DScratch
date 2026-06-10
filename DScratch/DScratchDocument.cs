using DScratch.Nodes;

namespace DScratch;

public class DScratchDocument
{
    private readonly Dictionary<NodeId, DNode> nodes = new Dictionary<NodeId, DNode>();
    
    public DNode Root { get; }

    internal DScratchDocument(NodeId initId)
    {
        var paragraph = new ParagraphNode(initId, null, null);
        
        Root = new RootNode();
        Root.InsertChild(paragraph);
        
        AddNode(Root);
        AddNode(paragraph);
    }
    
    internal DScratchDocument(DNode root)
    {
        Root = root;
        AddNode(Root);
    }
    
    internal DNode? FindNode(NodeId nodeId) => nodes.GetValueOrDefault(nodeId);

    public void AddNode(DNode node)
    {
        nodes[node.Id] = node;

        if (node is TextNode textNode)
        {
            for (var i = 0; i < textNode.Length; i++)
            {
                nodes[new NodeId(node.Id.Client, node.Id.IdValue + i)] = node;
            }
        }
    }
}