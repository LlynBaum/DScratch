using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests;

public class TreeWalkerTests
{
    [Test]
    public void MoveNext_FiltersExpectedNodes()
    {
        // Arrange
        var charNode = new CharNode('a', "6", null, null);
        var child = new TextNode("5", null, null, [charNode]);
        charNode.Parent = child;
        var testNode1 = new TextNode("2", null, null, [child]);
        child.Parent = testNode1;
        var testNode2 = new TestInlineElementNode("3", testNode1, null);
        testNode1.RightOrigin = testNode2;
        var testNode3 = new TextNode("4", testNode2, null);
        testNode2.RightOrigin = testNode3;
        
        var paragraph = new ParagraphNode("1", null, null, [testNode1, testNode2, testNode3]);
        testNode1.Parent = paragraph;
        testNode2.Parent = paragraph;
        testNode3.Parent = paragraph;
        
        // Act & Assert
        var walker = new TreeWalker<TextNode>(paragraph);
        
        Assert.That(walker.NextNode(), Is.EqualTo(testNode1));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(testNode1));
        
        Assert.That(walker.NextNode(), Is.EqualTo(child));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(child));
        
        Assert.That(walker.NextNode(), Is.EqualTo(testNode3));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(testNode3));

        Assert.That(walker.NextNode(), Is.Null);
        Assert.That(walker.Current, Is.Null);
    }
}