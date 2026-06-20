using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Events;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers;

[TestFixture]
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
        document = builder.CreateDocument();
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator) { DisableCleanUp = true };
        handler = new InsertParagraphHandler(service);
    }

    private class SimpleInsert : InsertParagraphHandlerTests
    {
        [Test]
        public void CreatesExpectedChanges_WithOffsetOfZero()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abc"); });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0));

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
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abc"); });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 3));

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
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abc"); });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

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
            TextNode textNode = null!;
            var parent = builder.Paragraph(t =>
            {
                textNode = t.Text("a");
                t.Text("b");
                t.Text("c");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

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
    }
    
    private class SelectionInsert : InsertParagraphHandlerTests
    {
        [Test]
        public void CreatesExpectedChanges_WithOffsetInBetweenText_WithTextNodeForEachChar_TwoInsertParagraph()
        {
            // Arrange
            TextNode textNode1 = null!;
            TextNode textNode2 = null!;
            var parent = builder.Paragraph(t =>
            {
                textNode1 = t.Text("a");
                textNode2 = t.Text("b");
                t.Text("c");
            });

            // Act
            var result1 = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode1.Id, 1));
            var result2 = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode2.Id, 1));

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
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abcde"); });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, 1, 4));

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

    private class MergeParagraphs : InsertParagraphHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverTwoParagraphs()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => 
            {
                startNode = t.Text("abc");
            });
            var parent2 = builder.Paragraph(t => 
            {
                endNode = t.Text("def");
            });
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: startNode.Id,
                anchorOffset: 2,
                focusId: endNode.Id,
                focusOffset: 1);

            // Act
            var result = handler.Handle(keyPressInfo);
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                
                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
                Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverTwoParagraphs_Backwards()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => 
            {
                endNode = t.Text("abc");
            });
            var parent2 = builder.Paragraph(t => 
            {
                startNode = t.Text("def");
            });
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: startNode.Id,
                anchorOffset: 1,
                focusId: endNode.Id,
                focusOffset: 2,
                direction: SelectionDirection.Backward);

            // Act
            var result = handler.Handle(keyPressInfo);
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent2.Origin, Is.Not.Null);
                Assert.That(parent2.RightOrigin, Is.Null);
            }
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                
                Assert.That(parent.RightOrigin!.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin!.FirstChild!).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverThreeParagraphs()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => 
            {
                startNode = t.Text("abc");
            });
            var parent2 = builder.Paragraph(t => 
            {
                t.Text("def");
            });
            var parent3 = builder.Paragraph(t => 
            {
                endNode = t.Text("ghi");
            });

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: startNode.Id,
                anchorOffset: 2,
                focusId: endNode.Id,
                focusOffset: 1);
            
            // Act
            var result = handler.Handle(keyPressInfo);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                
                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin.Origin, Is.EqualTo(parent));
                Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("hi"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverThreeParagraphs_Backwards()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => 
            {
                endNode = t.Text("abc");
            });
            var parent2 = builder.Paragraph(t => 
            {
                t.Text("def");
            });
            var parent3 = builder.Paragraph(t => 
            {
                startNode = t.Text("ghi");
            });
            
            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: startNode.Id,
                anchorOffset: 1,
                focusId: endNode.Id, 
                focusOffset: 2, 
                direction: SelectionDirection.Backward);

            // Act
            var result = handler.Handle(keyPressInfo);

            builder.Print();
            
            // Assert
            using (Assert.EnterMultipleScope()) 
            {
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent2.Origin, Is.Not.Null);
                Assert.That(parent2.RightOrigin, Is.EqualTo(parent3));
                Assert.That(parent3.Origin, Is.EqualTo(parent2));
                Assert.That(parent3.RightOrigin, Is.Null);
            }
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                
                Assert.That(parent.RightOrigin!.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("hi"));
            }
            
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.DeleteElementDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.DeleteTextDiff>(),
                Is.TypeOf<StepDiff.InsertElementDiff>(),
                Is.TypeOf<StepDiff.InsertTextDiff>()
            ]);
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.RightOrigin.Id, 0);
        }
    }
}