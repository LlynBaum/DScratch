namespace DScratch.Nodes.NodeTypes;

public static class NodeTypeHelpers
{
    extension(DNode node)
    {
        public bool IsTextOrInline() => node is TextNode or IInlineElement;
    }
}