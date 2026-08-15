using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

public class UpdateLinkHandlerTest
{
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private UpdateLinkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        var userStateServiceFake = new UserStateServiceFake();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: userStateServiceFake)
        {
            DisableCleanUp = true
        };
        
        handler = new UpdateLinkHandler(dScratchService);
    }

    [Test]
    public void DoesNothing_WithSelection()
    {
        // Arrange
        LinkNode node = null!;
        builder.Paragraph(t =>
        {
            node = t.Link("dscratch.darki.dev", "_blank", tt =>
            {
                tt.Text("abc");
            });
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(node.FirstChild!.Id, 1, node.FirstChild!.Id, 2);
        var result = handler.Execute(keyPressInfo.Selection!, new UpdateLinkCommand("www.google.com","_self"));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Href, Is.EqualTo("dscratch.darki.dev"));
            Assert.That(node.Target, Is.EqualTo("_blank"));
            Assert.That(result.CursorPosition, Is.Null);
        }
    }

    [Test]
    public void UpdatesAttributesFromGivenLinkNode()
    {
        // Arrange
        LinkNode node = null!;
        builder.Paragraph(t =>
        {
            node = t.Link("dscratch.darki.dev", "_blank", tt =>
            {
                tt.Text("abc");
            });
        });
        
        // Act
        var result = handler.Execute(new SelectionInfo
        {
            AnchorId = node.FirstChild!.Id.Value,
            AnchorOffset = 0,
            FocusId = node.FirstChild!.Id.Value,
            FocusOffset = 0
        }, new UpdateLinkCommand("www.google.com","_self"));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node.Href, Is.EqualTo("www.google.com"));
            Assert.That(node.Target, Is.EqualTo("_self"));
            Assert.That(result.CursorPosition, Is.Null);
        }
    }
}