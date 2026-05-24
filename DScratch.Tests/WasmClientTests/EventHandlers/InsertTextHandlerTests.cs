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
        serviceMock.Setup(s => s.StartTransaction()).Returns(transactionMock.Object);

        handler = new InsertTextHandler(serviceMock.Object);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges()
    {
        // Arrange
        var existingChar = new CharNode('a', "3", null, null);
        var child = new TextNode("2", null, null, [existingChar]);
        existingChar.Parent = child;
        var parent = new TextNode("1", null, null, [child]);
        child.Parent = parent;
        var textNode = new TextNode("4", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(1));
        
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
        var existingChar1 = new CharNode('a', "3", null, null);
        var child = new TextNode("2", null, null, [existingChar1]);
        existingChar1.Parent = child;
        var parent = new TextNode("1", null, null, [child]);
        child.Parent = parent;
        var textNode = new TextNode("4", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(0));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode?>(n => n == null),
            rightOrigin: It.Is<TextNode>(n => n == child)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TextNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingAtEnd()
    {
        // Arrange
        var existingChar1 = new CharNode('a', "5", null, null);
        var child1 = new TextNode("4", null, null, [existingChar1]);
        existingChar1.Parent = child1;
        
        var existingChar2 = new CharNode('a', "3", null, null);
        var child2 = new TextNode("2", child1, null, [existingChar2]);
        existingChar1.Parent = child2;
        child1.RightOrigin = child2;
        
        var parent = new TextNode("1", null, null, [child1, child2]);
        child1.Parent = parent;
        var textNode = new TextNode("6", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(2));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode?>(n => n == child2),
            rightOrigin: It.Is<TextNode?>(n => n == null)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TextNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingBetween()
    {
        // Arrange
        var existingChar1 = new CharNode('a', "5", null, null);
        var child1 = new TextNode("4", null, null, [existingChar1]);
        existingChar1.Parent = child1;
        
        var existingChar2 = new CharNode('a', "3", null, null);
        var child2 = new TextNode("2", child1, null, [existingChar2]);
        existingChar2.Parent = child2;
        child1.RightOrigin = child2;
        
        var parent = new TextNode("1", null, null, [child1, child2]);
        child1.Parent = parent;
        child2.Parent = parent;
        var textNode = new TextNode("6", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(1));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode>(n => n == child1),
            rightOrigin: It.Is<TextNode>(n => n == child2)));
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
                Direction = SelectionDirection.None,
                Offset = offset,
                End = [],
                EndOffset = 0
            }
        };
    }
}