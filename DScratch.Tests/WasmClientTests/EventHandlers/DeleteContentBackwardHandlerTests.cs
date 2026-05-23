using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
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
        transactionMock.Verify(t => t.Delete(node: It.IsAny<TextNode>()), Times.Never);
        Assert.That(result.IsEmpty, Is.True);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges()
    {
        // Arrange
        var char1 = new CharNode('a', "6", null, null);
        var char2 = new CharNode('a', "5", null, null);
        var char3 = new CharNode('a', "4", null, null);
        var child1 = new TextNode("3", null, null, [char1, char2]);
        char1.Parent = child1;
        char2.Parent = child1;
        var child2 = new TextNode("2", child1, null, [char3]);
        char3.Parent = child1;
        child1.RightOrigin = child2;
        var parent = new TestInlineElementNode("1", null, null, [child1, child2]);
        child1.Parent = parent;
        child2.Parent = parent;
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(3));
        
        // Assert
        transactionMock.Verify(t => t.Delete(node: It.Is<CharNode>(n => n == char3)));
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
                Direction = SelectionDirection.None,
                Offset = offset,
                End = [],
                EndOffset = 0
            }
        };
    }
}