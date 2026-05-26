using DScratch.Client.Scripts.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.WasmClientTests.Helpers;
using DScratch.Transactions;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class DeleteContentForwardHandlerTests
{
    private DScratchDocument document = null!;
    private IDScratchService service;

    private DeleteContentForwardHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;
    
    [SetUp]
    public void SetUp()
    {
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        document = new DScratchDocument(builder.Root);
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);
        handler = new DeleteContentForwardHandler(service);
    }

    [Test]
    public void Handle_DoesNothing_WhenNodesDoesNotExist()
    {
        // Arrange
        var parent = builder.Text(p =>
        {
            p.Char('a');
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath().Path, 1));

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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath().Path, 2));

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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath().Path, 1));

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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath().Path, 0));

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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath().Path, start, end));
        
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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath().Path, start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ghi"));
            
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

        // Actparent.GetElementPath().Path,
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath().Path, start, end));
        
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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath().Path, start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            
            Assert.That(result.Diffs, Has.Count.EqualTo(1));
            Assert.That(result.Diffs[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
        }
    }
}