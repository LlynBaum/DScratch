using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Events;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers;

[TestFixture]
public class InsertTextHandlerTests
{
    private TestLayoutEngineFake layoutEngineFake;
    private DScratchDocument document = null!;
    private IDScratchService service;

    private InsertTextHandler handler;
    private TestNodeIdGenerator idGenerator;

    private TreeBuilder builder;
    private ITransaction transaction;

    [SetUp]
    public void SetUp()
    {
        layoutEngineFake = new TestLayoutEngineFake();
        idGenerator = new TestNodeIdGenerator();
        builder = new TreeBuilder(idGenerator);
        document = builder.CreateDocument();
        
        service = new DScratchService(document, new DNodeFactory(idGenerator), idGenerator, layoutEngineFake) { DisableCleanUp = true };
        handler = new InsertTextHandler(service);
        transaction = service.StartTransaction();
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.FirstChild, Is.TypeOf<TextNode>());
                Assert.That(parent.LastChild, Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("x"));
                Assert.That(((TextNode)parent.LastChild!).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.LastChild.Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(parent.LastChild, Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.LastChild!).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.LastChild.Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("x"));
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[2].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[2].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(keyPressInfo, transaction);
            
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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, start, end), transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, start, end), transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(keyPressInfo, transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[2].Id, 3);
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
            handler.Handle(keyPressInfo, transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(keyPressInfo, transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(keyPressInfo, transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(keyPressInfo, transaction);

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

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.ChildNodes[1].Id, 3);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 0), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent.FirstChild, Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.FirstChild.Id, 3);
        }
    }
}