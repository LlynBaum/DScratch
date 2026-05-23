namespace DScratch.Nodes.NodeTypes;

public static class NodeTypeHelpers
{
    public static bool IsBlockNode(this DNode node) => node is IBlockElement;

    public static bool IsInlineNode(this DNode node) => node is IInlineElement;
    
    public static bool IsTextNode(this DNode node) => node is TextNode;

    public static bool IsTextOrInline(this DNode node) => node.IsTextNode() || node.IsInlineNode();
}