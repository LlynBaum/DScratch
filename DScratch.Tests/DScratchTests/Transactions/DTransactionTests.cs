using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions;

public class DTransactionTests
{
    private DScratchDocument Document { get; set; }
    private DTransaction Transaction { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
        Transaction = new DTransaction(Document, new TestNodeIdGenerator());
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
        
        Assert.That(result.Diffs, Has.Count.EqualTo(1));
        Assert.That(result.Diffs.Single(), Is.TypeOf<TestStepDiff>());
    }
    
    [Test]
    public void Insert_AddsInsertStep()
    {
        // Act
        Transaction.Insert(TestNode.Empty(), TestNode.Empty());
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        var step = Transaction.Steps.Single();
        Assert.That(step, Is.TypeOf<InsertStep>());
    }
    
    [Test]
    public void DeleteNode_AddsDeleteStep()
    {
        // Act
        Transaction.Delete(TestNode.Empty());
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<DeleteStep>());
    }
    
    [Test]
    public void DeleteNode_AddsDeleteRangeStep()
    {
        // Act
        Transaction.DeleteRange(TestNode.Empty(), TestNode.Empty());
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<DeleteRangeStep>());
    }
    
    private class TestStep : IStep
    {
        public bool Executed;
        public bool Reverted;
        
        public IReadOnlyList<StepDiff> Execute()
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