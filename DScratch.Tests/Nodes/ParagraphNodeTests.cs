using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Nodes;

public class ParagraphNodeTests
{
    [Test]
    public void TagName_ReturnsExpectedDomElementName()
    {
        var paragraph = new ParagraphNode("1", null, null);
        Assert.That(paragraph.TagName, Is.EqualTo("p"));
    }

    [Test]
    public void Length_ReturnsLengthCombines_FromAllChildNodes()
    {
        // Arrange
        var charNode1 = new CharNode('a', "4", null, null);
        var testNode1 = new TextNode("3", null, null, [charNode1]);
        charNode1.Parent = testNode1;
        
        var charNode2 = new CharNode('b', "6", null, null);
        var testNode2 = new TextNode("5", null, null, [charNode2]);
        charNode2.Parent = testNode2;
        testNode2.Delete();
        
        var charNode3 = new CharNode('c', "8", null, null);
        var testNode3 = new TextNode("7", null, null, [charNode3]);
        charNode3.Parent = testNode3;
        
        var paragraph = new ParagraphNode("2", null, null, [testNode1, testNode2, testNode3]);
        testNode1.Parent = paragraph;
        testNode2.Parent = paragraph;
        testNode3.Parent = paragraph;
        
        // Act
        var result = paragraph.Length;

        // Assert
        Assert.That(result, Is.EqualTo(2));
    }
    
    [Test]
    public void TextContent_ReturnsTextContentCombines_FromAllChildNodes()
    {
        // Arrange
        var charNode1 = new CharNode('a', "4", null, null);
        var testNode1 = new TextNode("3", null, null, [charNode1]);
        charNode1.Parent = testNode1;
        
        var charNode2 = new CharNode('b', "6", null, null);
        var testNode2 = new TextNode("5", null, null, [charNode2]);
        charNode2.Parent = testNode2;
        testNode2.Delete();
        
        var charNode3 = new CharNode('c', "8", null, null);
        var testNode3 = new TextNode("7", null, null, [charNode3]);
        charNode3.Parent = testNode3;
        
        var paragraph = new ParagraphNode("2", null, null, [testNode1, testNode2, testNode3]);
        testNode1.Parent = paragraph;
        testNode2.Parent = paragraph;
        testNode3.Parent = paragraph;
        
        // Act
        var result = paragraph.TextContent;

        // Assert
        Assert.That(result, Is.EqualTo("ac"));
    }
    
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotText()
    {
        // Arrange
        var paragraph = new ParagraphNode("1", null, null);
        var node = new TestNode("2", null, null)
        {
            Parent = paragraph
        };

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => paragraph.InsertChild(node);
    }
}