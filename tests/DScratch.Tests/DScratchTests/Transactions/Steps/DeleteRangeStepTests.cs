using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class DeleteRangeStepTests
{
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        transactionFake = new TestTransactionFake();
    }
    
    [Test]
    public void DeletesNode()
    {
        // Act
        var step = new DeleteRangeStep(null, null);
        var result = step.Execute(transactionFake, null!);
            
        // Assert
        Assert.That(result, Has.Count.Zero);
        Assert.That(transactionFake.ChangedNodes, Has.Count.Zero);
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
        builder.Paragraph(t =>
        {
            node1 = t.TestInlineElementNode();
            node2 = t.TestInlineElementNode();
            node3 = t.TestInlineElementNode();
            node4 = t.TestInlineElementNode();
            node5 = t.TestInlineElementNode();
        });
        
        // Act
        var step = new DeleteRangeStep(node2.Id, node4.Id);
        step.Execute(transactionFake, builder.Document);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node1.IsDeleted, Is.False);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(node4.IsDeleted, Is.True);
            Assert.That(node5.IsDeleted, Is.False);
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node2, node3, node4]));
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
        builder.Paragraph(t =>
        {
            node1 = t.TestInlineElementNode();
            node2 = t.TestInlineElementNode();
            node3 = t.TestInlineElementNode();
            node4 = t.TestInlineElementNode();
            node5 = t.TestInlineElementNode();
        });
        
        // Act
        var step = new DeleteRangeStep(node2.Id, null);
        step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node1.IsDeleted, Is.False);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(node4.IsDeleted, Is.True);
            Assert.That(node5.IsDeleted, Is.True);
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node2, node3, node4, node5]));
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
        builder.Paragraph(t =>
        {
            node1 = t.TestInlineElementNode();
            node2 = t.TestInlineElementNode();
            node3 = t.TestInlineElementNode();
            node4 = t.TestInlineElementNode();
            node5 = t.TestInlineElementNode();
        });
        
        // Act
        var step = new DeleteRangeStep(null, node3.Id);
        step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node1.IsDeleted, Is.True);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(node4.IsDeleted, Is.False);
            Assert.That(node5.IsDeleted, Is.False);
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node1, node2, node3]));
        }
    }
    
    [Test]
    public void DeletesNode_DeletesAllBetweenGivenNode_IncludingStartAndEnd_AndSplitsTextNodes()
    {
        // Arrange
        var builder = new TreeBuilder();
        TextNode startNode = null!;
        TextNode midNode = null!;
        TextNode endNode = null!;
        var parent = builder.Paragraph(t =>
        {
            startNode = t.Text("abc");
            midNode = t.Text("def");
            endNode = t.Text("ghi");
        });
        
        // Act
        var step = new DeleteRangeStep(new NodeId(startNode.Id.Client, startNode.Id.Clock + 1), new NodeId(endNode.Id.Client, endNode.Id.Clock + 1));
        step.Execute(transactionFake, builder.Document);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.IsDeleted, Is.False);
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.EqualTo(startNode));
            Assert.That(parent.ChildNodes[2], Is.EqualTo(midNode));
            Assert.That(parent.ChildNodes[3], Is.EqualTo(endNode));
        }
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("bc"));
            
            Assert.That(parent.ChildNodes[2].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("def"));
            
            Assert.That(parent.ChildNodes[3].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("gh"));
            
            Assert.That(parent.ChildNodes[4].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("i"));
        }
        
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo(parent.ChildNodes));
    }
}