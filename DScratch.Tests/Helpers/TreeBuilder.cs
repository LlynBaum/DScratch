using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Helpers;

public class TreeBuilder : TreeBuilder.IParagraphTreeBuilder, TreeBuilder.ITextTreeBuilder
{
    public RootNode Root { get; private set; }

    public DNode FirstChild => Root.FirstChild!;

    private readonly DNode parent;
    private DNode? previousChild;

    private readonly TestNodeIdGenerator idGenerator;
    private readonly DNodeFactory factory;

    public TreeBuilder(TestNodeIdGenerator? testNodeIdGenerator = null)
    {
        Root = new RootNode();
        parent = Root;
        idGenerator = testNodeIdGenerator ?? new TestNodeIdGenerator();
        factory =  new DNodeFactory(idGenerator);
    }
    
    private TreeBuilder(DNode parent, TestNodeIdGenerator idGenerator, RootNode rootNode)
    {
        Root = rootNode;
        this.parent = parent;
        this.idGenerator = idGenerator;
        factory = new DNodeFactory(idGenerator);
    }

    public CharNode Char(char value)
    {
        var charNode = factory.Char(value, null, null);
        Append(charNode);
        return charNode;
    }

    public TextNode Text(Action<ITextTreeBuilder>? configureChildNodes = null)
    {
        var text = factory.String(string.Empty, null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(text));
        Append(text);
        return text;
    }
    
    public TextNode Text(string value)
    {
        var text = factory.String(value, null, null);
        Append(text);
        return text;
    }

    public ParagraphNode Paragraph(Action<IParagraphTreeBuilder>? configureChildNodes = null)
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
    
    public interface ITextTreeBuilder
    {
        CharNode Char(char value);
    }
    
    public interface IParagraphTreeBuilder : ITreeMaker
    {
        TextNode Text(Action<ITextTreeBuilder>? configureChildNodes = null);
        
        TextNode Text(string value);

        TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null);
    }
}