using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class DeleteRangeStepTests
{
    [Test]
    public void DeletesNode()
    {
        // Act
        var step = new DeleteRangeStep(null, null);
        var result = step.Execute();
            
        // Assert
        Assert.That(result, Has.Count.Zero);
    }
    
    [Test]
    public void DeletesNode_DeletesAllBetweenGivenNode_IncludingStartAndEnd()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node1 = null!;
        DNode node2 = null!;
        DNode node3 = null!;
        DNode node4 = null!;
        DNode node5 = null!;
        builder.TestNode(t =>
        {
            node1 = t.TestInlineElementNode();
            node2 = t.TestInlineElementNode();
            node3 = t.TestInlineElementNode();
            node4 = t.TestInlineElementNode();
            node5 = t.TestInlineElementNode();
        });
        
        // Act
        var step = new DeleteRangeStep(node2, node4);
        step.Execute();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node1.IsDeleted, Is.False);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(node4.IsDeleted, Is.True);
            Assert.That(node5.IsDeleted, Is.False);
        }
    }
    
    [Test]
    public void DeletesNode_DeletesAllUntilLastChild_WhenNoEndIsGiven()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node1 = null!;
        DNode node2 = null!;
        DNode node3 = null!;
        DNode node4 = null!;
        DNode node5 = null!;
        builder.TestNode(t => // ID "1"
        {
            node1 = t.TestInlineElementNode();
            node2 = t.TestInlineElementNode();
            node3 = t.TestInlineElementNode();
            node4 = t.TestInlineElementNode();
            node5 = t.TestInlineElementNode();
        });
        
        // Act
        var step = new DeleteRangeStep(node2, null);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node1.IsDeleted, Is.False);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(node4.IsDeleted, Is.True);
            Assert.That(node5.IsDeleted, Is.True);
        }
    }
    
    [Test]
    public void DeletesNode_DeletesAllUntilFirstChild_WhenNoStartIsGiven()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node1 = null!;
        DNode node2 = null!;
        DNode node3 = null!;
        DNode node4 = null!;
        DNode node5 = null!;
        builder.TestNode(t => // ID "1"
        {
            node1 = t.TestInlineElementNode();
            node2 = t.TestInlineElementNode();
            node3 = t.TestInlineElementNode();
            node4 = t.TestInlineElementNode();
            node5 = t.TestInlineElementNode();
        });
        
        // Act
        var step = new DeleteRangeStep(null, node3);
        step.Execute();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node1.IsDeleted, Is.True);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(node4.IsDeleted, Is.False);
            Assert.That(node5.IsDeleted, Is.False);
        }
    }
}