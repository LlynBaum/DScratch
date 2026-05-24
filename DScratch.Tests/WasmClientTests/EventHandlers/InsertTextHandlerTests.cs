using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
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
        TextNode child = null!;

        var builder = new TreeBuilder();
        var parent = builder.TestInlineElementNode(t => 
        {
            child = t.Text(c => 
            {
                c.Char('a');
            });
        });
        var textNode = new TextNode("4", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetDefaultKeyPressInfo(1));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode>(n => n == child),
            rightOrigin: It.Is<TextNode?>(n => n == null)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TestInlineElementNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingAtStart()
    {
        // Arrange
        TextNode child = null!;

        var builder = new TreeBuilder();
        var parent = builder.TestInlineElementNode(t => 
        {
            child = t.Text(c => 
            {
                c.Char('a');
            });
        });
        var textNode = new TextNode("4", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetDefaultKeyPressInfo(0));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode?>(n => n == null),
            rightOrigin: It.Is<TextNode>(n => n == child)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TestInlineElementNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingAtEnd()
    {
        // Arrange
        TextNode child2 = null!;

        var builder = new TreeBuilder();
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text(c => c.Char('a'));
            child2 = t.Text(c => c.Char('a'));
        });
        var textNode = new TextNode("6", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetDefaultKeyPressInfo(2));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode?>(n => n == child2),
            rightOrigin: It.Is<TextNode?>(n => n == null)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TestInlineElementNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingBetween()
    {
        // Arrange
        TextNode child1 = null!;
        TextNode child2 = null!;

        var builder = new TreeBuilder();
        var parent = builder.TestInlineElementNode(t => 
        {
            child1 = t.Text(c => c.Char('a'));
            child2 = t.Text(c => c.Char('a'));
        });
        var textNode = new TextNode("6", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetDefaultKeyPressInfo(1));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode>(n => n == child1),
            rightOrigin: It.Is<TextNode>(n => n == child2)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TestInlineElementNode>(n => n == parent)));
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelected()
    {
        // Arrange
        TextNode child1 = null!;
        TextNode child2 = null!;

        var builder = new TreeBuilder();
        var parent = builder.Text(p => 
        {
            var t = (TreeBuilder)p;
            child1 = t.Text(c => c.Char('a'));
            child2 = t.Text(c => c.Char('a'));
        });
        var textNode = new TextNode("6", null, null);
        
        transactionMock.Setup(t => t.FindNode(It.IsAny<NodePath>())).Returns(parent);
        factoryMock.Setup(f => f.String(It.IsAny<string>(), It.IsAny<DNode?>(), It.IsAny<DNode?>()))
            .Returns(textNode);
        serviceMock.Setup(s => s.Apply(It.Is<ITransaction>(t => t == transactionMock.Object)))
            .Returns(TransactionResult.Empty);

        // Act
        handler.Handle(GetKeyPressInfo(1, 3));
        
        // Assert
        factoryMock.Verify(f => f.String(
            value: It.Is<string>(s => s == "abc"),
            origin: It.Is<TextNode>(n => n == child1),
            rightOrigin: It.Is<TextNode>(n => n == child2)));
        transactionMock.Verify(t => t.Insert(
            node: It.Is<TextNode>(n => n == textNode), 
            parent: It.Is<TextNode>(n => n == parent)));
    }

    private static KeyPressInfo GetDefaultKeyPressInfo(int offset)
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
    
    private static KeyPressInfo GetKeyPressInfo(int offset, int endOffset)
    {
        var direction = offset < endOffset ? SelectionDirection.Forward : SelectionDirection.Backward;
        return new KeyPressInfo
        {
            Data = "abc",
            Path = [],
            InputType = InsertTextHandler.EventName,
            Selection = new KeyPressInfo.SelectionInfo
            {
                Direction = direction,
                Offset = offset,
                End = [],
                EndOffset = endOffset
            }
        };
    }
}