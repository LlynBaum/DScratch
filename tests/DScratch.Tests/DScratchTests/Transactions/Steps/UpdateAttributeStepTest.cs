using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class UpdateAttributeStepTest
{
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        transactionFake = new TestTransactionFake();
    }

    [Test]
    public void Execute_InvokesGivenAction()
    {
        // Arrange
        var invoked = false;
        var node = new LinkNode(new NodeId("", 1), null, null, "test", "test");

        // Act
        var result = new UpdateAttributeStep(node, Update).Execute(transactionFake, null!);
        
        // Assert
        Assert.That(invoked, Is.True);
        Assert.That(result, Is.Not.Empty);
        return;

        void Update() => invoked = true;
    }
}