namespace DScratch.Nodes.NodeTypes;

public static class NodeTypeHelpers
{
    extension(DNode node)
    {
        public bool IsElement() => node is IElement;
        public bool IsBlockNode() => node is IBlockElement;
        public bool IsInlineNode() => node is IInlineElement;
        public bool IsTextNode() => node is TextNode;
        public bool IsParagraphNode() => node is ParagraphNode;
        public bool IsTextOrInline() => node.IsTextNode() || node.IsInlineNode();
    }
}