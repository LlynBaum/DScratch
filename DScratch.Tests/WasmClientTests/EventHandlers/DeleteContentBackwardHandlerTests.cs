using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using Moq;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class DeleteContentBackwardHandlerTests
{
    private Mock<ITransaction> transactionMock;
    private Mock<INodeFactory> factoryMock;
    private Mock<IDScratchService> serviceMock;

    private DeleteContentBackwardHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        transactionMock = new Mock<ITransaction>();
        factoryMock = new Mock<INodeFactory>();
        serviceMock = new Mock<IDScratchService>();
        
        serviceMock.Setup(s => s.NodeFactory).Returns(factoryMock.Object);
        serviceMock.Setup(s => s.StartTransaction()).Returns(transactionMock.Object);

        handler = new DeleteContentBackwardHandler(serviceMock.Object);
    }

    [Test]
    public void Handle_DoesNothing_WhenNodesDoesNotExist()
    {
        // Arrange
        var child = new TextNode("2", null, null);
        var parent = new TextNode("1", null, null, [child]);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        var result = handler.Handle(GetKeyPressInfo(2));
        
        // Assert
        transactionMock.Verify(t => t.DeleteNode(node: It.IsAny<TextNode>()), Times.Never);
        Assert.That(result.IsEmpty, Is.True);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges()
    {
        // Arrange
        var child = new TextNode("2", null, null);
        var parent = new TextNode("1", null, null, [child]);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(1));
        
        // Assert
        transactionMock.Verify(t => t.DeleteNode(node: It.Is<TextNode>(n => n == child)));
    }
    
    private static KeyPressInfo GetKeyPressInfo(int offset)
    {
        return new KeyPressInfo
        {
            Data = "abc",
            Path = [],
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = "",
                Offset = offset,
                End = [],
                EndOffset = 0
            }
        };
    }
}