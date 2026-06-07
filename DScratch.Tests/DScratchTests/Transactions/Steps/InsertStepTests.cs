using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class InsertStepTests
{
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TestNode node3 = null!;
        TestNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            node3 = t.TestNode(); // ID "2"
            node4 = t.TestNode(); // ID "3"
            t.TestNode();         // ID "4"
        });

        var node = new TestNode(new NodeId("Test", -1), node3, node4);     
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.IdValue, Is.EqualTo(1));
            Assert.That(node2.ChildNodes[1].Id.IdValue, Is.EqualTo(-1));
            
            Assert.That(node3.RightOrigin!.Id.IdValue, Is.EqualTo(-1));
            Assert.That(node4.Origin?.Id.IdValue, Is.EqualTo(-1));
        }
    }
    
    [Test]
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AtPositionThree()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TestNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            t.TestNode(); // ID "2"
            node4 = t.TestNode(); // ID "3"
        });

        var node = new TestNode(new NodeId("Test", -1), node4, null);        
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.IdValue, Is.EqualTo(1));
            Assert.That(node2.ChildNodes[2].Id.IdValue, Is.EqualTo(-1));
            Assert.That(node4.RightOrigin!.Id.IdValue, Is.EqualTo(-1));
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
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.IdValue, Is.EqualTo(4));
            Assert.That(node5.ChildNodes[0].Id.IdValue, Is.EqualTo(-1));
        }
    }
    
    [Test]
    public void ContinuesTextNode_IsCombinedWithPreviousNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            node4 = t.Text("ab"); // ID "2"
        });

        var node = new TextNode(new NodeId(node4.Id.Client, node4.Id.IdValue + 1), node4, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(node4.TextContent, Is.EqualTo("abc"));
        }
    }
    
    [Test]
    public void TextNode_IsNotCombinedWithPreviousNode_WhenIdsAreNotContinues()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            node4 = t.Text("ab"); // ID "2"
        });

        var node = new TextNode(new NodeId(node4.Id.Client, node4.Id.IdValue + 2), node4, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.RightOrigin!.Id.IdValue, Is.EqualTo(4));
        }
    }
    
    [Test]
    public void TextNode_IsNotCombinedWithPreviousNode_WhenClientIsDifferent()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            node4 = t.Text("ab"); // ID "2"
        });

        var node = new TextNode(new NodeId("whatever", node4.Id.IdValue + 1), node4, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("abc"));
            Assert.That(node4.RightOrigin!.Id.IdValue, Is.EqualTo(3));
        }
    }
    
    [Test]
    public void TextNode_IsNotCombinedWithPreviousNode_WhenIsInsertedInFront()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        TextNode node4 = null!;
        var node2 = builder.TestInlineElementNode(t => // ID "1"
        {
            node4 = t.Text("ab"); // ID "2"
        });

        var node = new TextNode(new NodeId(node4.Id.Client, node4.Id.IdValue + 1), null, node4, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("abc"));
            Assert.That(node4.Origin!.Id.IdValue, Is.EqualTo(3));
        }
    }
}