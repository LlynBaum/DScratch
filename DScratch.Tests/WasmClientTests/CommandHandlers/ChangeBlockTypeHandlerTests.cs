using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.CommandHandlers;
using DScratch.Client.BrowserInteractions.CommandHandlers.Commands;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;

namespace DScratch.Tests.WasmClientTests.CommandHandlers;

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
        builder.TestBlockElementNode();
        var target = builder.TestBlockElementNode();
        builder.TestBlockElementNode();
        
        var selection = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            AnchorId = target.Id.Value,
            AnchorOffset = 2,
            FocusId = target.Id.Value,
            FocusOffset = 3
        };
        
        // Act
        ChangeBlockTypeHandler.Execute(transaction, selection, BlockType.Paragraph);
        var result = transaction.Commit();
        
        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
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
        builder.TestBlockElementNode();
        var start =builder.TestBlockElementNode();
        builder.TestBlockElementNode();
        var end = builder.TestBlockElementNode();
        builder.TestBlockElementNode();
        
        var selection = new SelectionInfo
        {
            Direction = SelectionDirection.Forward,
            AnchorId = start.Id.Value,
            AnchorOffset = 2,
            FocusId = end.Id.Value,
            FocusOffset = 3
        };
        
        // Act
        ChangeBlockTypeHandler.Execute(transaction, selection, BlockType.Paragraph);
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
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, start.Id, 2);
    }
    
    [Test]
    public void MakesExpectedChanges_InGivenSelectionRange_Reversed()
    {
        // Arrange
        builder.TestBlockElementNode();
        var start =builder.TestBlockElementNode();
        builder.TestBlockElementNode();
        var end = builder.TestBlockElementNode();
        builder.TestBlockElementNode();
        
        var selection = new SelectionInfo
        {
            Direction = SelectionDirection.Backward,
            AnchorId = end.Id.Value,
            AnchorOffset = 3,
            FocusId = start.Id.Value,
            FocusOffset = 2
        };
        
        // Act
        ChangeBlockTypeHandler.Execute(transaction, selection, BlockType.Paragraph);
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
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, start.Id, 2);
    }
}