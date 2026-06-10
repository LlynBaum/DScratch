using DScratch.Client.BrowserInteractions;
using DScratch.Client.BrowserInteractions.EventHandlers;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.WasmClientTests.Helpers;
using DScratch.Transactions;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.WasmClientTests.EventHandlers;

[TestFixture]
public class DeleteWordBackwardHandlerTests
{
    private DScratchDocument document = null!;
    private IDScratchService service;

    private DeleteWordBackwardHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;
    
    [SetUp]
    public void SetUp()
    {
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        document = builder.CreateDocument();
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator);
        handler = new DeleteWordBackwardHandler(service);
    }
    
    private class SimpleDelete : DeleteWordBackwardHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges()
        {
            // Arrange
            TextNode text1 = null!;
            TextNode text2 = null!;
            var parent = builder.TestInlineElementNode(t => 
            {
                text1 = t.Text("ab");
                text2 = t.Text("c");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 3));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(text1.IsDeleted, Is.True);
                Assert.That(text2.IsDeleted, Is.True);
            }
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(), 
                Is.TypeOf<StepDiff.DeleteTextDiff>()]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_DeletesStartingWhiteSpaces()
        {
            // Arrange
            TextNode text = null!;
            var parent = builder.TestInlineElementNode(t => 
            {
                text = t.Text("ab  ");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 3));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(text.IsDeleted, Is.True);
                Assert.That(text.TextContent, Is.EqualTo("ab "));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            }
            
            Assert.That(text.RightOrigin, Is.TypeOf<TextNode>());
            var remainingText = (TextNode)text.RightOrigin!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(remainingText.IsDeleted, Is.False);
                Assert.That(remainingText.TextContent, Is.EqualTo(" "));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_StopsAtWhiteSpace()
        {
            // Arrange
            TextNode text = null!;
            var parent = builder.TestInlineElementNode(t => 
            {
                text = t.Text(" ab ");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 4));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(text.IsDeleted, Is.False);
                Assert.That(text.TextContent, Is.EqualTo(" "));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            }
            
            Assert.That(text.RightOrigin, Is.TypeOf<TextNode>());
            var tombstone = (TextNode)text.RightOrigin!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(tombstone.IsDeleted, Is.True);
                Assert.That(tombstone.TextContent, Is.EqualTo("ab "));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps,Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 1);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_LastCharInTextNode()
        {
            // Arrange
            TextNode text = null!;
            var parent = builder.TestInlineElementNode(t => 
            {
                text = t.Text("bc");
                t.Text("a");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 2));

            // Assert
            Assert.That(text.IsDeleted, Is.True);
            AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_SingleCharInNode()
        {
            // Arrange
            TextNode text = null!;
            var parent = builder.TestInlineElementNode(t => 
            {
                text = t.Text("a");
                t.Text("bc");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 1));

            // Assert
            Assert.That(text.IsDeleted, Is.True);
            AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }
        
        [Test]
        public void Handle_DoesNothing_WhenWhereIsNoPreviousParagraph()
        {
            // Arrange
            var parent = builder.Paragraph(t => 
            {
                t.Text("abc");
                t.Text("def");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 0));
        
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.IsDeleted, Is.False);
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            
                Assert.That(result.Steps, Has.Count.Zero);
                Assert.That(result.CursorPosition, Is.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));
            
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            }
        }
    }
    
    private class SelectionDelete : DeleteWordBackwardHandlerTests
    {
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
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.Id, start, end));

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

                AssertHelper.ThatStepsEqualTo(actual: result.Steps, expected:
                [
                    Is.TypeOf<StepDiff.DeleteTextDiff>(),
                    Is.TypeOf<StepDiff.DeleteTextDiff>()
                ]);
                AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 2);
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
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.Id, start, end));

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
            }

            AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }

        [Test]
        [TestCase(2, 4)]
        [TestCase(4, 2)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_InBetween(int start, int end)
        {
            // Arrange
            var parent = builder.TestInlineElementNode(t => { t.Text("abcdef"); });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.Id, start, end));

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
            }

            AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 2);
        }

        [Test]
        [TestCase(6, 9)]
        [TestCase(9, 6)]
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
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(parent.Id, start, end));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));

                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            }

            AssertHelper.ThatStepsEqualTo(result.Steps, Is.TypeOf<StepDiff.DeleteTextDiff>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 6);
        }
    }
    
    private class MergeParagraphs : DeleteWordBackwardHandlerTests
    {
        [Test]
        public void Handle_MergesParagraph_WhenAtBeginningOfParagraph()
        {
            // Arrange
            var parent = builder.Paragraph(t => 
            {
                t.Text("abc");
                t.Text("def");
            });
            var oldParent = builder.Paragraph(t =>
            {
                t.Text("ghi");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(oldParent.Id, 0));
        
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(oldParent.IsDeleted, Is.True);
            
                Assert.That(oldParent.ChildNodes, Has.Count.Zero);
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));
            
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ghi"));
            }
        
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 6);
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
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: parent.Id, 
                anchorOffset: 2, 
                focusId: parent2.Id, 
                focusOffset: 1);

            // Act
            var result = handler.Handle(keyPressInfo);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 2);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverTwoParagraphs_Backwards()
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
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: parent2.Id, 
                anchorOffset: 1,
                focusId: parent.Id, 
                focusOffset: 2, 
                direction: SelectionDirection.Backward);

            // Act
            var result = handler.Handle(keyPressInfo);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 2);
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
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: parent.Id, 
                anchorOffset: 2, 
                focusId: parent3.Id, 
                focusOffset: 1);

            // Act
            var result = handler.Handle(keyPressInfo);

            builder.Print();
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("hi"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 2);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverThreeParagraphs_Backwards()
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
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: parent3.Id, 
                anchorOffset: 1,
                focusId: parent.Id, 
                focusOffset: 2, 
                direction: SelectionDirection.Backward);

            // Act
            var result = handler.Handle(keyPressInfo);

            builder.Print();
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("hi"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 2);
        }
    }
}