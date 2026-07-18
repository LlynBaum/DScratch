using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Events;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers;

[TestFixture]
public class InsertTextHandlerTests
{
    private TreeBuilder builder;
    private InsertTextHandler handler;
    private TestNodeIdGenerator idGenerator;
    private UserStateService userStateService;

    [SetUp]
    public void SetUp()
    {
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        userStateService = new UserStateService();
        
        var document = builder.CreateDocument();
        var service = new DScratchService(document, new DNodeFactory(idGenerator), userStateService) { DisableCleanUp = true };
        
        handler = new InsertTextHandler(service);
    }

    private class SimpleInsert : InsertTextHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                textNode = t.Text("x");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.FirstChild, Is.TypeOf<TextNode>());
                Assert.That(parent.LastChild, Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("x"));
                Assert.That(((TextNode)parent.LastChild!).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.LastChild.Id, 3);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_NotContinues()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                textNode = t.Text("x");
            });
            idGenerator.TakeIds(1);
            
            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.LastChild, Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.LastChild!).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.LastChild.Id, 3);
        }

        [Test]
        public void Handle_CreatesExpectedChanges_WithInsertingAtStart()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                t.Text("a");
                textNode = t.Text("b");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }

        [Test]
        public void Handle_CreatesExpectedChanges_WithInsertingAtEnd()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                t.Text("a");
                textNode = t.Text("x");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("x"));
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[2].Id, 3);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WithInsertingAtEnd_NotContinues()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                t.Text("a");
                textNode = t.Text("x");
            });
            idGenerator.TakeIds(1);

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[2].Id, 3);
        }

        [Test]
        public void Handle_CreatesExpectedChanges_WithInsertingBetween()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                textNode = t.Text("a");
                t.Text("a");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WithInsertingBetweenOfNode()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                textNode = t.Text("ab");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));

                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
                
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("abc"));
                
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("b"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }
    }

    private class SelectionInsert : InsertTextHandlerTests
    {
        [Test]
        [TestCase(SelectionDirection.Forward)]
        [TestCase(SelectionDirection.Backward)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected(SelectionDirection direction)
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                startNode = t.Text("abc");
                endNode = t.Text("def");
                t.Text("ghi");
            });
            
            var startNodeId = direction is SelectionDirection.Forward ? startNode.Id : endNode.Id;
            var endNodeId = direction is SelectionDirection.Forward ? endNode.Id : startNode.Id;

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(startNodeId, 2, endNodeId, 2, direction);

            // Act
            var result = handler.Handle(keyPressInfo);
            
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

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }

        [Test]
        [TestCase(0, 3)]
        [TestCase(3, 0)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_AtStart(int start, int end)
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                textNode = t.Text("abc");
                t.Text("def");
                t.Text("ghi");
            });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, start, end));

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

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }

        [Test]
        [TestCase(2, 4)]
        [TestCase(2, 4)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_InBetween(int start, int end)
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestBlockElementNode(t => { textNode = t.Text("abcdef"); });

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, start, end));

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

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }

        [Test]
        [TestCase(SelectionDirection.Forward)]
        [TestCase(SelectionDirection.Backward)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_AtEnd(SelectionDirection direction)
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.TestBlockElementNode(t =>
            {
                t.Text("abc");
                startNode = t.Text("def");
                endNode = t.Text("ghi");
            });

            var startNodeId = direction is SelectionDirection.Forward ? startNode.Id : endNode.Id;
            var endNodeId = direction is SelectionDirection.Forward ? endNode.Id : startNode.Id;

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(startNodeId, 3, endNodeId, 3, direction);
            
            // Act
            var result = handler.Handle(keyPressInfo);

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

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[2].Id, 3);
        }
    }
    
    private class MergeParagraphs : InsertTextHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverTwoParagraphs()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => { startNode = t.Text("abc"); });
            var parent2 = builder.Paragraph(t => { endNode = t.Text("def"); });

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
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));

                Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("ef"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverTwoParagraphs_Backwards()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => { startNode = t.Text("abc"); });
            var parent2 = builder.Paragraph(t => { endNode = t.Text("def"); });

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(
                anchorId: endNode.Id, 
                anchorOffset: 1, 
                focusId: startNode.Id,
                focusOffset: 2, 
                direction: SelectionDirection.Backward);
            
            // Act
            var result = handler.Handle(keyPressInfo);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));

                Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("ef"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }

        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverThreeParagraphs()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => { startNode = t.Text("abc"); });
            var parent2 = builder.Paragraph(t => { t.Text("def"); });
            var parent3 = builder.Paragraph(t => { endNode = t.Text("ghi"); });
            
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
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));

                Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("hi"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelectedOverThreeParagraphs_Backwards()
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.Paragraph(t => { endNode = t.Text("abc"); });
            var parent2 = builder.Paragraph(t => { t.Text("def"); });
            var parent3 = builder.Paragraph(t => { startNode = t.Text("ghi"); });

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
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("xyz"));

                Assert.That(parent.ChildNodes[3], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("hi"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.ChildNodes[1].Id, 3);
        }
    }

    private class EmptyBlocks : InsertTextHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges()
        {
            // Arrange
            var parent = builder.TestBlockElementNode();

            // Act
            var result = handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 0));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.FirstChild, Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, parent.FirstChild.Id, 3);
        }
    }

    private class Marks : InsertTextHandlerTests
    {
        [Test]
        public void CopyMarks_FromOrigin()
        {
            // Arrange
            TextNode node = null!;
            builder.Paragraph(t =>
            {
                node = t.Text("a");
            });

            node.SetMark(new Mark(MarkKey.Bold));

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(node.Id, 1));
            
            // Assert
            Assert.That(node.RightOrigin, Is.TypeOf<TextNode>());
            Assert.That(((TextNode)node.RightOrigin).Marks, Is.EquivalentTo(node.Marks));
        }
        
        [Test]
        public void CopyMarks_FromOrigin_IgnoresRightOrigin()
        {
            // Arrange
            TextNode node = null!;
            TextNode right = null!;
            builder.Paragraph(t =>
            {
                node = t.Text("a");
                right = t.Text("a");
            });

            node.SetMark(new Mark(MarkKey.Bold));
            right.SetMark(new Mark(MarkKey.Italic));

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(node.Id, 1));
            
            // Assert
            Assert.That(node.RightOrigin, Is.TypeOf<TextNode>());
            Assert.That(((TextNode)node.RightOrigin).Marks, Is.EquivalentTo(node.Marks));
        }

        [Test]
        public void NoMarks_WhenInsertAsFirstChild()
        {
            // Arrange
            TextNode node = null!;
            builder.Paragraph(t =>
            {
                node = t.Text("a");
            });

            node.SetMark(new Mark(MarkKey.Bold));

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(node.Id, 0));
            
            // Assert
            Assert.That(node.Origin, Is.TypeOf<TextNode>());
            Assert.That(((TextNode)node.Origin).Marks, Is.Empty);
        }
        
        [Test]
        public void NoMarks_WhenInsertIntoEmptyBlock()
        {
            // Arrange
            var node = builder.Paragraph();

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(node.Id, 0));
            
            // Assert
            Assert.That(node.FirstChild, Is.TypeOf<TextNode>());
            Assert.That(((TextNode)node.FirstChild).Marks, Is.Empty);
        }
    }
}