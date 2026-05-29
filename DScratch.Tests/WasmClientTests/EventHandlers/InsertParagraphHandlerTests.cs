using DScratch.Client.BrowserInteractions.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.WasmClientTests.Helpers;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class InsertParagraphHandlerTests
{
    private DScratchDocument document = null!;
    private IDScratchService service;

    private InsertParagraphHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;

    [SetUp]
    public void SetUp()
    {
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        document = new DScratchDocument(builder.Root);
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);
        handler = new InsertParagraphHandler(service);
    }

    [Test]
    public void CreatesExpectedChanges_WithOffsetOfZero()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abc");
        });
        
        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 0));

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.Origin, Is.Not.Null);
            Assert.That(parent.Origin.RightOrigin, Is.EqualTo(parent));
            Assert.That(parent.Origin.ChildNodes, Has.Count.Zero);
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
    }
    
    [Test]
    public void CreatesExpectedChanges_WithOffsetAtEnd()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abc");
        });
        
        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 3));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.Zero);
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
    }
    
    [Test]
    public void CreatesExpectedChanges_WithOffsetInBetweenText()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abc");
        });
        
        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(),1));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("bc"));
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
    }
    
    [Test]
    public void CreatesExpectedChanges_WithOffsetInBetweenText_WithTextNodeForEachChar()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("a");
            t.Text("b");
            t.Text("c");
        });
        
        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 1));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(((TextNode)parent.RightOrigin.ChildNodes[0]).TextContent, Is.EqualTo("b"));
            Assert.That(((TextNode)parent.RightOrigin.ChildNodes[1]).TextContent, Is.EqualTo("c"));
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
    }
    
    [Test]
    public void CreatesExpectedChanges_WithOffsetInBetweenText_WithTextNodeForEachChar_TwoInsertParagraph()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("a");
            t.Text("b");
            t.Text("c");
        });
        
        // Act
        var result1 = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 1));
        var result2 = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.RightOrigin!.GetElementPath(), 1));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.RightOrigin.ChildNodes[0]).TextContent, Is.EqualTo("b"));
            
            Assert.That(parent.RightOrigin.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.RightOrigin.RightOrigin.ChildNodes[0]).TextContent, Is.EqualTo("c"));
        }
        AssertHelper.ThatCursorPositionEqualTo(result1.CursorPosition, parent.RightOrigin.Id, 0);
        AssertHelper.ThatCursorPositionEqualTo(result2.CursorPosition, parent.RightOrigin.RightOrigin.Id, 0);
    }
    
    [Test]
    public void CreatesExpectedChanges_WithSelection()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abcde");
        });
        
        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(), 1, 4));
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("e"));
        }
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
    }
}