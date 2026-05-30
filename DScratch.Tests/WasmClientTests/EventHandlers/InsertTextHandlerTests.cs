using DScratch.Client.BrowserInteractions.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.WasmClientTests.Helpers;
using DScratch.Transactions;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

public class InsertTextHandlerTests
{
    private DScratchDocument document = null!;
    private IDScratchService service;

    private InsertTextHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;

    [SetUp]
    public void SetUp()
    {
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        document = new DScratchDocument(builder.Root);
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);
        handler = new InsertTextHandler(service);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges()
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text(c => 
            {
                c.Char('a');
            });
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 1));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(parent.LastChild, Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.LastChild!).TextContent, Is.EqualTo("abc"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 4);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingAtStart()
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("a");
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 0));
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(parent.FirstChild, Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 3);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingAtEnd()
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("a");
            t.Text("a");
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(), 2));
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("abc"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 5);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WithInsertingBetween()
    {
        // Arrange
        var parent = builder.TestInlineElementNode(t => 
        {
            t.Text("a");
            t.Text("a");
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.GetElementPath(),1));
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("abc"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 4);
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
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(),start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(6));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));
            
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[2]).IsDeleted, Is.True);
            
            Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[3]).IsDeleted, Is.True);
            
            Assert.That(parent.ChildNodes[4], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("f"));
            
            Assert.That(parent.ChildNodes[5], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[5]).TextContent, Is.EqualTo("ghi"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, 
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 5);
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
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(), start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));
            
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("def"));
            
            Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("ghi"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, 
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 3);
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

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(), start, end));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));
            
            Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("ef"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, 
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 5);
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
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(), start, end));
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            
            Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("xyz"));
        }
        AssertHelper.ThatStepsEqualTo(result.Steps, 
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>());
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 9);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverTwoParagraphs()
    {
        // Arrange
        var parent = builder.Paragraph(t => 
        {
            t.Text("abc");
        });
        var parent2 = builder.Paragraph(t => 
        {
            t.Text("def");
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(), 2, parent2.GetElementPath(), 1));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent2.IsDeleted, Is.True);
            Assert.That(parent2.ChildNodes, Has.Count.Zero);
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));
            
            Assert.That(parent.ChildNodes[4], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("ef"));
        }
        
        AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>(),
            Is.TypeOf<StepDiff.DeleteElementDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>()
        ]);
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 5);
    }
    
    [Test]
    public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverThreeParagraphs()
    {
        // Arrange
        var parent = builder.Paragraph(t => 
        {
            t.Text("abc");
        });
        var parent2 = builder.Paragraph(t => 
        {
            t.Text("def");
        });
        var parent3 = builder.Paragraph(t => 
        {
            t.Text("ghi");
        });

        // Act
        var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.GetElementPath(), 2, parent3.GetElementPath(), 1));
        
        var visualizer = new DocumentVisualizer(document);
        visualizer.Print();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent2.IsDeleted, Is.True);
            Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(parent3.IsDeleted, Is.True);
            Assert.That(parent3.ChildNodes, Has.Count.Zero);
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
            
            Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));
            
            Assert.That(parent.ChildNodes[4], Is.TypeOf<TextNode>());
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("hi"));
        }
        
        AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.DeleteTextDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>(),
            Is.TypeOf<StepDiff.DeleteElementDiff>(),
            Is.TypeOf<StepDiff.DeleteElementDiff>(),
            Is.TypeOf<StepDiff.InsertTextDiff>()
        ]);
        AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 5);
    }
}