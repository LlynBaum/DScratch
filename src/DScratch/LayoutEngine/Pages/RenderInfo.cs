using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;

namespace DScratch.LayoutEngine.Pages;

internal sealed class RenderInfo
{
    private readonly DNode node;
    
    private RenderInfo(DNode node)
    {
        this.node = node;
    }
    
    public required DPage CurrentPage { get; set; }
    
    public ElementType ElementType { get; init; }

    public string TextValue => ElementType is ElementType.Text
        ? ((TextNode)node).TextContent
        : throw new InvalidOperationException("Can only get TextValue of a Text Node");

    public static RenderInfo Create(DNode node, DPage page)
    {
        return new RenderInfo(node)
        {
            CurrentPage = page,
            ElementType = GetElementType(node)
        };
    }

    private static ElementType GetElementType(DNode node)
    {
        return node switch
        {
            TextNode => ElementType.Text,
            IBlockTextNode => ElementType.TextBlock,
            _ => ElementType.Generic
        };
    }
}