using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.Transactions;

public class DTransactionTests
{
    private DScratchDocument Document { get; set; }
    private DTransaction Transaction { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
        Transaction = new DTransaction(Document);
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
        Transaction.Insert(TestNode.Empty(), new NodePath([]), 0);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        var step = Transaction.Steps.Single();
        Assert.That(step, Is.TypeOf<InsertStep>());
    }
    
    [Test]
    public void DeleteNode_AddsDeleteStep()
    {
        // Act
        Transaction.DeleteNode(new NodePath([]), 0);
        
        // Assert
        Assert.That(Transaction.Steps, Has.Count.EqualTo(1));
        Assert.That(Transaction.Steps.Single(), Is.TypeOf<DeleteStep>());
    }
    
    private class TestStep : IStep
    {
        public bool Executed;
        public bool Reverted;
        
        public IReadOnlyList<StepDiff> Execute(DScratchDocument document)
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