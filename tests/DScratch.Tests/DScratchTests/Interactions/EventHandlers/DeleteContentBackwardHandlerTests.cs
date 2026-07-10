using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Events;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers;

[TestFixture]
public class DeleteContentBackwardHandlerTests
{
    private TestLayoutEngineFake layoutEngineFake;
    private DScratchDocument document = null!;
    private IDScratchService service;

    private DeleteContentBackwardHandler handler;
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
        handler = new DeleteContentBackwardHandler(service);
        transaction = service.StartTransaction();
    }
    
    private class SimpleDelete : DeleteContentBackwardHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges()
        {
            // Arrange
            TextNode textNode = null!;
            builder.TestInlineElementNode(t => 
            {
                t.Text("ab");
                textNode = t.Text("a");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);
            transaction.Commit();

            // Assert
            Assert.That(textNode.IsDeleted, Is.True);
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, textNode.Origin!.Id, 2);
        }

        [Test]
        public void Handle_CreatesExpectedChanges_WithDeletedText()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestInlineElementNode(t =>
            {
                textNode = t.Text("abc");
                t.Text("d").Delete();
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 3), transaction);
            transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(textNode.IsDeleted, Is.False);
                Assert.That(textNode.TextContent, Is.EqualTo("ab"));
                
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
                
                Assert.That(parent.ChildNodes[2].IsDeleted, Is.True);
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("d"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, textNode.Id, 2);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_MiddleOfTextNode()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestInlineElementNode(t =>
            {
                textNode = t.Text("abcd");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 3), transaction);
            transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(textNode.IsDeleted, Is.False);
                Assert.That(textNode.TextContent, Is.EqualTo("ab"));
                
                Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("c"));
                
                Assert.That(parent.ChildNodes[2].IsDeleted, Is.False);
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("d"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, textNode.Id, 2);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_LastCharInTextNode()
        {
            // Arrange
            TextNode textNode = null!;
            TextNode textNode2 = null!;
            var parent = builder.TestInlineElementNode(t =>
            {
                textNode = t.Text("ab");
                textNode2 = t.Text("c");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 2), transaction);
            transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(textNode.IsDeleted, Is.False);
                Assert.That(textNode.RightOrigin!.IsDeleted, Is.True);
                Assert.That(textNode2.IsDeleted, Is.False);
            }
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, textNode.Id, 1);
        }
        
        [Test]
        public void Handle_CreatesExpectedChanges_SingleCharInNode()
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestInlineElementNode(t =>
            {
                textNode = t.Text("a");
                t.Text("bc");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 1), transaction);
            transaction.Commit();
            
            // Assert
            Assert.That(textNode.IsDeleted, Is.True);
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.Id, 0);
        }
        
        [Test]
        public void Handle_DoesNothing_WhenWhereIsNoPreviousParagraph()
        {
            // Arrange
            DNode textNode = null!;
            var parent = builder.Paragraph(t => 
            {
                textNode = t.Text("abc");
                t.Text("def");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0), transaction);
            transaction.Commit();
        
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.IsDeleted, Is.False);
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
            
                Assert.That(layoutEngineFake.TransactionResult?.ModifiedNodes, Has.Count.Zero);
                Assert.That(layoutEngineFake.TransactionResult?.CursorPosition, Is.Null);
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
    
    private class SelectionDelete : DeleteContentBackwardHandlerTests
    {
        [Test]
        [TestCase(SelectionDirection.Forward)]
        [TestCase(SelectionDirection.Backward)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected(SelectionDirection direction)
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.TestInlineElementNode(t =>
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
            transaction.Commit();

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

                AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, startNode.Id, 2);
            }
        }

        [Test]
        [TestCase(0, 3)]
        [TestCase(3, 0)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_AtStart(int start, int end)
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestInlineElementNode(t =>
            {
                textNode = t.Text("abc");
                t.Text("def");
                t.Text("ghi");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, start, end), transaction);
            transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));

                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ghi"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, textNode.Id, 0);
        }

        [Test]
        [TestCase(2, 4)]
        [TestCase(4, 2)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_InBetween(int start, int end)
        {
            // Arrange
            TextNode textNode = null!;
            var parent = builder.TestInlineElementNode(t => { textNode = t.Text("abcdef"); });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfo(textNode.Id, start, end), transaction);
            transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));

                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("ab"));

                Assert.That(parent.ChildNodes[2], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("ef"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, textNode.Id, 2);
        }

        [Test]
        [TestCase(SelectionDirection.Forward)]
        [TestCase(SelectionDirection.Backward)]
        public void Handle_CreatesExpectedChanges_WhenTextIsSelected_AtEnd(SelectionDirection direction)
        {
            // Arrange
            TextNode startNode = null!;
            TextNode endNode = null!;
            var parent = builder.TestInlineElementNode(t =>
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
            transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));

                Assert.That(parent.ChildNodes[0], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("abc"));

                Assert.That(parent.ChildNodes[1], Is.TypeOf<TextNode>());
                Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("def"));
            }

            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, startNode.Id, 3);
        }
    }
    
    private class MergeParagraphs : DeleteContentBackwardHandlerTests
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
            TextNode oldTextNode = null!;
            var oldParent = builder.Paragraph(t =>
            {
                oldTextNode = t.Text("ghi");
            });

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(oldTextNode.Id, 0), transaction);
            transaction.Commit();
        
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
        
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, oldTextNode.Id, 0);
        }
        
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
            transaction.Commit();

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
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, startNode.Id, 2);
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
            transaction.Commit();

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
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, endNode.Id, 2);
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
            transaction.Commit();
            
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
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, startNode.Id, 2);
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
            transaction.Commit();
            
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
            
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, endNode.Id, 2);
        }
    }
    
    private class EmptyBlocks : DeleteContentBackwardHandlerTests
    {
        [Test]
        public void Handle_CreatesExpectedChanges()
        {
            // Arrange
            var parent = builder.TestBlockElementNode(t => t.Text("abc"));
            var target = builder.TestBlockElementNode();

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(target.Id, 0), transaction);
            transaction.Commit();

            // Assert
            Assert.That(target.IsDeleted, Is.True);
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.FirstChild!.Id, 3);
        }
        
        [Test]
        public void Handle_FocusToOrigin_WhenNoTextNodeIsPresent()
        {
            // Arrange
            var parent = builder.TestBlockElementNode();
            var target = builder.TestBlockElementNode();

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(target.Id, 0), transaction);
            transaction.Commit();

            // Assert
            Assert.That(target.IsDeleted, Is.True);
            AssertHelper.ThatCursorPositionEqualTo(layoutEngineFake.TransactionResult?.CursorPosition, parent.Id, 0);
        }
        
        [Test]
        public void Handle_DoesNothing_WhenNoOriginIsPresent()
        {
            // Arrange
            var target = builder.TestBlockElementNode();

            // Act
            handler.Handle(KeyPressInfoHelper.GetKeyPressInfoDirectionNone(target.Id, 0), transaction);
            transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(target.IsDeleted, Is.False);
                Assert.That(layoutEngineFake.TransactionResult?.IsEmpty, Is.True);
            }
        }
    }
}