using System.Diagnostics.CodeAnalysis;
using DScratch.Nodes;

namespace DScratch.LayoutEngine;

public class ElementNode
{
    private ElementNode(string tag, string? textValue)
    {
        Tag = tag;
        TextValue = textValue;
        ChildNodes = null;
    }
    
    private ElementNode(string tag, List<ElementNode>? childNodes)
    {
        Tag = tag;
        TextValue = null;
        ChildNodes = childNodes;
    }
    
    public NodeId Id { get; set; }

    public string Tag { get; }
    
    public string? TextValue { get; }
    
    public List<ElementNode>? ChildNodes { get; }

    [MemberNotNullWhen(true, nameof(ChildNodes))]
    [MemberNotNullWhen(false, nameof(TextValue))]
    public bool HasChildNodes => ChildNodes is not null;

    public static ElementNode Create(DNode node)
    {
        if (node is TextNode textNode)
        {
            return new ElementNode(node.TagName, textNode.TextContent);
        }

        return new ElementNode(node.TagName, []);
    }

    public static ElementNode Root(DNode documentRoot)
    {
        return new ElementNode("Root", []);
    }
}