using DScratch.Marks;
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
        textNode.SetMark(MarkKey.FontWeight, "bold");
        
        // Act
        var result = textNode.Split(1, _ => new NodeId("Test", -1));

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
        var result = textNode.Split(0, _ => new NodeId("Test", -1));

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
        var result = textNode.Split(3, _ => new NodeId("Test", -1));

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
}