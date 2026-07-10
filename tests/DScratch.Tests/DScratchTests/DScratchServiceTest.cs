using DScratch.LayoutEngine;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using Moq;

namespace DScratch.Tests.DScratchTests;

public class DScratchServiceTest
{
    [Test]
    public async Task Apply_ReturnsTransactionResult()
    {
        // Arrange
        var transactionMock = new Mock<ITransaction>();
        var layoutMock = new Mock<ILayoutEngineService>();
        var service = new DScratchService(Mock.Of<INodeFactory>(), Mock.Of<INodeIdGenerator>(), layoutMock.Object);

        var modifiedNode = new ModifiedNode(TestNode.Empty(), Modification.Insert);
        
        transactionMock.Setup(t => t.Commit()).Returns(new TransactionResult(new HashSet<ModifiedNode>
        {
            modifiedNode
        }));
        
        // Act
        await service.ApplyAsync(transactionMock.Object);
        
        // Assert
        transactionMock.Verify(t => t.Commit(), Times.Once);
        layoutMock.Verify(l => l.RenderAsync(
            It.IsAny<DScratchDocument>(),
            It.Is<TransactionResult>(m => modifiedNode == m.ModifiedNodes.Single())), Times.Once);
    }
}