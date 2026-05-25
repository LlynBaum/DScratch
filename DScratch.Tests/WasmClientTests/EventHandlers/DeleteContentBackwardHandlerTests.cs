using DScratch.Client.Scripts;
using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.WasmClientTests.Helpers;
using DScratch.Transactions;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class DeleteContentBackwardHandlerTests
{
    private DScratchDocument document;
    private IDScratchService service;

    private DeleteContentBackwardHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;
    
    [SetUp]
    public void SetUp()
    {
        document = new DScratchDocument();
        idGenerator = new TestNodeIdGenerator();
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);
        
        builder = new TreeBuilder(idGenerator);
        handler = new DeleteContentBackwardHandler(service);
    }

    [Test]
    public void Handle_DoesNothing_WhenNodesDoesNotExist()
    {
        // Arrange
        var parent = builder.Text(p =>
        {
            p.Char('a');
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(2));

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
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(3));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(char3.IsDeleted, Is.True);
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
        }
        Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_LastCharInTextNode()
    {
        // Arrange
        CharNode char2 = null!;
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text(txt =>
            {
                txt.Char('a');
                char2 = txt.Char('b');
            });
            t.Text("c");
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(2));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(char2.IsDeleted, Is.True);
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
        }
        Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_SingleCharInNode()
    {
        // Arrange
        CharNode char1 = null!;
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text(txt =>
            {
                char1 = txt.Char('a');
            });
            t.Text("bc");
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(1));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(char1.IsDeleted, Is.True);
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
        }
        Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
    }
    
    [Test]
    [TestCase(2, 5)]
    [TestCase(5, 2)]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelected(int start, int end)
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("abc");
            t.Text("def");
            t.Text("ghi");
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            
            Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("f"));
            
            Assert.That(parent.ChildNodes[4], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("ghi"));
            
            Assert.That(result.Diffs, Has.Count.EqualTo(2));
            Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
            Assert.That(result.Diffs[1], Is.TypeOf<StepDiff.DeleteTextDiff>());
        }
    }
    
    [Test]
    [TestCase(0, 3)]
    [TestCase(3, 0)]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelected_AtStart(int start, int end)
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("abc");
            t.Text("def");
            t.Text("ghi");
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
            
            Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("def"));
            
            Assert.That(parent.ChildNodes[4], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("ghi"));
            
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
            Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
        }
    }
    
    [Test]
    [TestCase(2, 4)]
    [TestCase(2, 4)]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelected_InBetween(int start, int end)
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("abcdef");
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ef"));
            
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
            Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
        }
    }
    
    [Test]
    [TestCase(6, 9)]
    [TestCase(6, 9)]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelected_AtEnd(int start, int end)
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("abc");
            t.Text("def");
            t.Text("ghi");
        });
        document.Page.Root = parent;

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            
            Assert.That(result.Diffs, Has.Count.EqualTo(2));
            Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
            Assert.That(result.Diffs[1], Is.TypeOf<StepDiff.DeleteTextDiff>());
        }
    }
}