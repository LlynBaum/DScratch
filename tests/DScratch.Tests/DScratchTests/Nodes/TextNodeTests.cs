using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.TreeVisualizers;

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
        Assert.That(result, Is.EqualTo(3));
    }
    
    [Test]
    public void LastId_ReturnsIdOfLastImplicitChild()
    {
        // Arrange
        var testNode = new TreeBuilder().Text("abc");

        // Act & Assert
        Assert.That(testNode.Id.Clock, Is.EqualTo(0));
        Assert.That(testNode.LastId.Clock, Is.EqualTo(2));
        Assert.That(testNode.LastId.Client, Is.EqualTo(testNode.Id.Client));
    }
    
    [Test]
    public void LastId_ReturnsId_WhenLengthIsZero()
    {
        // Arrange
        var testNode = new TreeBuilder().Text("a");

        // Act & Assert
        Assert.That(testNode.Id.Clock, Is.EqualTo(0));
        Assert.That(testNode.LastId.Clock, Is.EqualTo(0));
        Assert.That(testNode.LastId.Client, Is.EqualTo(testNode.Id.Client));
    }
    
    [Test]
    public void Split_CreatesExpectedNode()
    {
        // Arrange
        var testNode = new TreeBuilder()
            .Paragraph(t =>
            {
                t.Text("a");
                t.Text("abc");
                t.Text("a");
            });
        
        var textNode = (TextNode)testNode.ChildNodes[1];
        textNode.SetMark(new Mark(MarkKey.Bold));
        
        // Act
        var result = textNode.Split(1, () => new NodeId("Test", -1));

        // Assert
        Assert.That(testNode.ChildNodes, Has.Count.EqualTo(4));
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.Clock, Is.EqualTo(-1));

            Assert.That(textNode.Length, Is.EqualTo(1));
            Assert.That(result.Length, Is.EqualTo(2));

            Assert.That(textNode.TextContent, Is.EqualTo("a"));
            Assert.That(result.TextContent, Is.EqualTo("bc"));

            Assert.That(textNode.Parent, Is.EqualTo(testNode));
            Assert.That(result.Parent, Is.EqualTo(testNode));

            Assert.That(testNode.FirstChild!.Origin, Is.Null);
            Assert.That(testNode.FirstChild.RightOrigin, Is.EqualTo(textNode));
            
            Assert.That(textNode.Origin, Is.EqualTo(testNode.FirstChild));
            Assert.That(textNode.RightOrigin, Is.EqualTo(result));
            Assert.That(result.Origin, Is.EqualTo(textNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode.LastChild));
            
            Assert.That(testNode.LastChild!.Origin, Is.EqualTo(result));
            Assert.That(testNode.LastChild.RightOrigin, Is.Null);
            
            Assert.That(textNode.Marks, Is.EqualTo(result.Marks));
        }
    }
    
    [Test]
    public void Split_CreatesExpectedNode_WhenSplittingAtOffsetZero()
    {
        // Arrange
        var testNode = new TreeBuilder()
            .Paragraph(t =>
            {
                t.Text("a");
                t.Text("abc");
                t.Text("a");
            });
        
        var textNode = (TextNode)testNode.ChildNodes[1];
        
        // Act
        var result = textNode.Split(0, () => new NodeId("Test", -1));

        // Assert
        Assert.That(testNode.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result?.Id.Clock, Is.EqualTo(2));
            Assert.That(textNode.TextContent, Is.EqualTo("abc"));
        }
    }
    
    [Test]
    public void Split_CreatesExpectedNode_WhenSplittingAtOffsetEqualToLength()
    {
        // Arrange
        TextNode textNode = null!;
        var testNode = new TreeBuilder()
            .Paragraph(t =>
            {
                t.Text("a");
                textNode = t.Text("abc");
                t.Text("a");
            });
        
        // Act
        var result = textNode.Split(3, () => new NodeId("Test", -1));

        // Assert
        Assert.That(testNode.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Null);
            Assert.That(textNode.TextContent, Is.EqualTo("abc"));
        }
    }
    
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotCharNode()
    {
        // Arrange
        var testNode = new TextNode(new NodeId("Test", 1), null, null);
        var node = new TestNode(new NodeId("Test", 2), null, null)
        {
            Parent = testNode
        };

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => testNode.InsertChild(node);
    }

    [Test]
    public void SetMark_AddMarkToSet()
    {
        // Arrange
        var testNode = new TextNode(new NodeId("Test", 1), null, null);
        
        // Act
        testNode.SetMark(new Mark(MarkKey.Bold, "Ka"));
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.EqualTo(1));
        Assert.That(testNode.Marks.Single().Key, Is.EqualTo(MarkKey.Bold));
    }
    
    [Test]
    public void SetMark_OverridesExisting()
    {
        // Arrange
        var testNode = new TextNode(new NodeId("Test", 1), null, null);
        testNode.SetMark(new Mark(MarkKey.Color, "0"));
        
        // Act
        testNode.SetMark(new Mark(MarkKey.Color, "1"));
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.EqualTo(1));
        Assert.That(testNode.Marks.Single().Key, Is.EqualTo(MarkKey.Color));
        Assert.That(testNode.Marks.Single().Value, Is.EqualTo("1"));
    }
    
    [Test]
    public void RemoveMark_RemovesMarkFromSet()
    {
        // Arrange
        var testNode = new TextNode(new NodeId("Test", 1), null, null);
        testNode.SetMark(new Mark(MarkKey.Bold, "Ka"));
        
        // Act
        testNode.RemoveMark(MarkKey.Bold);
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.Zero);
    }
}