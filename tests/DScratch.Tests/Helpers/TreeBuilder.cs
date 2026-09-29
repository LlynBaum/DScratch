using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.Helpers;

public class TreeBuilder : TreeBuilder.ITextTreeBuilder
{
    public RootNode Root { get; }

    public DScratchDocument Document { get; }
    
    public DNode FirstChild => Root.FirstChild!;

    private readonly DNode parent;
    private DNode? previousChild;

    public readonly TestNodeIdGenerator IdGenerator;
    private readonly DNodeFactory factory;

    public TreeBuilder(TestNodeIdGenerator? testNodeIdGenerator = null)
    {
        Root = new RootNode();
        parent = Root;
        IdGenerator = testNodeIdGenerator ?? new TestNodeIdGenerator();
        factory =  new DNodeFactory(IdGenerator);
        Document = new DScratchDocument(Root);
    }
    
    private TreeBuilder(DNode parent, TestNodeIdGenerator idGenerator, RootNode rootNode, DScratchDocument document)
    {
        Root = rootNode;
        this.parent = parent;
        IdGenerator = idGenerator;
        factory = new DNodeFactory(idGenerator);
        Document = document;
    }

    public void Print()
    {
        new TreeVisualizer(Root).Print();
    }
    
    public TextNode Text(string value)
    {
        var text = factory.String(value, previousChild, null);
        Append(text);
        return text;
    }

    public LinkNode Link(string href, string target = "_self", Action<ITextTreeBuilder>? configureChildNodes = null)
    {
        var link = factory.LinkNode(previousChild, null, href, target);
        configureChildNodes?.Invoke(GetChildTreeBuilder(link));
        Append(link);
        return link;
    }

    public ParagraphNode Paragraph(Action<ITextTreeBuilder>? configureChildNodes = null)
    {
        var paragraph = factory.Paragraph(previousChild, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(paragraph));
        Append(paragraph);
        return paragraph;
    }

    public HeadingNode Heading(HeadingLevel headingLevel, Action<ITextTreeBuilder>? configureChildNodes = null)
    {
        var heading = factory.Heading(headingLevel, previousChild, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(heading));
        Append(heading);
        return heading; 
    }

    public TestNode TestNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestNode(IdGenerator.GetNextId(), previousChild?.Id, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }
    
    public TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestInlineElementNode(IdGenerator.GetNextId(), previousChild?.Id, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }
    
    public TestBlockElementNode TestBlockElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestBlockElementNode(IdGenerator.GetNextId(), previousChild?.Id, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }

    private void Append(DNode node)
    {
        parent.InsertChild(node);
        previousChild = node;
        Document.AddNode(node);
    }

    private TreeBuilder GetChildTreeBuilder(DNode parentNode)
    {
        var builder = new TreeBuilder(parentNode, IdGenerator, Root, Document);
        return builder;
    }
    
    public interface ITreeMaker
    {
        RootNode Root { get; }

        void Print();
    }
    
    public interface ITextTreeBuilder : ITreeMaker
    {
        TextNode Text(string value);

        LinkNode Link(string href, string target = "_self", Action<ITextTreeBuilder>? configureChildNodes = null);

        TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null);
    }
}