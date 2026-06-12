using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions;

public class DTransactionTests
{
    private DScratchDocument Document { get; set; }
    private DTransaction Transaction { get; set; }
    private TreeBuilder TreeBuilder { get; set; }

    [SetUp]
    public void SetUp()
    {
        TreeBuilder = new TreeBuilder();
        Document = TreeBuilder.CreateDocument();
        Transaction = new DTransaction(Document, new TestNodeIdGenerator(), true);
    }

    [Test]
    public void Commit()
    {
        var testStep = new TestStep();
        ((List<IStep>)Transaction.Steps).Add(testStep);
        
        // Act
        var result = Transaction.Commit();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(testStep.Executed, Is.True);
            Assert.That(testStep.Reverted, Is.False);

            Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        }
        
        Assert.That(result.Steps, Has.Count.EqualTo(1));
        Assert.That(result.Steps.Single(), Is.TypeOf<TestStepDiff>());
    }
    
    [Test]
    public void Insert_AddsInsertStep()
    {
        // Act
        var nodeId = new NodeId("Test", 1);
        var node = new TestInlineElementNode(nodeId, null, null);
        
        // Act
        Transaction.Insert(node, TreeBuilder.Root);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        var step = Transaction.Steps.Single();
        Assert.That(step, Is.TypeOf<InsertStep>());
        
        Transaction.Commit();
        Assert.That(Document.FindNode(nodeId), Is.EqualTo(node));
    }
    
    [Test]
    public void DeleteNode_AddsDeleteStep()
    {
        // Act
        var node = TreeBuilder.TestInlineElementNode();
        Transaction.Delete(node);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<DeleteStep>());
        
        Transaction.Commit();
        Assert.That(Document.FindNode(node.Id), Is.EqualTo(node));
    }
    
    [Test]
    public void DeleteRange_AddsDeleteRangeStep()
    {
        // Act
        var node = TreeBuilder.TestInlineElementNode();
        var node2 = TreeBuilder.TestInlineElementNode();
        
        
        Transaction.DeleteRange(node, node2);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<DeleteRangeStep>());
        
        Transaction.Commit();
        Assert.That(Document.FindNode(node.Id), Is.EqualTo(node));
        Assert.That(Document.FindNode(node2.Id), Is.EqualTo(node2));
    }
    
    [Test]
    public void MoveRange_AddsMoveRangeStep()
    {
        // Act
        Transaction.MoveRange(TestNode.Empty(), TestNode.Empty(), TestNode.Empty(), null);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<MoveRangeStep>());
    }
    
    [Test]
    public void UpdateNodeType_AddsUpdateNodeTypeStep()
    {
        // Act
        Transaction.UpdateNodeType(TestNode.Empty(), node => node);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<UpdateNodeTypeStep>());
    }
    
    private class TestStep : IStep
    {
        public bool Executed;
        public bool Reverted;
        
        public IReadOnlyList<StepDiff> Execute(IRunningTransaction transaction, DScratchDocument document)
        {
            Executed = true;
            return [new TestStepDiff()];
        }

        public IReadOnlyList<StepDiff> Revert(DScratchDocument document)
        {
            Reverted = true;
            return [new TestStepDiff()];
        }
    }
}