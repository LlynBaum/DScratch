using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Nodes;

public class TextNodeTests
{
    [Test]
    public void Length_ReturnsLengthCombines_FromNotDeletedChildNodes()
    {
        // Arrange
        var charNode1 = new CharNode('a', "2", null, null);
        var charNode2 = new CharNode('b', "3", null, null);
        var charNode3 = new CharNode('c', "4", null, null);
        charNode2.Delete();
        var testNode = new TextNode("1", null, null, [charNode1, charNode2, charNode3]);
        charNode1.Parent = testNode;
        charNode2.Parent = testNode;
        charNode3.Parent = testNode;
        
        // Act
        var result = testNode.Length;

        // Assert
        Assert.That(result, Is.EqualTo(2));
    }
    
    [Test]
    public void TextContent_ReturnsTextContentCombines_FromNotDeletedChildNodes()
    {
        // Arrange
        var charNode1 = new CharNode('a', "2", null, null);
        var charNode2 = new CharNode('b', "3", null, null);
        var charNode3 = new CharNode('c', "4", null, null);
        charNode2.Delete();
        var testNode = new TextNode("1", null, null, [charNode1, charNode2, charNode3]);
        charNode1.Parent = testNode;
        charNode2.Parent = testNode;
        charNode3.Parent = testNode;
        
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