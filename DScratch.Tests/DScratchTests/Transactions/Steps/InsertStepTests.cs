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
        CharNode node3 = null!;
        CharNode node4 = null!;
        var node2 = builder.Text(t => // ID "1"
        {
            node3 = t.Char('a'); // ID "2"
            node4 = t.Char('a'); // ID "3"
            t.Char('a');         // ID "4"
        });

        var node = new CharNode('a', new NodeId("Test", -1), node3, node4);     
        
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
        CharNode node4 = null!;
        var node2 = builder.Text(t => // ID "1"
        {
            t.Char('a'); // ID "2"
            node4 = t.Char('b'); // ID "3"
        });

        var node = new CharNode('c', new NodeId("Test", -1), node4, null);     
        
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
    public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AsFirstChild()
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
}