using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

public class RemoveLinkHandlerTest
{
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private RemoveLinkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: new UserStateService()) { DisableCleanUp = true };
        
        handler = new RemoveLinkHandler(dScratchService);
    }

    [Test]
    public void DoesNothing_WhenSelectionDirectionIsNotNone()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Link("", tt => tt.Text("abc"));
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(parent.FirstChild!.FirstChild!.Id, 1, 2);
        var result = handler.Execute(keyPressInfo.Selection!, new RemoveLinkCommand());
        
        // Assert
        Assert.That(result.IsEmpty, Is.True);
        Assert.That(parent.FirstChild, Is.TypeOf<LinkNode>());
    }
    
    [Test]
    public void DoesNothing_WhenSelectionIsNotInALink()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abc");
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.FirstChild!.Id, 1);
        var result = handler.Execute(keyPressInfo.Selection!, new RemoveLinkCommand());
        
        // Assert
        Assert.That(result.IsEmpty, Is.True);
    }
    
    [Test]
    public void RemovesLink_AndMovesTextOutOfLink()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Link("dscratch.darki.dev", tt =>
            {
                tt.Text("abc");
                tt.Text("def");
            });
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.FirstChild!.FirstChild!.Id, 1);
        var result = handler.Execute(keyPressInfo.Selection!, new RemoveLinkCommand());
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[2], Is.TypeOf<LinkNode>());
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.False);
            Assert.That(parent.ChildNodes[2].IsDeleted, Is.True);

            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            Assert.That(parent.ChildNodes[2].ChildNodes, Has.Count.Zero);
        }

        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, keyPressInfo.Selection!);
    }
}