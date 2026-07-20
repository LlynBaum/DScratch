using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

[TestFixture]
public class UpdateMarkHandlerTest
{
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private UpdateMarkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: new UserStateService()) { DisableCleanUp = true };
        
        handler = new UpdateMarkHandler(dScratchService, new UserStateService());
    }

    private class ToggleAction : UpdateMarkHandlerTest
    {
        [Test]
        public void DoesNothing_WithSelectionDirectionNone()
        {
            // Arrange
            TextNode start = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
            });

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(start.Id, 2);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(start.Marks, Has.Count.Zero);
                Assert.That(result.CursorPosition, Is.Null);
            }
        }
        
        [Test]
        public void RemovesMarks_FromSelection_WhenAnchorHasGivenMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            start.SetMark(new Mark(MarkKey.Bold));
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(start.TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)start.RightOrigin!).TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(start.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)end.Origin!).Marks, Is.Empty);
                Assert.That(end.Marks, Is.Empty);
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.Empty);
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = start.RightOrigin.Id.Value,
                AnchorOffset = 0,
                FocusId = end.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddMarks_FromSelection_WhenAnchorDoesNotHaveGivenMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(start.TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)start.RightOrigin!).TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(start.Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(end.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.Empty);
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = start.RightOrigin.Id.Value,
                AnchorOffset = 0,
                FocusId = end.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddsMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold), new Mark(MarkKey.Italic)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node2.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddsMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks_Backwards()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic), new Mark(MarkKey.Bold)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node3.Id.Value,
                AnchorOffset = 1,
                FocusId = node2.Id.Value,
                FocusOffset = 0,
                Direction = SelectionDirection.Backward
            });
        }
        
        [Test]
        public void RemovesMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.Empty);
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node2.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void RemovesMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks_Backwards()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Toggle));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node3.Marks, Is.Empty);
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node3.Id.Value,
                AnchorOffset = 1,
                FocusId = node2.Id.Value,
                FocusOffset = 0,
                Direction = SelectionDirection.Backward
            });
        }
        
        [Test]
        public void AddsMark_ToSelection_WhenSelectingOverNodesWithMiddleHasMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode mid = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                mid = t.Text("cd");
                end = t.Text("ef");
            });

            mid.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 0, end.Id, 2);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Toggle));
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("ab"));
                Assert.That(node2.TextContent, Is.EqualTo("cd"));
                Assert.That(node3.TextContent, Is.EqualTo("ef"));

                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold), new Mark(MarkKey.Italic)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node1.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 2
            });
        }
    }

    private class AddAction : UpdateMarkHandlerTest
    {
        [Test]
        public void DoesNothing_WithSelectionDirectionNone()
        {
            // Arrange
            TextNode start = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
            });

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(start.Id, 2);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Add));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(start.Marks, Has.Count.Zero);
                Assert.That(result.CursorPosition, Is.Null);
            }
        }
        
        [Test]
        public void AddsMarks_FromSelection_AnchorHasMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            start.SetMark(new Mark(MarkKey.Bold));
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Add));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(start.TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)start.RightOrigin!).TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(start.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(end.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.Empty);
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = start.RightOrigin.Id.Value,
                AnchorOffset = 0,
                FocusId = end.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddsMarks_FromSelection_AnchorHasNoMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Add));
        
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(start.TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)start.RightOrigin!).TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(start.Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(end.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.Empty);
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = start.RightOrigin.Id.Value,
                AnchorOffset = 0,
                FocusId = end.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddsMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Add));
         
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold), new Mark(MarkKey.Italic)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node2.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddsMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks_Backwards()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Add));
            
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic), new Mark(MarkKey.Bold)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node3.Id.Value,
                AnchorOffset = 1,
                FocusId = node2.Id.Value,
                FocusOffset = 0,
                Direction = SelectionDirection.Backward
            });
        }
        
        [Test]
        public void AddsMark_ToSelectionStartingWithSameMark_WhenSelectingOverNodesWithDifferentMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Add));
  
            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic), new Mark(MarkKey.Bold)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node2.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void AddsMark_ToSelectionStartingWithSameMark_WhenSelectingOverNodesWithDifferentMarks_Backwards()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Add));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold), new Mark(MarkKey.Italic)]));
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node3.Id.Value,
                AnchorOffset = 1,
                FocusId = node2.Id.Value,
                FocusOffset = 0,
                Direction = SelectionDirection.Backward
            });
        }
    }
    
    private class RemoveAction : UpdateMarkHandlerTest
    {
        [Test]
        public void DoesNothing_WithSelectionDirectionNone()
        {
            // Arrange
            TextNode start = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
            });
            
            start.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(start.Id, 2);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(start.Marks, Has.Count.EqualTo(1));
                Assert.That(result.CursorPosition, Is.Null);
            }
        }
        
        [Test]
        public void RemovesMarks_FromSelection_AnchorHasMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            start.SetMark(new Mark(MarkKey.Bold));
            ((TextNode)start.RightOrigin!).SetMark(new Mark(MarkKey.Bold));
            markedNode.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(start.TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)start.RightOrigin).TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(start.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)end.Origin!).Marks, Is.Empty);
                Assert.That(end.Marks, Is.Empty);
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = start.RightOrigin.Id.Value,
                AnchorOffset = 0,
                FocusId = end.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void RemovesMarks_FromSelection_AnchorHasNoMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });

            ((TextNode)start.RightOrigin!).SetMark(new Mark(MarkKey.Bold));
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(start.TextContent, Is.EqualTo("ab"));
                Assert.That(((TextNode)start.RightOrigin).TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(start.Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)end.Origin!).Marks, Is.Empty);
                Assert.That(end.Marks, Is.Empty);
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.Empty);
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = start.RightOrigin.Id.Value,
                AnchorOffset = 0,
                FocusId = end.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void RemovesMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node3.Marks, Is.Empty);
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node2.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void RemovesMark_ToSelection_WhenSelectingOverNodesWithDifferentMarks_Backwards()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.Empty);
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node3.Id.Value,
                AnchorOffset = 1,
                FocusId = node2.Id.Value,
                FocusOffset = 0,
                Direction = SelectionDirection.Backward
            });
        }
        
        [Test]
        public void RemovesMark_ToSelectionStartingWithSameMark_WhenSelectingOverNodesWithDifferentMarks()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Bold), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.Empty);
                Assert.That(node3.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node2.Id.Value,
                AnchorOffset = 0,
                FocusId = node3.Id.Value,
                FocusOffset = 1
            });
        }
        
        [Test]
        public void RemovesMark_ToSelectionStartingWithSameMark_WhenSelectingOverNodesWithDifferentMarks_Backwards()
        {
            // Arrange
            TextNode start = null!;
            TextNode end = null!;
            var parent = builder.Paragraph(t =>
            {
                start = t.Text("ab");
                end = t.Text("cd");
            });

            start.SetMark(new Mark(MarkKey.Bold));
            end.SetMark(new Mark(MarkKey.Italic));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, new UpdateMarkCommand(new Mark(MarkKey.Italic), UpdateMarkAction.Remove));

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
            }

            var node1 = (TextNode)parent.ChildAt(0)!;
            var node2 = (TextNode)parent.ChildAt(1)!;
            var node3 = (TextNode)parent.ChildAt(2)!;
            var node4 = (TextNode)parent.ChildAt(3)!;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(node1.TextContent, Is.EqualTo("a"));
                Assert.That(node2.TextContent, Is.EqualTo("b"));
                Assert.That(node3.TextContent, Is.EqualTo("c"));
                Assert.That(node4.TextContent, Is.EqualTo("d"));
                
                Assert.That(node1.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node2.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(node3.Marks, Is.Empty);
                Assert.That(node4.Marks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
            }

            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, new SelectionInfo
            {
                AnchorId = node3.Id.Value,
                AnchorOffset = 1,
                FocusId = node2.Id.Value,
                FocusOffset = 0,
                Direction = SelectionDirection.Backward
            });
        }
    }
}