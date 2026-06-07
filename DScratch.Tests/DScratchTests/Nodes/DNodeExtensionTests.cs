using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class DNodeExtensionTests
{
    private readonly TreeBuilder treeBuilder = new TreeBuilder();
    
    [Test]
    public void GetTextLength_ReturnsExpectedLength()
    {
        // Arrange
        var testNode = treeBuilder.Paragraph(p =>
        {
            p.Text("abc");
            p.Text("abc").Delete();
            p.Text("abc");
        });
        
        // Act
        var result = testNode.GetTextLength();

        // Assert
        Assert.That(result, Is.EqualTo(6));
    }
    
    [Test]
    public void TextNode_GivenTree_ReturnsExpectedOffset()
    {
        // Arrange
        TextNode textNode = null!;
        var parent = treeBuilder.TestInlineElementNode(t =>
        {
            t.Text("abc");
            textNode = t.Text("x");
            t.Text("def");
        });
        
        // Act
        var result = parent.FindAbsolutTextOffset(textNode);
        
        // Assert
        Assert.That(result, Is.EqualTo(3));
    }
    
    [Test]
    public void TextNode_GivenTree_ReturnsExpectedOffset_Deleted()
    {
        // Arrange
        TextNode textNode = null!;
        var parent = treeBuilder.TestInlineElementNode(t =>
        {
            t.Text("abc");
            t.Text("abc").Delete();
            textNode = t.Text("x");
            textNode.Delete();
            t.Text("def");
        });
        
        // Act
        var result = parent.FindAbsolutTextOffset(textNode);
        
        // Assert
        Assert.That(result, Is.EqualTo(3));
    }
    
    [Test]
    public void TNode_GivenTree_ReturnsExpectedOffset()
    {
        // Arrange
        TestInlineElementNode inlineElement = null!;
        var parent = treeBuilder.TestInlineElementNode(t =>
        {
            t.Text("abc");
            inlineElement = t.TestInlineElementNode();
            t.Text("def");
        });
        
        // Act
        var result = parent.FindAbsolutTextOffset<IInlineElement>(inlineElement);
        
        // Assert
        Assert.That(result, Is.EqualTo(3));
    }
    
    [Test]
    public void TNode_GivenTree_ReturnsExpectedOffset_Deleted()
    {
        // Arrange
        TestInlineElementNode inlineElement = null!;
        var parent = treeBuilder.TestInlineElementNode(t =>
        {
            t.Text("abc");
            t.Text("abc").Delete();
            inlineElement = t.TestInlineElementNode();
            inlineElement.Delete();
            t.Text("def");
        });
        
        // Act
        var result = parent.FindAbsolutTextOffset<IInlineElement>(inlineElement);
        
        // Assert
        Assert.That(result, Is.EqualTo(3));
    }
}