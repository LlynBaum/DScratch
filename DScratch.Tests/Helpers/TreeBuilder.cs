using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.Helpers;

public class TreeBuilder : TreeBuilder.IBlockTextTreeBuilder
{
    public RootNode Root { get; }

    public DNode FirstChild => Root.FirstChild!;

    public event Action<DNode>? NodeAdded; 

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
    }
    
    private TreeBuilder(DNode parent, TestNodeIdGenerator idGenerator, RootNode rootNode)
    {
        Root = rootNode;
        this.parent = parent;
        this.IdGenerator = idGenerator;
        factory = new DNodeFactory(idGenerator);
    }

    public DScratchDocument CreateDocument()
    {
        var document = new DScratchDocument(Root);
        NodeAdded += document.AddNode;
        return document;
    }

    public void Print()
    {
        new TreeVisualizer(Root).Print();
    }
    
    public TextNode Text(string value)
    {
        var text = factory.String(value, null, null);
        Append(text);
        return text;
    }

    public ParagraphNode Paragraph(Action<IBlockTextTreeBuilder>? configureChildNodes = null)
    {
        var paragraph = factory.Paragraph(null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(paragraph));
        Append(paragraph);
        return paragraph;
    }

    public HeadingNode Heading(HeadingLevel headingLevel, Action<IBlockTextTreeBuilder>? configureChildNodes = null)
    {
        var heading = factory.Heading(headingLevel, null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(heading));
        Append(heading);
        return heading; 
    }

    public TestNode TestNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestNode(IdGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }
    
    public TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestInlineElementNode(IdGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(GetChildTreeBuilder(testNode));
        Append(testNode);
        return testNode;
    }
    
    public TestBlockElementNode TestBlockElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestBlockElementNode(IdGenerator.GetNextId(), null, null);
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
        
        NodeAdded?.Invoke(node);
    }

    private TreeBuilder GetChildTreeBuilder(DNode parentNode)
    {
        return new TreeBuilder(parentNode, IdGenerator, Root);
    }
    
    public interface ITreeMaker
    {
        RootNode Root { get; }

        void Print();
    }
    
    public interface IBlockTextTreeBuilder : ITreeMaker
    {
        TextNode Text(string value);

        TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null);
    }
}