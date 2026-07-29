using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps.Marks;

namespace DScratch.Tests.DScratchTests.Transactions.Steps.Marks;

public class AddMarkStepTest
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
        
        // Act
        var step = new AddMarkStep(node, MarkKey.FontWeight, "bold");
        var result = step.Execute(transactionFake, null!);
        
        // Assert
        Assert.That(node.Marks, Has.Count.EqualTo(1));
        Assert.That(node.Marks.Single().Key, Is.EqualTo(MarkKey.FontWeight));
        Assert.That(node.Marks.Single().Value, Is.EqualTo("bold"));
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([node]));
        AssertHelper.ThatStepsEqualTo(result, Is.TypeOf<StepDiff.UpdateMarksDiff>());
    }
}