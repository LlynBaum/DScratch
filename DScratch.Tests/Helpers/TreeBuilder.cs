using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Helpers;

public class TreeBuilder : TreeBuilder.IParagraphTreeMaker
{
    public DNode Root { get; private set; } = new RootNode();

    private readonly DNode parent;
    private DNode? previousChild;

    private readonly TestNodeIdGenerator idGenerator;
    private readonly DNodeFactory factory;

    public TreeBuilder()
    {
        parent = Root;
        idGenerator = new TestNodeIdGenerator();
        factory =  new DNodeFactory(idGenerator);
    }
    
    private TreeBuilder(DNode parent, TestNodeIdGenerator idGenerator)
    {
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
        configureChildNodes?.Invoke(new TreeBuilder(paragraph, idGenerator));
        Append(paragraph);
        return paragraph;
    }

    public TestNode TestNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestNode(idGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(new TreeBuilder(testNode, idGenerator));
        Append(testNode);
        return testNode;
    }
    
    public TestInlineElementNode TestInlineElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestInlineElementNode(idGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(new TreeBuilder(testNode, idGenerator));
        Append(testNode);
        return testNode;
    }
    
    public TestBlockElementNode TestBlockElementNode(Action<TreeBuilder>? configureChildNodes = null)
    {
        var testNode = new TestBlockElementNode(idGenerator.GetNextId(), null, null);
        configureChildNodes?.Invoke(new TreeBuilder(testNode, idGenerator));
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

    private class RootNode() : DNode("root", null, null);
    
    public interface ITreeMaker
    {
        DNode Root { get; }
    }
    
    public interface IParagraphTreeMaker : ITreeMaker
    {
        TextNode Text(string value);
    }
}