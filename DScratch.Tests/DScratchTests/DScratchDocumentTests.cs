using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

public class DScratchDocumentTests
{
    private const string ClientName = "Test";
    private DScratchDocument Document { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument(new NodeId(ClientName, -1));
        DefaultNodes();
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
    
    private void DefaultNodes()
    {
        var node1 = new TestNode(new NodeId(ClientName, 1), null, null);
        Document = new DScratchDocument(node1);
    }
}