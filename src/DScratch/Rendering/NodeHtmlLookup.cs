using DScratch.Nodes;

namespace DScratch.Rendering;

internal static class NodeHtmlLookup
{
    public static string GetHtmlTag(DNode node)
    {
        return node switch
        {
            TextNode => "span",
            ParagraphNode => "p",
            HeadingNode heading => GetHeadingHtmlTag(heading.HeadingLevel),
            LinkNode => "a",
            _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
        };
    }

    private static string GetHeadingHtmlTag(HeadingLevel headingLevel)
    {
        return headingLevel switch
        {
            HeadingLevel.Level1 => "h1",
            HeadingLevel.Level2 => "h2",
            HeadingLevel.Level3 => "h3",
            HeadingLevel.Level4 => "h4",
            HeadingLevel.Level5 => "h5",
            HeadingLevel.Level6 => "h6",
            _ => throw new ArgumentOutOfRangeException(nameof(headingLevel), headingLevel, null)
        };
    }
}
