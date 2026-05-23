using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests;

public class TreeWalkerTests
{
    [Test]
    public void MoveNext_FiltersExpectedNodes()
    {
        // Arrange
        var testNode1 = new TextNode("3", null, null);
        var testNode2 = new TestInlineElementNode("5", null, null);
        var testNode3 = new TextNode("5", null, null);
        
        var paragraph = new ParagraphNode("2", null, null, [testNode1, testNode2, testNode3]);
        testNode1.Parent = paragraph;
        testNode2.Parent = paragraph;
        testNode3.Parent = paragraph;
        
        // Act & Assert
        using var walker = new TreeWalker<TextNode>(paragraph);
        
        walker.MoveNext();
        var node1 = walker.Current;
        Assert.That(node1, Is.Not.Null);
        Assert.That(node1, Is.EqualTo(testNode1));
        
        walker.MoveNext();
        var node2 = walker.Current;
        Assert.That(node2, Is.Not.Null);
        Assert.That(node2, Is.EqualTo(testNode3));

        walker.MoveNext();
        var node3 = walker.Current;
        Assert.That(node3, Is.Null);
    }
}