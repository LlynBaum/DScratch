using DScratch.Nodes;

namespace DScratch;

public class CrdtLookupTable
{
    // O(1) Lookup for structural elements (Paragraphs, Blocks, Wrappers)
    private readonly Dictionary<NodeId, DNode> structuralNodes = new Dictionary<NodeId, DNode>();
     
    // O(log N) Lookup for characters inside text runs, segmented by client
    private readonly Dictionary<string, List<TextNode>> nodesByClient = new Dictionary<string, List<TextNode>>();

    public void Add(DNode node)
    {
        if (node is not TextNode textNode)
        {
            structuralNodes[node.Id] = node;
            return;
        }
        
        if (!nodesByClient.TryGetValue(node.Id.Client, out var nodes))
        {
            nodesByClient[node.Id.Client] = [textNode];
            return;
        }

        var index = nodes.BinarySearch(textNode, NodeClockComparer.Instance);
        if (index < 0)
        {
            // BinarySearch returns the bitwise complement of the next larger element's index
            nodes.Insert(~index, textNode);
        }
    }

    public DNode? LookUp(NodeId nodeId)
    {
        if (structuralNodes.TryGetValue(nodeId, out var structuralNode))
        {
            return structuralNode;
        }
        
        if (!nodesByClient.TryGetValue(nodeId.Client, out var nodes) || nodes.Count == 0)
        {
            return null;
        }

        var low = 0;
        var high = nodes.Count - 1;

        while (low <= high)
        {
            var mid = low + (high - low) / 2;
            var node = nodes[mid];

            var startClock = node.Id.Clock;
            var endClock = startClock + node.Length - 1;
            
            if (nodeId.Clock >= startClock && nodeId.Clock <= endClock)
            {
                return node;
            }

            if (node.Id.Clock < nodeId.Clock)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return null;
    }

    public void Remove(DNode node)
    {
        if (node is not TextNode textNode)
        {
            structuralNodes.Remove(node.Id);
            return;
        }
        
        if (!nodesByClient.TryGetValue(node.Id.Client, out var nodes))
        {
            nodesByClient[node.Id.Client] = [textNode];
            return;
        }
        
        var index = nodes.BinarySearch(textNode, NodeClockComparer.Instance);
        if (index >= 0)
        {
            nodes.RemoveAt(index);
        }
    }
}

public class NodeClockComparer : IComparer<DNode>
{
    public static readonly NodeClockComparer Instance = new NodeClockComparer();
    public int Compare(DNode? x, DNode? y) => x!.Id.Clock.CompareTo(y!.Id.Clock);
}