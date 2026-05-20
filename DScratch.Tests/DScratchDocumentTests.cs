using DScratch.Tests.Helpers;

namespace DScratch.Tests;

public class DScratchDocumentTests
{
    private DScratchDocument Document { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
        DefaultNodes();
    }
    
    [Test]
    public void FindNode_FindFirstNodePath()
    {
        // Act
        var result = Document.FindNode(new NodePath(["1"]));
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("1"));
    }
    
    [Test]
    public void FindNode_FindSecondNode()
    {
        // Act
        var result = Document.FindNode(new NodePath(["2"]));
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("2"));
    }
    
    [Test]
    public void FindNode_FindFirstChild()
    {
        // Act
        var result = Document.FindNode(new NodePath(["2", "3"]));
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("3"));
    }
    
    [Test]
    public void FindNode_FindSecondChild()
    {
        // Act
        var result = Document.FindNode(new NodePath(["2", "4"]));
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("4"));
    }
    
    [Test]
    public void FindNode_FindLastChild()
    {
        // Act
        var result = Document.FindNode(new NodePath(["2", "5"]));
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("5"));
    }
    
    [Test]
    public void FindNode_ReturnsNullWhenNodeDoesNotExist()
    {
        // Act
        var result = Document.FindNode(new NodePath(["abc"]));
        
        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void FindNode_ReturnsNullWhenChildNodeDoesNotExist()
    {
        // Act
        var result = Document.FindNode(new NodePath(["1", "abc"]));
        
        // Assert
        Assert.That(result, Is.Null);
    }
    
    private void DefaultNodes()
    {
        var node3 = new TestNode("3", null, null, null);
        var node4 = new TestNode("4", node3, null, null);
        var node5 = new TestNode("5", node4, null, null);
        node4.RightOrigin = node5;
        
        var node1 = new TestNode("1", null, null, null);
        var node2 = new TestNode("2", node1, null, null, [node3, node4, node5]);
        node1.RightOrigin = node2;

        node3.Parent = node2;
        node4.Parent = node2;
        node5.Parent = node2;
        
        Document.Page.Root = node1;
    }
}