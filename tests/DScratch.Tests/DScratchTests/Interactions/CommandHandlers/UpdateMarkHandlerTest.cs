using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

[TestFixture]
public class UpdateMarkHandlerTest
{
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private UserStateServiceFake userStateServiceFake;
    private UpdateMarkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        userStateServiceFake = new UserStateServiceFake();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: userStateServiceFake)
        {
            DisableCleanUp = true
        };
        
        handler = new UpdateMarkHandler(dScratchService, userStateServiceFake);
    }

    private class ToggleAction : UpdateMarkHandlerTest
    {
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
            
            start.SetMark(MarkKey.FontWeight, "bold");
            markedNode.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));

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
                
                Assert.That(start.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
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
            
            markedNode.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));

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
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(end.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontStyle, "italic"));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" }, { MarkKey.FontStyle, "italic" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" }, { MarkKey.FontWeight, "bold" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.Empty);
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontStyle, "italic"));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node3.Marks, Is.Empty);
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            mid.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 0, end.Id, 2);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Toggle(MarkKey.FontStyle, "italic"));
            
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

                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" }, { MarkKey.FontStyle, "italic" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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
            
            start.SetMark(MarkKey.FontWeight, "bold");
            markedNode.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));

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
                
                Assert.That(start.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(end.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
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
            
            markedNode.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));
        
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
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(end.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Add(MarkKey.FontStyle, "italic"));
         
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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" }, { MarkKey.FontStyle, "italic" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));
            
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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" }, { MarkKey.FontWeight, "bold" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));
  
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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" }, { MarkKey.FontWeight, "bold" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Add(MarkKey.FontStyle, "italic"));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" }, { MarkKey.FontStyle, "italic" } }));
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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
            
            start.SetMark(MarkKey.FontWeight, "bold");
            ((TextNode)start.RightOrigin!).SetMark(MarkKey.FontWeight, "bold");
            markedNode.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Remove(MarkKey.FontWeight));

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
                
                Assert.That(start.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)end.Origin!).Marks, Is.Empty);
                Assert.That(end.Marks, Is.Empty);
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
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

            ((TextNode)start.RightOrigin!).SetMark(MarkKey.FontWeight, "bold");
            markedNode.SetMark(MarkKey.FontWeight, "bold");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Remove(MarkKey.FontWeight));

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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Remove(MarkKey.FontStyle));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node3.Marks, Is.Empty);
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Remove(MarkKey.FontWeight));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.Empty);
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Remove(MarkKey.FontWeight));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.Empty);
                Assert.That(node3.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

            start.SetMark(MarkKey.FontWeight, "bold");
            end.SetMark(MarkKey.FontStyle, "italic");

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);

            // Act
            var result = handler.Execute(keyPressInfo.Selection, UpdateMarkCommand.Remove(MarkKey.FontStyle));

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
                
                Assert.That(node1.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node2.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
                Assert.That(node3.Marks, Is.Empty);
                Assert.That(node4.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
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

    private class SelectionDirectionNone : UpdateMarkHandlerTest
    {
        [Test]
        public void ActionAdd_AtBlockElement_SetsMarkToBlock()
        {
            // Arrange
            var blockNode = builder.Paragraph();
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(blockNode.Id, 0).Selection,
                UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(blockNode.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
            Assert.That(result.CursorPosition, Is.Null);

            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
        
        [Test]
        public void ActionRemove_AtBlockElement_RemovesMarkFromBlock()
        {
            // Arrange
            var blockNode = builder.Paragraph();
            blockNode.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(blockNode.Id, 0).Selection,
                UpdateMarkCommand.Remove(MarkKey.FontWeight));
            
            // Assert
            Assert.That(blockNode.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
        
        [Test]
        public void ActionToggleAdd_AtBlockElement_SetsMarkToBlock()
        {
            // Arrange
            var blockNode = builder.Paragraph();
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(blockNode.Id, 0).Selection,
                UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(blockNode.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
        
        [Test]
        public void ActionToggleRemove_AtBlockElement_RemovesMarkFromBlock()
        {
            // Arrange
            var blockNode = builder.Paragraph();
            blockNode.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(blockNode.Id, 0).Selection,
                UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(blockNode.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }

        
        [Test]
        public void ActionAdd_AtTextNode_AddsMarkAsPending()
        {
            // Arrange
            TextNode textNode = null!;
            builder.Paragraph(t =>
            {
                textNode = t.Text("a");
            });
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0).Selection,
                UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(userStateServiceFake.AddedMarks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
            Assert.That(textNode.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            Assert.That(result.Steps, Is.Empty);
        }
        
        
        [Test]
        public void ActionRemove_AtTextNode_AddsMarkAsPending()
        {
            // Arrange
            TextNode textNode = null!;
            builder.Paragraph(t =>
            {
                textNode = t.Text("a");
            });
            userStateServiceFake.AddedMarks.Add(MarkKey.FontWeight, "bold");
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0).Selection,
                UpdateMarkCommand.Remove(MarkKey.FontWeight));
            
            // Assert
            Assert.That(userStateServiceFake.RemovedMarks, Is.EquivalentTo([MarkKey.FontWeight]));
            Assert.That(textNode.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            Assert.That(result.Steps, Is.Empty);
        }
        
        [Test]
        public void ActionToggleAdd_AtTextNode_AddsMarkAsPending()
        {
            // Arrange
            TextNode textNode = null!;
            builder.Paragraph(t =>
            {
                textNode = t.Text("a");
            });
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0).Selection,
                UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(userStateServiceFake.AddedMarks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
            Assert.That(textNode.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            Assert.That(result.Steps, Is.Empty);
        }
        
        [Test]
        public void ActionToggleRemove_AtTextNode_AddsMarkAsPending()
        {
            // Arrange
            TextNode textNode = null!;
            builder.Paragraph(t =>
            {
                textNode = t.Text("a");
            });
            userStateServiceFake.AddedMarks.Add(MarkKey.FontWeight, "bold");
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(textNode.Id, 0).Selection,
                UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(userStateServiceFake.RemovedMarks, Is.EquivalentTo([MarkKey.FontWeight]));
            Assert.That(textNode.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            Assert.That(result.Steps, Is.Empty);
        }
    }

    private class EmptyBlocks : UpdateMarkHandlerTest
    {
        [Test]
        public void ActionToggleAdd_InEmptyBlock_AddsMarkToBlock()
        {
            // Arrange
            var block = builder.Paragraph();
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(block.Id, 0).Selection,
                UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(block.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string>
            {
                { MarkKey.FontWeight, "bold" }
            }));
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
        
        [Test]
        public void ActionToggleRemove_InEmptyBlock_RemovesMarkFromBlock()
        {
            // Arrange
            var block = builder.Paragraph();
            block.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(block.Id, 0).Selection,
                UpdateMarkCommand.Toggle(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(block.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
        
        [Test]
        public void ActionAdd_InEmptyBlock_AddsMarkToBlock()
        {
            // Arrange
            var block = builder.Paragraph();
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(block.Id, 0).Selection,
                UpdateMarkCommand.Add(MarkKey.FontWeight, "bold"));
            
            // Assert
            Assert.That(block.Marks, Is.EquivalentTo(new Dictionary<MarkKey, string>
            {
                { MarkKey.FontWeight, "bold" }
            }));
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
        
        [Test]
        public void ActionRemove_InEmptyBlock_RemovesMarkFromBlock()
        {
            // Arrange
            var block = builder.Paragraph();
            block.SetMark(MarkKey.FontWeight, "bold");
            
            // Act
            var result = handler.Execute(
                KeyPressInfoHelper.GetKeyPressInfoDirectionNone(block.Id, 0).Selection,
                UpdateMarkCommand.Remove(MarkKey.FontWeight));
            
            // Assert
            Assert.That(block.Marks, Is.Empty);
            Assert.That(result.CursorPosition, Is.Null);
            AssertHelper.ThatStepsEqualTo(result.Steps, expected: [
                Is.TypeOf<StepDiff.UpdateMarksDiff>()
            ]);
        }
    }
}