using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps.Marks;

namespace DScratch.Tests.DScratchTests.Transactions.Steps.Marks;

public class RemoveMarkStepTest
{
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        transactionFake = new TestTransactionFake();
    }
    
    [Test]
    public void AddsGivenMark_ToGivenNode()
    {
        // Arrange
        var node = new TextNode(new NodeId("", 1), null, null);
        node.SetMark(new Mark(MarkKey.FontWeight, "bold"));
        
        // Act
        var step = new RemoveMarkStep(node, MarkKey.FontWeight);
        var result = step.Execute(transactionFake, null!);

        // Assert
        Assert.That(node.Marks, Has.Count.Zero);
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        AssertHelper.ThatStepsEqualTo(result, Is.TypeOf<StepDiff.UpdateMarksDiff>());
    }
}