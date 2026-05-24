using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class DeleteContentBackwardHandlerTests
{
    private DScratchDocument document;
    private IDScratchService service;

    private DeleteContentBackwardHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        document = new DScratchDocument();
        var idGenerator = new TestNodeIdGenerator();
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);

        handler = new DeleteContentBackwardHandler(service);
    }

    [Test]
    public void Handle_DoesNothing_WhenNodesDoesNotExist()
    {
        // Arrange
        var builder = new TreeBuilder();
        var parent = builder.Text(p =>
        {
            p.Char('a');
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(GetKeyPressInfo(2));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.IsDeleted, Is.False);
            Assert.That(result.IsEmpty, Is.True);
        }
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges()
    {
        // Arrange
        CharNode char3 = null!;

        var builder = new TreeBuilder();
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("ab");
            t.Text(txt => 
            {
                char3 = txt.Char('a');
            });
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(GetKeyPressInfo(3));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(char3.IsDeleted, Is.True);
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
        }
        Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
    }
    
    private static KeyPressInfo GetKeyPressInfo(int offset)
    {
        return new KeyPressInfo
        {
            Data = "abc",
            Path = ["0"],
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