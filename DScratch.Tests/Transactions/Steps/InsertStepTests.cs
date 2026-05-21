using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.Transactions.Steps;

public class InsertStepTests
{
    private DScratchDocument Document { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
        DefaultNodes();
    }
    
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath()
    {
        // Arrange
        DefaultNodes();
        var node = new CharNode('a', "-1", null, null);
        var step = new InsertStep(node, new NodePath(["2"]), 1);
        
        // Act
        step.Execute(Document);
        
        // Assert
        var insertedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.FirstChild;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(insertedNode?.Id, Is.EqualTo("-1"));
            
            Assert.That(node.Parent?.Id, Is.EqualTo("2"));
            Assert.That(node.Origin, Is.Null);
            Assert.That(node.RightOrigin, Is.Null);
        }
    }

    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AsFirstChild()
    {
        // Arrange
        DefaultNodes();
        var node = new TestNode("-1", null, null);
        var step = new InsertStep(node, new NodePath(["2", "4"]), 0);
        
        // Act
        step.Execute(Document);
        
        // Assert
        var insertedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.FirstChild;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(insertedNode?.Id, Is.EqualTo("-1"));
            
            Assert.That(node.Parent?.Id, Is.EqualTo("4"));
            Assert.That(node.Origin, Is.Null);
            Assert.That(node.RightOrigin?.Id, Is.EqualTo("6"));
        }
    }
    
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AfterExistingChild()
    {
        // Arrange
        DefaultNodes();
        var node = new TestNode("-1", null, null);
        var step = new InsertStep(node, new NodePath(["2", "4"]), 1);
        
        // Act
        step.Execute(Document);
        
        // Assert
        var insertedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.FirstChild?.RightOrigin;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(insertedNode?.Id, Is.EqualTo("-1"));
            
            Assert.That(node.Parent?.Id, Is.EqualTo("4"));
            Assert.That(node.Origin?.Id, Is.EqualTo("6"));
            Assert.That(node.RightOrigin, Is.Null);
        }
    }
    
    private void DefaultNodes()
    {
        var node3 = new TestNode("3", null, null);
        var node4 = new TestNode("4", node3, null);
        node3.RightOrigin = node4;
        var node5 = new TestNode("5", node4, null);
        node4.RightOrigin = node5;
        
        var node1 = new TestNode("1", null, null);
        var node2 = new TestNode("2", node1, null, [node3, node4, node5]);
        node1.RightOrigin = node2;

        node3.Parent = node2;
        node4.Parent = node2;
        node5.Parent = node2;
        
        Document.Page.Root = node1;
    }
}