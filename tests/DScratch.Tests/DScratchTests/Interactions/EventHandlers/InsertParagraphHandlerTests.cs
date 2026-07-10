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
    private TestLayoutEngineFake layoutEngineFake;
    private DScratchDocument document = null!;
    private IDScratchService service;

    private InsertParagraphHandler handler;
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
        handler = new InsertParagraphHandler(service);
        transaction = service.StartTransaction();
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0), transaction);

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));

                Assert.That(parent.Origin, Is.Not.Null);
                Assert.That(parent.Origin!.RightOrigin, Is.EqualTo(parent));
                Assert.That(parent.Origin.ChildNodes, Has.Count.Zero);
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.Id, 0);
        }

        [Test]
        public void CreatesExpectedChanges_WithOffsetAtEnd()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abc"); });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 3), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("abc"));

                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin!.Origin, Is.EqualTo(parent));
                Assert.That(parent.RightOrigin.ChildNodes, Has.Count.Zero);
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin.Id, 0);
        }

        [Test]
        public void CreatesExpectedChanges_WithOffsetInBetweenText()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abc"); });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));

                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin!.Origin, Is.EqualTo(parent));
                Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin.FirstChild!).TextContent, Is.EqualTo("bc"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin.Id, 0);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));

                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin!.Origin, Is.EqualTo(parent));
                Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(((TextNode)parent.RightOrigin.ChildNodes[0]).TextContent, Is.EqualTo("b"));
                Assert.That(((TextNode)parent.RightOrigin.ChildNodes[1]).TextContent, Is.EqualTo("c"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin.Id, 0);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode1.Id, 1), transaction);
            var transactionResult1 = layoutEngineFake.TransactionResult;
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode2.Id, 1), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.FirstChild!).TextContent, Is.EqualTo("a"));

                Assert.That(parent.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin!.Origin, Is.EqualTo(parent));

                Assert.That(parent.RightOrigin.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin.ChildNodes[0]).TextContent, Is.EqualTo("b"));

                Assert.That(parent.RightOrigin.RightOrigin, Is.Not.Null);
                Assert.That(parent.RightOrigin.RightOrigin!.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.RightOrigin.RightOrigin.ChildNodes[0]).TextContent, Is.EqualTo("c"));
            }

            AssertHelper.ThatCursorPositionEqualTo(transactionResult1?.CursorPosition, parent.RightOrigin.Id, 0);
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin.RightOrigin.Id, 0);
        }

        [Test]
        public void CreatesExpectedChanges_WithSelection()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.Paragraph(t => { textNode = t.Text("abcde"); });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, 1, 4), transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));

                var newParagraph = parent.RightOrigin;
                Assert.That(newParagraph, Is.Not.Null);
                Assert.That(newParagraph!.Origin, Is.EqualTo(parent));
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(newParagraph.ChildNodes[0].IsDeleted, Is.True);
                Assert.That(((TextNode)newParagraph.ChildNodes[1]).TextContent, Is.EqualTo("e"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin!.Id, 0);
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
            handler.Handle(keyPressInfo, transaction);
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));

                var newParagraph = parent.RightOrigin;
                Assert.That(newParagraph, Is.Not.Null);
                Assert.That(newParagraph!.Origin, Is.EqualTo(parent));
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(newParagraph.ChildNodes[0].IsDeleted, Is.True);
                Assert.That(((TextNode)newParagraph.ChildNodes[1]).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin!.Id, 0);
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
            handler.Handle(keyPressInfo, transaction);
            
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
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                var newParagraph = parent.RightOrigin;
                Assert.That(newParagraph, Is.Not.Null);
                Assert.That(newParagraph!.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(newParagraph.ChildNodes[0].IsDeleted, Is.True);
                Assert.That(((TextNode)newParagraph.ChildNodes[1]).TextContent, Is.EqualTo("ef"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin!.Id, 0);
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
            handler.Handle(keyPressInfo, transaction);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent2.IsDeleted, Is.True);
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(parent3.IsDeleted, Is.True);
                Assert.That(parent3.ChildNodes, Has.Count.EqualTo(1));
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                var newParagraph = parent.RightOrigin;
                Assert.That(newParagraph, Is.Not.Null);
                Assert.That(newParagraph!.Origin, Is.EqualTo(parent));
                Assert.That(newParagraph.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(newParagraph.ChildNodes[0].IsDeleted, Is.True);
                Assert.That(((TextNode)newParagraph.ChildNodes[1]).TextContent, Is.EqualTo("hi"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin!.Id, 0);
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
            handler.Handle(keyPressInfo, transaction);
            
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
                
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));
                
                var newParagraph = parent.RightOrigin;
                Assert.That(newParagraph, Is.Not.Null);
                Assert.That(newParagraph!.ChildNodes, Has.Count.EqualTo(2));
                Assert.That(newParagraph.ChildNodes[0].IsDeleted, Is.True);
                Assert.That(((TextNode)newParagraph.ChildNodes[1]).TextContent, Is.EqualTo("hi"));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin!.Id, 0);
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
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(parent.Id, 0), transaction);

            // Assert
            Assert.That(parent.RightOrigin, Is.TypeOf<ParagraphNode>());
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.RightOrigin.Id, 0);
        }
    }
}