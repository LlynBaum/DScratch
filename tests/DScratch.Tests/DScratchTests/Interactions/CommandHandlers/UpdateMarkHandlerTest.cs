using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Interactions.UserStates;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

[TestFixture]
public class UpdateMarkHandlerTest
{
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private UserStateServiceFake userStateServiceFake;
    private UpdateMarkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        userStateServiceFake = new UserStateServiceFake();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: userStateServiceFake)
        {
            DisableCleanUp = true
        };
        
        handler = new UpdateMarkHandler(dScratchService, userStateServiceFake);
    }

    [Test]
    public void SelectionRange_CallsExecuteOnCommand_WithNodeInGivenSelectionOverDifferentBlocks()
    {
        // Arrange
        var command = new SpyCommand();
        
        TextNode start = null!;
        TextNode end = null!;
        var parent1 = builder.Paragraph(t =>
        {
            start = t.Text("abc");
            t.Text("def");
        });
        var parent2 = builder.Paragraph(t =>
        {
            t.Text("123");
            end = t.Text("456");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
        
        // Act
        var result = handler.Execute(keyPressInfo.Selection, command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(command.AddPendingCalled, Is.False);
            Assert.That(command.ExecuteCall, Is.Not.Empty);
            Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
        }
        
        Assert.That(command.ExecuteCall, Is.EquivalentTo([
            start.RightOrigin,
            start.RightOrigin!.RightOrigin,
            end.Origin,
            end
        ]));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = start.RightOrigin!.Id.Value,
            AnchorOffset = 0,
            FocusId = end.Id.Value,
            FocusOffset = 1
        });
    }
    
    [Test]
    public void SelectionRange_CallsExecuteOnCommand_WithNodeInGivenSelectionOfSiblingNodes()
    {
        // Arrange
        var command = new SpyCommand();
        
        TextNode start = null!;
        TextNode end = null!;
        var parent = builder.Paragraph(t =>
        {
            start = t.Text("ab");
            end = t.Text("cd");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

        // Act
        var result = handler.Execute(keyPressInfo.Selection, command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
        }

        Assert.That(command.ExecuteCall, Is.EquivalentTo([
            start.RightOrigin,
            end
        ]));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = end.Id.Value,
            AnchorOffset = 1,
            FocusId = start.RightOrigin!.Id.Value,
            FocusOffset = 0,
            Direction = SelectionDirection.Backward
        });
    }
    
    [Test]
    public void SelectionRange_CallsExecuteOnCommand_WithNodeInGivenSelection()
    {
        // Arrange
        var command = new SpyCommand();
        
        TextNode start = null!;
        TextNode mid = null!;
        TextNode end = null!;
        var parent = builder.Paragraph(t =>
        {
            start = t.Text("ab");
            mid = t.Text("cd");
            end = t.Text("ef");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 0, end.Id, 2);

        // Act
        var result = handler.Execute(keyPressInfo.Selection, command);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        }

        Assert.That(command.ExecuteCall, Is.EquivalentTo([
            start,
            mid,
            end
        ]));

        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = start.Id.Value,
            AnchorOffset = 0,
            FocusId = end.Id.Value,
            FocusOffset = 2
        });
    }

    [Test]
    public void SelectionDirectionNone_CallsAddPendingOnCommand()
    {
        // Arrange
        var command = new SpyCommand();
            
        TextNode textNode = null!;
        builder.Paragraph(t =>
        {
            textNode = t.Text("a");
        });
            
        // Act
        var result = handler.Execute(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0).Selection, command);
            
        // Assert
        Assert.That(command.AddPendingCalled, Is.True);
        Assert.That(command.ExecuteCall, Is.Empty);
        Assert.That(result.CursorPosition, Is.Null);
    }

    [Test]
    public void EmptyBlocks_CallsExecuteOnCommand_WithBlocksAsNodes()
    {
        // Arrange
        var command = new SpyCommand();
        var block = builder.Paragraph();
            
        // Act
        var result = handler.Execute(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(block.Id, 0).Selection, command);
            
        // Assert
        Assert.That(command.ExecuteCall, Is.EquivalentTo([block]));
        Assert.That(result.CursorPosition, Is.Null);
    }

    private class SpyCommand : IMarkCommand
    {
        public bool AddPendingCalled { get; private set; } = false;

        public readonly List<DNode> ExecuteCall = [];
        
        public void Execute(ITransaction transaction, DNode anchor, IEnumerable<DNode> nodes)
        {
            ExecuteCall.AddRange(nodes);
        }

        public void AddPending(IUserStateService userStateService)
        {
            AddPendingCalled = true;
        }
    }
}