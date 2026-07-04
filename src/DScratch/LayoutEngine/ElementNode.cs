namespace DScratch.LayoutEngine;

public class ElementNode
{
    public ElementNode(string tag, string? textValue)
    {
        Tag = tag;
        TextValue = textValue;
        ChildNodes = null;
    }
    
    public ElementNode(string tag, IReadOnlyList<ElementNode>? childNodes)
    {
        Tag = tag;
        TextValue = null;
        ChildNodes = childNodes;
    }

    public string Tag { get; }
    
    public string? TextValue { get; }
    
    public IReadOnlyList<ElementNode>? ChildNodes { get; }

    public bool IsTextNode => TextValue is not null;
}