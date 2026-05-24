using DScratch.Nodes;
using DScratch.Tests.Helpers;
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
        var builder = new TreeBuilder();
        DNode node1 = builder.TestNode(); // ID "0"
        DNode node3 = null!;
        DNode node4 = null!;
        DNode node2 = builder.TestNode(t => // ID "1"
        {
            node3 = t.TestNode(); // ID "2"
            node4 = t.TestNode(); // ID "3"
            t.TestNode();         // ID "4"
        });
        
        Document.Page.Root = node1;

        var node = new TestNode("-1", node3, node4);
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id, Is.EqualTo("1"));
            Assert.That(node2.ChildNodes[1].Id, Is.EqualTo("-1"));
            
            Assert.That(node3.RightOrigin!.Id, Is.EqualTo("-1"));
            Assert.That(node4.Origin?.Id, Is.EqualTo("-1"));
        }
    }

    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AsFirstChild()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node1 = builder.TestNode(); // ID "0"
        TestInlineElementNode node5 = null!;
        builder.TestNode(t => // ID "1"
        {
            t.TestNode(); // ID "2"
            t.TestNode(); // ID "3"
            node5 = t.TestInlineElementNode(); // ID "4"
        });
        
        Document.Page.Root = node1;
        
        var node = new TestInlineElementNode("-1", null, null);
        
        // Act
        var step = new InsertStep(node, node5);
        step.Execute(Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id, Is.EqualTo("4"));
            Assert.That(node5.ChildNodes[0].Id, Is.EqualTo("-1"));
        }
    }
}