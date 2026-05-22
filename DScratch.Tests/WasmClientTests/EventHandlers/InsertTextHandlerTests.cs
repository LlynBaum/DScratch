using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using Moq;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class InsertTextHandlerTests
{
    private Mock<ITransaction> transactionMock;
    private Mock<INodeFactory> factoryMock;
    private Mock<IDScratchService> serviceMock;

    private InsertTextHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        transactionMock = new Mock<ITransaction>();
        factoryMock = new Mock<INodeFactory>();
        serviceMock = new Mock<IDScratchService>();
        
        serviceMock.Setup(s => s.NodeFactory).Returns(factoryMock.Object);
        serviceMock.Setup(s => s.StartTransaction(It.IsAny<DScratchDocument>())).Returns(transactionMock.Object);

        handler = new InsertTextHandler(serviceMock.Object);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges()
    {
        // Arrange
        var child = new TextNode("2", null, null);
        var parent = new TextNode("1", null, null, [child]);
        var textNode = new TextNode("2", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(1), new DScratchDocument());
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode>(n => n == child),
            rightOrigin: It.Is<TextNode?>(n => n == null)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TextNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingAtStart()
    {
        // Arrange
        var child = new TextNode("2", null, null);
        var parent = new TextNode("1", null, null, [child]);
        var textNode = new TextNode("2", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(0), new DScratchDocument());
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode?>(n => n == null),
            rightOrigin: It.Is<TextNode>(n => n == child)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TextNode>(n => n == parent)));
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