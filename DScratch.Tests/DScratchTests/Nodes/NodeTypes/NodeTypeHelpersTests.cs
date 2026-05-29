using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes.NodeTypes;

public class NodeTypeHelpersTests
{
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