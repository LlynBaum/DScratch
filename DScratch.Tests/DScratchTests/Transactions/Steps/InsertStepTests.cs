using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class InsertStepTests
{
    private DScratchDocument Document { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
    }
    
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath()
    {
        // Arrange
        var node3 = new CharNode('a', "3", null, null);
        var node4 = new CharNode('a', "4", node3, null);
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

        var node = new CharNode('a', "-1", node3, node4);
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id, Is.EqualTo("2"));
            Assert.That(node2.ChildNodes[1].Id, Is.EqualTo("-1"));
            
            Assert.That(node3.RightOrigin.Id, Is.EqualTo("-1"));
            Assert.That(node4.Origin?.Id, Is.EqualTo("-1"));
        }
    }

    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AsFirstChild()
    {
        // Arrange
        var node3 = new CharNode('a', "3", null, null);
        var node4 = new CharNode('a', "4", node3, null);
        node3.RightOrigin = node4;
        var node5 = new TestInlineElementNode("5", node4, null);
        node4.RightOrigin = node5;
        
        var node1 = new TestNode("1", null, null);
        var node2 = new TestNode("2", node1, null, [node3, node4, node5]);
        node1.RightOrigin = node2;

        node3.Parent = node2;
        node4.Parent = node2;
        node5.Parent = node2;
        Document.Page.Root = node1;
        
        var node = new TestInlineElementNode("-1", null, null);
        
        // Act
        var step = new InsertStep(node, node5);
        step.Execute(Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id, Is.EqualTo("5"));
            Assert.That(node5.ChildNodes[0].Id, Is.EqualTo("-1"));
        }
    }
}