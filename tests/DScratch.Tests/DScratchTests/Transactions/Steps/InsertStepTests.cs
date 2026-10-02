using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
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

        var node = new TextNode(new NodeId("Test", -1), node3.Id, node4.Id);     
        
        // Act
        var step = new InsertStep(node, node2);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes[1], Is.EqualTo(node));
            
            Assert.That(node3.NextSibling(), Is.EqualTo(node));
            Assert.That(node4.PreviousSibling(), Is.EqualTo(node));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [..AssertStepDiffHelper.InsertTextNodeTypes]);
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

        var node = new TextNode(new NodeId("Test", -1), node4.Id, null);        
        
        // Act
        var step = new InsertStep(node, node2);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.Clock, Is.EqualTo(1));
            Assert.That(node2.ChildNodes[2].Id.Clock, Is.EqualTo(-1));
            Assert.That(node4.NextSibling()!.Id.Clock, Is.EqualTo(-1));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [..AssertStepDiffHelper.InsertTextNodeTypes]);
    }

    [Test]
    public void GivenNode_IsInsertedAsChildOfNode_AsFirstChild()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        LinkNode node5 = null!;
        builder.Paragraph(t => // ID "1"
        {
            t.Text(" "); // ID "2"
            t.Text(" "); // ID "3"
            node5 = t.Link("", ""); // ID "4"
        });
        
        var node = new TextNode(new NodeId("Test", -1), null, null, " ");
        
        // Act
        var step = new InsertStep(node, node5);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Parent?.Id.Clock, Is.EqualTo(4));
            Assert.That(node5.ChildNodes[0].Id.Clock, Is.EqualTo(-1));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [..AssertStepDiffHelper.InsertTextNodeTypes]);
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

        var node = new TextNode(new NodeId(node4.LastId.Client, node4.LastId.Clock + 2), node4.LastId, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.NextSibling()!.Id.Clock, Is.EqualTo(4));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [..AssertStepDiffHelper.InsertTextNodeTypes]);
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

        var node = new TextNode(new NodeId("whatever", node4.LastId.Clock + 1), node4.LastId, null, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.NextSibling()!.Id.Clock, Is.EqualTo(3));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [..AssertStepDiffHelper.InsertTextNodeTypes]);
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

        var node = new TextNode(new NodeId(node4.LastId.Client, node4.LastId.Clock + 1), null, node4.Id, "c");
        
        // Act
        var step = new InsertStep(node, node2);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node2.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(node4.TextContent, Is.EqualTo("ab"));
            Assert.That(node4.PreviousSibling()!.Id.Clock, Is.EqualTo(3));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [..AssertStepDiffHelper.InsertTextNodeTypes]);
    }
    
    [Test]
    public void TextNode_SplitExistingNode_AndInsertBetween()
    {
        // Arrange
        var builder = new TreeBuilder();
        TextNode textNode = null!;
        var parent = builder.Paragraph(t =>
        {
            textNode = t.Text("ac");
        });

        var node = new TextNode(new NodeId(textNode.LastId.Client, textNode.LastId.Clock + 1), textNode.Id, textNode.LastId, "b");
        
        // Act
        var step = new InsertStep(node, parent);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.EqualTo(textNode));
            Assert.That(parent.ChildNodes[1], Is.EqualTo(node));
            
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("c"));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.InsertTextNodeTypes
        ]);
    }
    
    [Test]
    public void TextNode_SplitExistingNode_AndInsertBetween_WithThreeCharacters()
    {
        // Arrange
        var builder = new TreeBuilder();
        TextNode textNode = null!;
        var parent = builder.Paragraph(t =>
        {
            textNode = t.Text("abd");
        });

        var node = new TextNode(
            id: new NodeId(textNode.LastId.Client, textNode.LastId.Clock + 1),
            origin: new NodeId(textNode.Id.Client, textNode.Id.Clock + 1),
            rightOrigin: textNode.LastId,
            content: "c");
        
        // Act
        var step = new InsertStep(node, parent);
        var result = step.Execute(transactionFake, builder.Document);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.EqualTo(textNode));
            Assert.That(parent.ChildNodes[1], Is.EqualTo(node));
            
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("d"));
            
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        }
        
        AssertHelper.ThatStepsEqualTo(result, [
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.InsertTextNodeTypes
        ]);
    }
}