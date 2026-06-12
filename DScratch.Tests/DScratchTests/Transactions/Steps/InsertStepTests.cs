using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class InsertStepTests
{
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        transactionFake = new TestTransactionFake();
    }
    
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TextNode node3 = null!;
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            node3 = t.Text("a"); // ID "2"
            node4 = t.Text("a"); // ID "3"
            t.Text("a");         // ID "4"
        });

        var node = new TextNode(new NodeId("Test", -1), node3, node4);     
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(transactionFake);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.Clock, Is.EqualTo(1));
            Assert.That(node2.ChildNodes[1].Id.Clock, Is.EqualTo(-1));
            
            Assert.That(node3.RightOrigin!.Id.Clock, Is.EqualTo(-1));
            Assert.That(node4.Origin?.Id.Clock, Is.EqualTo(-1));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
    }
    
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AtPositionThree()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            t.Text("a"); // ID "2"
            node4 = t.Text("a"); // ID "3"
        });

        var node = new TextNode(new NodeId("Test", -1), node4, null);        
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(transactionFake);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.Clock, Is.EqualTo(1));
            Assert.That(node2.ChildNodes[2].Id.Clock, Is.EqualTo(-1));
            Assert.That(node4.RightOrigin!.Id.Clock, Is.EqualTo(-1));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
    }

    [Test]
    public void GivenNode_IsInsertedAsChildOfNode_AsFirstChild()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TestInlineElementNode node5 = null!;
        builder.TestNode(t => // ID "1"
        {
            t.TestNode(); // ID "2"
            t.TestNode(); // ID "3"
            node5 = t.TestInlineElementNode(); // ID "4"
        });
        
        var node = new TestInlineElementNode(new NodeId("Test", -1), null, null);
        
        // Act
        var step = new InsertStep(node, node5);
        step.Execute(transactionFake);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.Clock, Is.EqualTo(4));
            Assert.That(node5.ChildNodes[0].Id.Clock, Is.EqualTo(-1));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
    }
    
    [Test]
    public void TextNode_IsNotCombinedWithPreviousNode_WhenIdsAreNotContinues()
    {
        // Arrange
        var builder = new TreeBuilder();
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "0"
        {
            node4 = t.Text("ab"); // ID "1"
        });

        var node = new TextNode(new NodeId(node4.LastId.Client, node4.LastId.Clock + 2), node4, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(transactionFake);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.RightOrigin!.Id.Clock, Is.EqualTo(4));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
    }
    
    [Test]
    public void TextNode_IsNotCombinedWithPreviousNode_WhenClientIsDifferent()
    {
        // Arrange
        var builder = new TreeBuilder();
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "0"
        {
            node4 = t.Text("ab"); // ID "1"
        });

        var node = new TextNode(new NodeId("whatever", node4.LastId.Clock + 1), node4, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(transactionFake);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.RightOrigin!.Id.Clock, Is.EqualTo(3));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
    }
    
    [Test]
    public void TextNode_IsNotCombinedWithPreviousNode_WhenIsInsertedInFront()
    {
        // Arrange
        var builder = new TreeBuilder();
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "0"
        {
            node4 = t.Text("ab"); // ID "1"
        });

        var node = new TextNode(new NodeId(node4.LastId.Client, node4.LastId.Clock + 1), null, node4, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute(transactionFake);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.Origin!.Id.Clock, Is.EqualTo(3));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
    }
}