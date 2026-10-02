using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class DeleteStepTests
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
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode();
        DNode node3 = null!;
        builder.TestNode(t =>
        {
            node3 = t.TestInlineElementNode();
            t.TestNode();
            t.TestNode();
        });
        
        // Act
        var step = new DeleteStep(node3.Id);
        var result = step.Execute(transactionFake, null!);
            
        // Assert
        Assert.That(node3.IsDeleted, Is.True);
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node3]));
        
        AssertHelper.ThatStepsEqualTo(result, Is.TypeOf<StepDiff.DeleteElementDiff>());
    }
    
    [Test]
    public void DeletesNode_DeleteCharacterInMiddleOfText()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode();
        
        TextNode textNode = null!;
        var parent = builder.TestInlineElementNode(t =>
        {
            textNode = t.Text("abc");
            t.Text("d");
        }); 
        
        // Act
        var steps = new DeleteStep(new NodeId(textNode.Id.Client, textNode.Id.Clock + 1)); // "b"
        var result = steps.Execute(transactionFake, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("c"));
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("d"));
            
            Assert.That(parent.ChildNodes[0], Is.EqualTo(textNode));
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
        }
        
        AssertHelper.ThatStepsEqualTo(result, expected: [
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.SplitTextTypes,
            Is.TypeOf<StepDiff.DeleteElementDiff>()
        ]);
    }
    
    [Test]
    public void DeletesNode_DeleteCharacterAtStartOfText()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode();
        
        TextNode textNode = null!;
        var parent = builder.TestInlineElementNode(t =>
        {
            textNode = t.Text("abc");
            t.Text("d");
        }); 
        
        // Act
        var steps = new DeleteStep(textNode.Id); // "a"
        var result = steps.Execute(transactionFake, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("bc"));
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("d"));
            
            Assert.That(parent.ChildNodes[0], Is.EqualTo(textNode));
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.True);
        }
        
        AssertHelper.ThatStepsEqualTo(result, expected: [
            ..AssertStepDiffHelper.SplitTextTypes,
            Is.TypeOf<StepDiff.DeleteElementDiff>()
        ]);
    }
    
    [Test]
    public void DeletesNode_DeleteCharacterAtEndOfText()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode();
        
        TextNode textNode = null!;
        var parent = builder.TestInlineElementNode(t =>
        {
            textNode = t.Text("abc");
            t.Text("d");
        }); 
        
        // Act
        var steps = new DeleteStep(textNode.LastId); // "c"
        var result = steps.Execute(transactionFake, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("d"));
            
            Assert.That(parent.ChildNodes[0], Is.EqualTo(textNode));
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
        }
        
        AssertHelper.ThatStepsEqualTo(result, expected: [
            ..AssertStepDiffHelper.SplitTextTypes,
            Is.TypeOf<StepDiff.DeleteElementDiff>()
        ]);
    }
}