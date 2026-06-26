using DScratch.Nodes;

namespace DScratch;

public class DScratchDocument
{
    private readonly CrdtLookupTable nodeLookup = new CrdtLookupTable();
    
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
    
    internal DNode? FindNode(NodeId nodeId) => nodeLookup.LookUp(nodeId);

    internal void AddNode(DNode node) => nodeLookup.Add(node);
    
    internal void RemoveNode(DNode node) => nodeLookup.Remove(node);
}