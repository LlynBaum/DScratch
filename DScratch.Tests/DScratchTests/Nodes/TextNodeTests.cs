using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class TextNodeTests
{
    [Test]
    public void Length_ReturnsLengthCombines_FromNotDeletedChildNodes()
    {
        // Arrange
        var testNode = new TreeBuilder().Text("abc");
        
        // Act
        var result = testNode.Length;

        // Assert
        Assert.That(result, Is.EqualTo(2));
    }
    
    [Test]
    public void TextContent_ReturnsTextContentCombines_FromNotDeletedChildNodes()
    {
        // Arrange
        var testNode = new TreeBuilder().Text(t =>
        {
            t.Char('a');
            t.Char('b').Delete();
            t.Char('c');
        });
        
        // Act
        var result = testNode.TextContent;

        // Assert
        Assert.That(result, Is.EqualTo("ac"));
    }
    
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotCharNode()
    {
        // Arrange
        var testNode = new TextNode("1", null, null);
        var node = new TestNode("2", null, null)
        {
            Parent = testNode
        };

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => testNode.InsertChild(node);
    }
}