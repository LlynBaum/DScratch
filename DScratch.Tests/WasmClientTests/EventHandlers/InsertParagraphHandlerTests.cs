using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.WasmClientTests.Helpers;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class InsertParagraphHandlerTests
{
    private DScratchDocument document;
    private IDScratchService service;

    private InsertParagraphHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;

    [SetUp]
    public void SetUp()
    {
        document = new DScratchDocument();
        idGenerator = new TestNodeIdGenerator();
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);

        builder = new TreeBuilder(idGenerator);
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
        document.Page.Root = parent;
        
        // Act
        handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(0));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.Origin, Is.Not.Null);
            Assert.That(parent.Origin.RightOrigin, Is.EqualTo(parent));
            Assert.That(parent.Origin.ChildNodes, Has.Count.Zero);
        }
    }
    
    [Test]
    public void CreatesExpectedChanges_WithOffsetAtEnd()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abc");
        });
        document.Page.Root = parent;
        
        // Act
        handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(3));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.Zero);
        }
    }
    
    [Test]
    public void CreatesExpectedChanges_WithOffsetInBetweenText()
    {
        // Arrange
        var parent = builder.Paragraph(t =>
        {
            t.Text("abc");
        });
        document.Page.Root = parent;
        
        // Act
        handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(1));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.RightOrigin, Is.Not.Null);
            Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
            Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("bc"));
        }
    }
}