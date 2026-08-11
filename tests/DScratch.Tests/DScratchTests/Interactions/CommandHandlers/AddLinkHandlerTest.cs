using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

public class AddLinkHandlerTest
{
    private const string Href = "dscratch.darki.dev";
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private AddLinkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: new UserStateService()) { DisableCleanUp = true };
        
        handler = new AddLinkHandler(dScratchService);
    }

    [Test]
    public void AddsDisplayTextAsTextWithLink_InsertInMiddleOfText()
    {
        const string displayText = "DScratch";
        
        // Arrange
        TextNode target = null!;
        var parent = builder.Paragraph(t =>
        {
            target = t.Text("ab");
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(target.Id, 1);
        var result = handler.Execute(keyPressInfo.Selection!, new AddLinkCommand(Href, "_self", displayText));
        
        // Arrange
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[1], Is.TypeOf<LinkNode>());
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
        }
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("b"));
        }
        
        var linkNode = (LinkNode)parent.ChildNodes[1];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode.Href, Is.EqualTo(Href));
            Assert.That(linkNode.ChildNodes, Has.Count.EqualTo(1));
        }
        
        Assert.That(linkNode.ChildNodes[0], Is.TypeOf<TextNode>());
        Assert.That(((TextNode)linkNode.ChildNodes[0]).TextContent, Is.EqualTo(displayText));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = linkNode.ChildNodes[0].Id.Value,
            AnchorOffset = displayText.Length,
            FocusId = linkNode.ChildNodes[0].Id.Value,
            FocusOffset = displayText.Length
        });
    }
    
    [Test]
    public void AddsDisplayTextAsTextWithLink_InsertAfterText()
    {
        const string displayText = "DScratch";
        
        // Arrange
        TextNode target = null!;
        var parent = builder.Paragraph(t =>
        {
            target = t.Text("ab");
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(target.Id, 2);
        var result = handler.Execute(keyPressInfo.Selection!, new AddLinkCommand(Href, "_self", displayText));
        
        // Arrange
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[1], Is.TypeOf<LinkNode>());
        }
        Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
        
        var linkNode = (LinkNode)parent.ChildNodes[1];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode.Href, Is.EqualTo(Href));
            Assert.That(linkNode.ChildNodes, Has.Count.EqualTo(1));
        }
        
        Assert.That(linkNode.ChildNodes[0], Is.TypeOf<TextNode>());
        Assert.That(((TextNode)linkNode.ChildNodes[0]).TextContent, Is.EqualTo(displayText));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = linkNode.ChildNodes[0].Id.Value,
            AnchorOffset = displayText.Length,
            FocusId = linkNode.ChildNodes[0].Id.Value,
            FocusOffset = displayText.Length
        });
    }
    
    [Test]
    public void AddsDisplayTextAsTextWithLink_InsertInEmptyBlock()
    {
        const string displayText = "DScratch";
        
        // Arrange
        var parent = builder.Paragraph();
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 2);
        var result = handler.Execute(keyPressInfo.Selection!, new AddLinkCommand(Href, "_self", displayText));
        
        // Arrange
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<LinkNode>());
        }

        var linkNode = (LinkNode)parent.ChildNodes[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode.Href, Is.EqualTo(Href));
            Assert.That(linkNode.ChildNodes, Has.Count.EqualTo(1));
        }
        
        Assert.That(linkNode.ChildNodes[0], Is.TypeOf<TextNode>());
        Assert.That(((TextNode)linkNode.ChildNodes[0]).TextContent, Is.EqualTo(displayText));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = linkNode.ChildNodes[0].Id.Value,
            AnchorOffset = displayText.Length,
            FocusId = linkNode.ChildNodes[0].Id.Value,
            FocusOffset = displayText.Length
        });
    }
    
    [Test]
    public void AddsDisplayTextAsTextWithLink_InsertBeforeText()
    {
        const string displayText = "DScratch";
        
        // Arrange
        TextNode target = null!;
        var parent = builder.Paragraph(t =>
        {
            target = t.Text("ab");
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(target.Id, 0);
        var result = handler.Execute(keyPressInfo.Selection!, new AddLinkCommand(Href, "_self", displayText));
        
        // Arrange
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<LinkNode>());
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
        }
        Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("ab"));
        
        var linkNode = (LinkNode)parent.ChildNodes[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode.Href, Is.EqualTo(Href));
            Assert.That(linkNode.ChildNodes, Has.Count.EqualTo(1));
        }
        
        Assert.That(linkNode.ChildNodes[0], Is.TypeOf<TextNode>());
        Assert.That(((TextNode)linkNode.ChildNodes[0]).TextContent, Is.EqualTo(displayText));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = linkNode.ChildNodes[0].Id.Value,
            AnchorOffset = displayText.Length,
            FocusId = linkNode.ChildNodes[0].Id.Value,
            FocusOffset = displayText.Length
        });
    }

    [Test]
    public void AddsLinkToSelection()
    {
        // Arrange
        TextNode originTarget = null!;
        TextNode rightOriginTarget = null!;
        var parent = builder.Paragraph(t =>
        {
            t.Text("ab");
            originTarget = t.Text("cd");
            rightOriginTarget = t.Text("ef");
            t.Text("gh");
        });
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(originTarget.Id, 1, rightOriginTarget.Id, 1);
        var result = handler.Execute(keyPressInfo.Selection!, new AddLinkCommand(Href, "_self"));
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[2], Is.TypeOf<LinkNode>());
            Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
            Assert.That(parent.ChildNodes[4], Is.TypeOf<TextNode>());
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("f"));
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("gh"));
        }

        var linkNode = (LinkNode)parent.ChildNodes[2];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode.Href, Is.EqualTo(Href));
            Assert.That(linkNode.ChildNodes, Has.Count.EqualTo(2));
        }
        
        Assert.That(linkNode.ChildNodes[0], Is.TypeOf<TextNode>());
        Assert.That(linkNode.ChildNodes[1], Is.TypeOf<TextNode>());
        Assert.That(((TextNode)linkNode.ChildNodes[0]).TextContent, Is.EqualTo("d"));
        Assert.That(((TextNode)linkNode.ChildNodes[1]).TextContent, Is.EqualTo("e"));
        
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = linkNode.ChildNodes[1].Id.Value,
            AnchorOffset = 1,
            FocusId = linkNode.ChildNodes[1].Id.Value,
            FocusOffset = 1
        });
    }
    
    [Test]
    public void AddsLinkToSelection_ForEachBlockElement()
    {
        // Arrange
        TextNode originTarget = null!;
        TextNode rightOriginTarget = null!;
        var parent1 = builder.Paragraph(t => originTarget = t.Text("ab"));
        var parent2 = builder.Paragraph(t => t.Text("cd"));
        var parent3 = builder.Paragraph(t => rightOriginTarget = t.Text("ef"));
        
        // Act
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(originTarget.Id, 1, rightOriginTarget.Id, 1);
        var result = handler.Execute(keyPressInfo.Selection!, new AddLinkCommand(Href, "_self"));
        builder.Print();
        
        // Assert
        Assert.That(parent1.ChildNodes, Has.Count.EqualTo(2));
        Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
        Assert.That(parent3.ChildNodes, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent1.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(parent1.ChildNodes[1], Is.TypeOf<LinkNode>());
            
            Assert.That(parent2.ChildNodes[0], Is.TypeOf<LinkNode>());
            
            Assert.That(parent3.ChildNodes[0], Is.TypeOf<LinkNode>());
            Assert.That(parent3.ChildNodes[1], Is.TypeOf<TextNode>());
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent1.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            Assert.That(((TextNode)parent3.ChildNodes[1]).TextContent, Is.EqualTo("f"));
        }

        var linkNode1 = (LinkNode)parent1.ChildNodes[1];
        var linkNode2 = (LinkNode)parent2.ChildNodes[0];
        var linkNode3 = (LinkNode)parent3.ChildNodes[0];
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode1.Href, Is.EqualTo(Href));
            Assert.That(linkNode1.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(linkNode2.Href, Is.EqualTo(Href));
            Assert.That(linkNode2.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(linkNode3.Href, Is.EqualTo(Href));
            Assert.That(linkNode3.ChildNodes, Has.Count.EqualTo(1));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(linkNode1.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)linkNode1.ChildNodes[0]).TextContent, Is.EqualTo("b"));
            
            Assert.That(linkNode2.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)linkNode2.ChildNodes[0]).TextContent, Is.EqualTo("cd"));
            
            Assert.That(linkNode3.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)linkNode3.ChildNodes[0]).TextContent, Is.EqualTo("e"));
        }

        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
        {
            AnchorId = linkNode3.ChildNodes[0].Id.Value,
            AnchorOffset = 1,
            FocusId = linkNode3.ChildNodes[0].Id.Value,
            FocusOffset = 1
        });
    }
}