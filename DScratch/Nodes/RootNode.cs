using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class RootNode() : DNode(NodeId.Root, null, null), IElement
{
    public override string TagName => throw new InvalidOperationException("root element has no html equivalent.");
}