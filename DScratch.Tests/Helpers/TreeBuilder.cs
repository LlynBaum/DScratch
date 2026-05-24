using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Helpers;

public class TreeBuilder : TreeBuilder.IParagraphTreeMaker
{
    public RootNode Root { get; private set; }

    public DNode FirstChild => Root.FirstChild!;

    private readonly DNode parent;
    private DNode? previousChild;

    private readonly TestNodeIdGenerator idGenerator;
    private readonly DNodeFactory factory;

    public TreeBuilder()
    {
        Root = new RootNode();
        parent = Root;
        idGenerator = new TestNodeIdGenerator();
        factory =  new DNodeFactory(idGenerator);
    }
    
    private TreeBuilder(DNode parent, TestNodeIdGenerator idGenerator, RootNode rootNode)
    {
        Root = rootNode;
        this.parent = parent;
        this.idGenerator = idGenerator;
        factory = new DNodeFactory(idGenerator);
    }
    
    public TextNode Text(string value)
    {
        var text = factory.String(value, null, null);
        Append(text);
        return text;
    }

    public ParagraphNode Paragraph(Action<IParagraphTreeMaker>? configureChildNodes = null)
    {
        var paragraph = factory.Paragraph(null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(paragraph));
        Append(paragraph);
        return paragraph;
    }

    public TestNode TestNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestNode(idGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }
    
    public TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestInlineElementNode(idGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }
    
    public TestBlockElementNode TestBlockElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestBlockElementNode(idGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }

    private void Append(DNode node)
    {
        parent.AppendChild(node);
        node.Origin = previousChild;
        previousChild?.RightOrigin = node;
        previousChild = node;
    }

    private TreeBuilder GetChildTreeBuilder(DNode parentNode)
    {
        return new TreeBuilder(parentNode, idGenerator, Root);
    }

    public class RootNode() : DNode("root", null, null);
    
    public interface ITreeMaker
    {
        RootNode Root { get; }
    }
    
    public interface IParagraphTreeMaker : ITreeMaker
    {
        TextNode Text(string value);
    }
}