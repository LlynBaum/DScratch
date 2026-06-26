using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

public class DScratchDocumentTests
{
    private const string ClientName = "Test";
    private DScratchDocument Document { get; set; }

    [SetUp]
    public void SetUp()
    {
        var node1 = new TestNode(new NodeId(ClientName, 1), null, null);
        Document = new DScratchDocument(node1);
    }
    
    [Test]
    public void FindNode_FindExpectedTextNode_WithAddNode()
    {
        // Arrange
        var textNode1 = new TextNode(new NodeId(ClientName, 2), null, null);
        var textNode2 = new TextNode(new NodeId(ClientName, 3), null, null);
        
        textNode1.AddText("a");
        textNode2.AddText("a");
        
        // Act
        Document.AddNode(textNode1);
        Document.AddNode(textNode2);
        var result1 = Document.FindNode(new NodeId(ClientName, 2));
        var result2 = Document.FindNode(new NodeId(ClientName, 3));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.Not.Null);
            Assert.That(result2, Is.Not.Null);
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.EqualTo(textNode1));
            Assert.That(result2, Is.EqualTo(textNode2));
        }
    }
    
    [Test]
    public void FindNode_FindExpectedTextNode_WithContinuesRunNode()
    {
        // Arrange
        var textNode = new TextNode(new NodeId(ClientName, 2), null, null);
        textNode.AddText("abc");
        Document.AddNode(textNode);
        
        // Act
        var result1 = Document.FindNode(new NodeId(ClientName, 2));
        var result2 = Document.FindNode(new NodeId(ClientName, 3));
        var result3 = Document.FindNode(new NodeId(ClientName, 4));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.Not.Null);
            Assert.That(result2, Is.Not.Null);
            Assert.That(result3, Is.Not.Null);
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.EqualTo(textNode));
            Assert.That(result2, Is.EqualTo(textNode));
            Assert.That(result3, Is.EqualTo(textNode));
        }
    }
    
    [Test]
    public void FindNode_FindExpectedNode()
    {
        // Act
        var result = Document.FindNode(new NodeId(ClientName, 1));
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id.Clock, Is.EqualTo(1));
    }
    
    [Test]
    public void FindNode_ReturnsNullWhenNodeDoesNotExist()
    {
        // Act
        var result = Document.FindNode(new NodeId("abc", -2));
        
        // Assert
        Assert.That(result, Is.Null);
    }
}