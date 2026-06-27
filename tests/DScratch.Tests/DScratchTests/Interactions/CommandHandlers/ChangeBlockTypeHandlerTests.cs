using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

public class ChangeBlockTypeHandlerTests
{
    private TreeBuilder builder;
    private ITransaction transaction;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        transaction = new DTransaction(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator),
            nodeIdGenerator: builder.IdGenerator, 
            disableCleanUp: false);
    }

    [Test]
    public void MakesExpectedChanges()
    {
        // Arrange
        DNode target = null!;
        builder.TestBlockElementNode();
        var blockNode = builder.TestBlockElementNode(t => target = t.Text("abc"));
        builder.TestBlockElementNode();
        
        var selection = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            AnchorId = target.Id.Value,
            AnchorOffset = 2,
            FocusId = target.Id.Value,
            FocusOffset = 2
        };
        
        // Act
        ChangeBlockTypeHandler.Execute(transaction, selection, BlockNodeType.Paragraph);
        var result = transaction.Commit();
        
        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
        var newBlockNode = transaction.FindNode(blockNode.Id);
        Assert.That(newBlockNode, Is.TypeOf<ParagraphNode>());
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.Root.ChildNodes[0], Is.TypeOf<TestBlockElementNode>());
            Assert.That(builder.Root.ChildNodes[1], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[2], Is.TypeOf<TestBlockElementNode>());
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, target.Id, 2);
    }
    
    [Test]
    public void MakesExpectedChanges_InGivenSelectionRange()
    {
        // Arrange
        DNode targetStart = null!;
        DNode targetEnd = null!;
        builder.TestBlockElementNode();
        builder.TestBlockElementNode(t => targetStart = t.Text("a"));
        builder.TestBlockElementNode();
        builder.TestBlockElementNode(t => targetEnd = t.Text("b"));
        builder.TestBlockElementNode();
        
        var selection = new SelectionInfo
        {
            Direction = SelectionDirection.Forward,
            AnchorId = targetStart.Id.Value,
            AnchorOffset = 2,
            FocusId = targetEnd.Id.Value,
            FocusOffset = 3
        };
        
        // Act
        ChangeBlockTypeHandler.Execute(transaction, selection, BlockNodeType.Paragraph);
        var result = transaction.Commit();
        
        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.Root.ChildNodes[0], Is.TypeOf<TestBlockElementNode>());
            Assert.That(builder.Root.ChildNodes[1], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[2], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[3], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[4], Is.TypeOf<TestBlockElementNode>());
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, targetStart.Id, 2);
    }
    
    [Test]
    public void MakesExpectedChanges_InGivenSelectionRange_Reversed()
    {
        // Arrange
        DNode targetStart = null!;
        DNode targetEnd = null!;
        builder.TestBlockElementNode();
        builder.TestBlockElementNode(t => targetEnd = t.Text("a"));
        builder.TestBlockElementNode();
        builder.TestBlockElementNode(t => targetStart = t.Text("b"));
        builder.TestBlockElementNode();
        
        var selection = new SelectionInfo
        {
            Direction = SelectionDirection.Backward,
            AnchorId = targetStart.Id.Value,
            AnchorOffset = 3,
            FocusId = targetEnd.Id.Value,
            FocusOffset = 2
        };
        
        // Act
        ChangeBlockTypeHandler.Execute(transaction, selection, BlockNodeType.Paragraph);
        var result = transaction.Commit();
        
        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.Root.ChildNodes[0], Is.TypeOf<TestBlockElementNode>());
            Assert.That(builder.Root.ChildNodes[1], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[2], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[3], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[4], Is.TypeOf<TestBlockElementNode>());
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, targetEnd.Id, 2);
    }
}