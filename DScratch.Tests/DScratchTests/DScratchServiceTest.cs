using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using Moq;

namespace DScratch.Tests.DScratchTests;

public class DScratchServiceTest
{
    [Test]
    public void Apply_ReturnsTransactionResult()
    {
        // Arrange
        var transactionMock = new Mock<ITransaction>();
        var service = new DScratchService(Mock.Of<INodeFactory>());

        transactionMock.Setup(t => t.Commit()).Returns(new TransactionResult([new TestStepDiff()]));
        
        // Act
        var result = service.Apply(transactionMock.Object);
        
        // Assert
        transactionMock.Verify(t => t.Commit(), Times.Once);
        Assert.That(result.Diffs, Has.Count.EqualTo(1));
        Assert.That(result.Diffs.Single(), Is.EqualTo(new TestStepDiff()));
    }
}