using DScratch.Nodes.NodeTypes;

namespace DScratch.Nodes;

public class RootNode() : DNode("root", null, null), IElement
{
    public string TagName => throw new InvalidOperationException("root element has no html equivalent.");
}