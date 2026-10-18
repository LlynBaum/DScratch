using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Events;
using DScratch.Interactions.UserStates;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers;

[TestFixture]
public class InsertParagraphHandlerTests
{
    private IDScratchService service;

    private InsertParagraphHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;

    [SetUp]
    public void SetUp()
    {
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        service = new DScratchService(builder.Document, new DNodeFactory(idGenerator), new UserStateService()) { DisableCleanUp = true };
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(builder.Root.ChildNodes[1], Is.EqualTo(parent));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));

                Assert.That(builder.Root.ChildNodes[1].Origin, Is.Null);
                Assert.That(builder.Root.ChildNodes[0].RightOrigin, Is.EqualTo(parent.Id));
                Assert.That(builder.Root.ChildNodes[0].ChildNodes, Has.Count.Zero);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
                Assert.That(parent.RightOrigin, Is.Null);
                
                Assert.That(builder.Root.ChildNodes[1].Origin, Is.EqualTo(parent.Id));
                Assert.That(builder.Root.ChildNodes[1].RightOrigin, Is.Null);
                Assert.That(builder.Root.ChildNodes[1].ChildNodes, Has.Count.Zero);
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, builder.Root.ChildNodes[1].Id, 0);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            
            var newParagraph = builder.Root.ChildNodes[1];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
                Assert.That(parent.RightOrigin, Is.Null);

                Assert.That(newParagraph.Origin, Is.EqualTo(parent.Id));
                Assert.That(newParagraph.RightOrigin, Is.Null);
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)newParagraph.FirstChild!).TextContent, Is.EqualTo("bc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, newParagraph.Id, 0);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            
            var newParagraph = builder.Root.ChildNodes[1];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
                Assert.That(parent.RightOrigin, Is.Null);

                Assert.That(newParagraph.Origin, Is.EqualTo(parent.Id));
                Assert.That(newParagraph.RightOrigin, Is.Null);
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(((TextNode)newParagraph.ChildNodes[0]).TextContent, Is.EqualTo("b"));
                Assert.That(((TextNode)newParagraph.ChildNodes[1]).TextContent, Is.EqualTo("c"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, newParagraph.Id, 0);
        }

        [Test]
        public void CopiesComputedMarksFromFirstCharacter_WhenInsertingParagraphAtStart()
        {
            // Arrange
            TextNode textNode = null!;
            TextNode textNode2 = null!;
            var parent = builder.Paragraph(t =>
            {
                textNode = t.Text("a");
                textNode2 = t.Text("a");
            });
            
            textNode.SetMark(MarkKey.Color, "#fff");
            textNode2.SetMark(MarkKey.Color, "#000");
            parent.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0));
            
            // Assert
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[1], Is.EqualTo(parent));
            
            Assert.That(builder.Root.ChildNodes[0], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[0].Marks, Is.EquivalentTo(new Dictionary<MarkKey, string>
            {
                { MarkKey.Color, "#fff" },
                { MarkKey.FontWeight, "bold" }
            }));
        }
        
        [Test]
        public void CopiesComputedMarksFromLastCharacter_WhenInsertingParagraphAtEnd()
        {
            // Arrange
            TextNode textNode = null!;
            TextNode textNode2 = null!;
            var parent = builder.Paragraph(t =>
            {
                textNode = t.Text("a");
                textNode2 = t.Text("a");
            });
            
            textNode.SetMark(MarkKey.Color, "#fff");
            textNode2.SetMark(MarkKey.Color, "#000");
            parent.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode2.Id, 1));
            
            // Assert
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[1], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[1].Marks, Is.EquivalentTo(new Dictionary<MarkKey, string>
            {
                { MarkKey.Color, "#000" },
                { MarkKey.FontWeight, "bold" }
            }));
        }
        
        [Test]
        public void CopiesComputedMarksFromParagraph_WhenInsertingParagraphInTheMiddle()
        {
            // Arrange
            TextNode textNode = null!;
            TextNode textNode2 = null!;
            var parent = builder.Paragraph(t =>
            {
                textNode = t.Text("a");
                textNode2 = t.Text("a");
            });
            
            textNode.SetMark(MarkKey.Color, "#fff");
            textNode2.SetMark(MarkKey.Color, "#000");
            parent.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode2.Id, 0));
            
            // Assert
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[1], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[1].Marks, Is.EquivalentTo(new Dictionary<MarkKey, string>
            {
                { MarkKey.FontWeight, "bold" },
                { MarkKey.Color, "#000" }
            }));
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Null);

                var parent2 = builder.Root.ChildNodes[1];
                Assert.That(parent2.Origin, Is.EqualTo(parent.Id));
                Assert.That(parent2.RightOrigin, Is.Null);

                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent2.ChildNodes[0]).TextContent, Is.EqualTo("b"));

                var parent3 = builder.Root.ChildNodes[2];
                Assert.That(parent3.Origin, Is.EqualTo(parent2.Id));
                Assert.That(parent3.RightOrigin, Is.Null);
                
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent3.ChildNodes[0]).TextContent, Is.EqualTo("c"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result1.CursorPosition, builder.Root.ChildNodes[1].Id, 0);
            AssertHelper.ThatCursorPositionEqualTo(result2.CursorPosition, builder.Root.ChildNodes[2].Id, 0);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("bcd"));

                var newParagraph = builder.Root.ChildNodes[1];
                Assert.That(newParagraph.Origin, Is.EqualTo(parent.Id));
                Assert.That(newParagraph.RightOrigin, Is.Null);
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(newParagraph.ChildNodes[0].IsDeleted, Is.False);
                Assert.That(((TextNode)newParagraph.ChildNodes[0]).TextContent, Is.EqualTo("e"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, builder.Root.ChildNodes[1].Id, 0);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[2], Is.EqualTo(parent2));
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Null);
                Assert.That(builder.Root.ChildNodes[1].Origin, Is.EqualTo(parent.LastId));
                Assert.That(builder.Root.ChildNodes[1].RightOrigin, Is.EqualTo(parent2.Id));
                Assert.That(parent2.Origin, Is.EqualTo(parent.LastId));
                Assert.That(parent2.RightOrigin, Is.Null);
            }
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));

                var newParagraph = builder.Root.ChildNodes[1];
                Assert.That(newParagraph.Origin, Is.EqualTo(parent.Id));
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)newParagraph.ChildNodes[0]).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, builder.Root.ChildNodes[1].Id, 0);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[2], Is.EqualTo(parent2));
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Null);
                Assert.That(builder.Root.ChildNodes[1].Origin, Is.EqualTo(parent.LastId));
                Assert.That(builder.Root.ChildNodes[1].RightOrigin, Is.EqualTo(parent2.Id));
                Assert.That(parent2.Origin, Is.EqualTo(parent.LastId));
                Assert.That(parent2.RightOrigin, Is.Null);
            }
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));

                var newParagraph = builder.Root.ChildNodes[1];
                Assert.That(newParagraph.Origin, Is.EqualTo(parent.Id));
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)newParagraph.ChildNodes[0]).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, builder.Root.ChildNodes[1].Id, 0);
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
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(4));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[2], Is.EqualTo(parent2));
            Assert.That(builder.Root.ChildNodes[3], Is.EqualTo(parent3));

            var newParent = builder.Root.ChildNodes[1];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.IsDeleted, Is.False);
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Null);
                
                Assert.That(newParent.IsDeleted, Is.False);
                Assert.That(newParent.Origin, Is.EqualTo(parent.LastId));
                Assert.That(newParent.RightOrigin, Is.EqualTo(parent2.Id));
                
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.Origin, Is.EqualTo(parent.LastId));
                Assert.That(parent2.RightOrigin, Is.Null);
                
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.Origin, Is.EqualTo(parent2.LastId));
                Assert.That(parent3.RightOrigin, Is.Null);
            }
            
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(newParent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
                
                Assert.That(((TextNode)parent2.ChildNodes[0]).TextContent, Is.EqualTo("def"));
                
                Assert.That(((TextNode)newParent.ChildNodes[0]).TextContent, Is.EqualTo("hi"));
                
                Assert.That(((TextNode)parent3.ChildNodes[0]).TextContent, Is.EqualTo("g"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, newParent.Id, 0);
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
            
            // Assert
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(4));
            Assert.That(builder.Root.ChildNodes[0], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[2], Is.EqualTo(parent2));
            Assert.That(builder.Root.ChildNodes[3], Is.EqualTo(parent3));

            var newParent = builder.Root.ChildNodes[1];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.IsDeleted, Is.False);
                Assert.That(parent.Origin, Is.Null);
                Assert.That(parent.RightOrigin, Is.Null);
                
                Assert.That(newParent.IsDeleted, Is.False);
                Assert.That(newParent.Origin, Is.EqualTo(parent.LastId));
                Assert.That(newParent.RightOrigin, Is.EqualTo(parent2.Id));
                
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.Origin, Is.EqualTo(parent.LastId));
                Assert.That(parent2.RightOrigin, Is.Null);
                
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.Origin, Is.EqualTo(parent2.LastId));
                Assert.That(parent3.RightOrigin, Is.Null);
            }
            
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(newParent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
                
                Assert.That(((TextNode)parent2.ChildNodes[0]).TextContent, Is.EqualTo("def"));
                
                Assert.That(((TextNode)newParent.ChildNodes[0]).TextContent, Is.EqualTo("hi"));
                
                Assert.That(((TextNode)parent3.ChildNodes[0]).TextContent, Is.EqualTo("g"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, newParent.Id, 0);
        }
    }
    
    private class EmptyBlocks : InsertParagraphHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges()
        {
            // Arrange
            var parent = builder.TestBlockElementNode();

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 0));

            // Assert
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[1], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[0], Is.TypeOf<ParagraphNode>());
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }

        [Test]
        public void Handle_CopiesMarks_FromBlock()
        {
            var parent = builder.TestBlockElementNode();
            parent.SetMark(MarkKey.Color, "#fff");

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 0));

            // Assert
            Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(builder.Root.ChildNodes[1], Is.EqualTo(parent));
            Assert.That(builder.Root.ChildNodes[0], Is.TypeOf<ParagraphNode>());
            Assert.That(builder.Root.ChildNodes[0].Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> {{ MarkKey.Color, "#fff" }}));
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.Id, 0);
        }
    }
}