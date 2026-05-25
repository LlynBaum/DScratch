using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes.NodeTypes;

public class NodeTypeHelpersTests
{
    [Test]
    public void IsElement_ReturnsTrue_WhenInherits_Element()
    {
        // Arrange
        var inlineElement = TestInlineElementNode.Empty();
        var blockElement = TestBlockElementNode.Empty();
        
        // Act & Assert
        Assert.That(inlineElement.IsElement(), Is.True);
        Assert.That(blockElement.IsElement(), Is.True);
    }
    
    [Test]
    public void IsElement_ReturnsFalse_WhenNotInherits_Element()
    {
        // Arrange
        var testNode = TestNode.Empty();
        
        // Act & Assert
        Assert.That(testNode.IsElement(), Is.False);
    }
    
    [Test]
    public void IsInlineNode_ReturnsTrue_WhenInheritsInline_Element()
    {
        // Arrange
        var inlineElement = TestInlineElementNode.Empty();
        
        // Act & Assert
        Assert.That(inlineElement.IsInlineNode(), Is.True);
    }
    
    [Test]
    public void IsInlineNode_ReturnsFalse_WhenNotInherits_InlineElement()
    {
        // Arrange
        var testNode = TestNode.Empty();
        
        // Act & Assert
        Assert.That(testNode.IsInlineNode(), Is.False);
    }
    
    [Test]
    public void IsBlockNode_ReturnsTrue_WhenInherits_BlockElement()
    {
        // Arrange
        var blockElement = TestBlockElementNode.Empty();
        
        // Act & Assert
        Assert.That(blockElement.IsBlockNode(), Is.True);
    }
    
    [Test]
    public void IsBlockNode_ReturnsFalse_WhenNotInherits_BlockElement()
    {
        // Arrange
        var testNode = TestNode.Empty();
        
        // Act & Assert
        Assert.That(testNode.IsBlockNode(), Is.False);
    }
    
    [Test]
    public void IsTextNode_ReturnsTrue_WhenIs_TextNode()
    {
        // Arrange
        var textNode = new TextNode("", null, null);
        
        // Act & Assert
        Assert.That(textNode.IsTextNode(), Is.True);
    }
    
    [Test]
    public void IsTextNode_ReturnsTrue_WhenIsNot_TextNode()
    {
        // Arrange
        var testNode = TestNode.Empty();
        
        // Act & Assert
        Assert.That(testNode.IsTextNode(), Is.False);
    }
    
    [Test]
    public void IsParagraphNode_ReturnsTrue_WhenIs_ParagraphNode()
    {
        // Arrange
        var textNode = new ParagraphNode("", null, null);
        
        // Act & Assert
        Assert.That(textNode.IsParagraphNode(), Is.True);
    }
    
    [Test]
    public void IsParagraphNode_ReturnsTrue_WhenIsNot_ParagraphNode()
    {
        // Arrange
        var testNode = TestNode.Empty();
        
        // Act & Assert
        Assert.That(testNode.IsParagraphNode(), Is.False);
    }
    
    [Test]
    public void IsTextOrInline_ReturnsTrue_WhenInherits_InlineElementOrTextNode()
    {
        // Arrange
        var inlineElement = TestInlineElementNode.Empty();
        var textNode = new TextNode("", null, null);
        
        // Act & Assert
        Assert.That(inlineElement.IsTextOrInline(), Is.True);
        Assert.That(textNode.IsTextOrInline(), Is.True);
    }
    
    [Test]
    public void IsTextOrInline_ReturnsFalse_WhenNotInherits_InlineElementOrTextNode()
    {
        // Arrange
        var testNode = TestNode.Empty();
        
        // Act & Assert
        Assert.That(testNode.IsTextOrInline(), Is.False);
    }
}