using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class StepHelpersTests
{
    private class ToInsert
    {
        [Test]
        public void WhenParentIsNull_FallbackToRoot()
        {
            // Arrange - Single node, manual setup is fine
            var node = new TestInlineElementNode(new NodeId(), null, null);

            // Act
            var result = node.ToInsertSteps();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertElementInlineDiff>());
            
            var step = (StepDiff.InsertElementInlineDiff)result.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.ParentId, Is.EqualTo("Root"));
                Assert.That(step.Offset, Is.EqualTo(0));
            }
        }
        
        [Test]
        public void CharNode_ReturnsInsertTextDiff()
        {
            // Arrange
            var builder = new TreeBuilder();
            TextNode textNode = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                // 1: TextNode, 2: CharNode ('a'), 3: CharNode ('a')
                textNode = t.Text("aa"); 
            });

            var node = textNode.ChildNodes[1]; // Target the second CharNode (ID: "3")

            // Act
            var result = node.ToInsertSteps();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertTextDiff>());
            
            var step = (StepDiff.InsertTextDiff)result.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.ParentId, Is.EqualTo("Test-0"));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Text, Is.EqualTo("a"));
            }
        }
        
        [Test]
        public void Element_ReturnsInsertElementDiff()
        {
            // Arrange
            var builder = new TreeBuilder();
            ParagraphNode paragraph = null!;

            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.TestNode(); // 1: Empty node sibling
                
                // 2: ParagraphNode
                paragraph = t.Paragraph(p =>
                {
                    p.Text("a"); // 3: TextNode, 4: CharNode ('a')
                });
            });
            
            // Act
            var result = paragraph.ToInsertSteps();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.TypeOf<StepDiff.InsertElementBlockDiff>());
                Assert.That(result[1], Is.TypeOf<StepDiff.InsertTextDiff>());
            }
            
            using (Assert.EnterMultipleScope())
            {
                var insertParagraph = (StepDiff.InsertElementBlockDiff)result[0];
                Assert.That(insertParagraph.ParentId, Is.EqualTo("Test-0"));
                Assert.That(insertParagraph.PreviousSiblingId, Is.Null);
                Assert.That(insertParagraph.NewNodeId, Is.EqualTo("Test-2"));
                Assert.That(insertParagraph.TagName, Is.EqualTo(paragraph.TagName));
                
                var insertText = (StepDiff.InsertTextDiff)result[1];
                Assert.That(insertText.ParentId, Is.EqualTo("Test-2"));
                Assert.That(insertText.Offset, Is.EqualTo(0));
                Assert.That(insertText.Text, Is.EqualTo("a"));
            }
        }
        
        [Test]
        public void TextElement_ReturnsExpectedSteps()
        {
            // Arrange
            var builder = new TreeBuilder();
            ParagraphNode paragraph = null!;

            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.TestNode(); // 1: Empty node sibling
                
                // 2: ParagraphNode
                paragraph = t.Paragraph(p =>
                {
                    p.Text("a"); // 3: TextNode, 4: CharNode ('a')
                    
                    // 5: TestInlineElementNode
                    p.TestInlineElementNode(te =>
                    {
                        te.Text("b"); // 6: TextNode, 7: CharNode ('b')
                    });
                    
                    p.Text("c"); // 8: TextNode, 9: CharNode ('c')
                });
            });
            
            // Act
            var result = paragraph.ToInsertSteps();

            // Assert
            Assert.That(result, Has.Length.EqualTo(5));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.TypeOf<StepDiff.InsertElementBlockDiff>());
                Assert.That(result[1], Is.TypeOf<StepDiff.InsertTextDiff>());
                Assert.That(result[2], Is.TypeOf<StepDiff.InsertElementInlineDiff>());
                Assert.That(result[3], Is.TypeOf<StepDiff.InsertTextDiff>());
                Assert.That(result[4], Is.TypeOf<StepDiff.InsertTextDiff>());
            }
            
            using (Assert.EnterMultipleScope())
            {
                var insertParagraph = (StepDiff.InsertElementBlockDiff)result[0];
                Assert.That(insertParagraph.ParentId, Is.EqualTo("Test-0"));
                Assert.That(insertParagraph.PreviousSiblingId, Is.Null);
                Assert.That(insertParagraph.NewNodeId, Is.EqualTo("Test-2"));
                
                var insertLeftText = (StepDiff.InsertTextDiff)result[1];
                Assert.That(insertLeftText.ParentId, Is.EqualTo("Test-2"));
                Assert.That(insertLeftText.Offset, Is.EqualTo(0));
                Assert.That(insertLeftText.Text, Is.EqualTo("a"));
                
                var insertTestElement = (StepDiff.InsertElementInlineDiff)result[2];
                Assert.That(insertTestElement.ParentId, Is.EqualTo("Test-2"));
                Assert.That(insertTestElement.Offset, Is.EqualTo(1));
                Assert.That(insertTestElement.NewNodeId, Is.EqualTo("Test-5"));
                
                var insertText = (StepDiff.InsertTextDiff)result[3];
                Assert.That(insertText.ParentId, Is.EqualTo("Test-5"));
                Assert.That(insertText.Offset, Is.EqualTo(0));
                Assert.That(insertText.Text, Is.EqualTo("b"));
                
                var insertRightText = (StepDiff.InsertTextDiff)result[4];
                Assert.That(insertRightText.ParentId, Is.EqualTo("Test-2"));
                Assert.That(insertRightText.Offset, Is.EqualTo(2));
                Assert.That(insertRightText.Text, Is.EqualTo("c"));
            }
        }
        
        [Test]
        public void UnknownNodes_ThrowArgumentException()
        {
            // Arrange - Single node setup
            var testNode = TestNode.Empty();
            testNode.Parent = TestNode.Empty();
            
            // Assert
            Assert.Throws<ArgumentException>(Act);
            return;

            // Act
            void Act() => testNode.ToInsertSteps();
        }
    }
    
    private class ToDelete
    {
        [Test]
        public void CharNode_ReturnsDeleteTextDiff()
        {
            // Arrange
            var builder = new TreeBuilder();
            TextNode text2 = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.Text("a");     // 1: TextNode, 2: CharNode ('a')
                text2 = t.Text("a"); // 3: TextNode, 4: CharNode ('a')
            });
            
            var node2 = text2.ChildNodes.First(); // Target second CharNode (ID: "4")
            
            // Act
            var result = node2.ToDeleteSteps();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteTextDiff>());
            var step = (StepDiff.DeleteTextDiff)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.ParentId, Is.EqualTo("Test-0"));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Length, Is.EqualTo(1));
            }
        }

        [Test]
        public void TextNode_ReturnDeleteTextDiff()
        {
            // Arrange
            var builder = new TreeBuilder();
            TextNode text2 = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.Text("a");      // 1: TextNode, 2: CharNode ('a')
                text2 = t.Text("aa"); // 3: TextNode, 4: CharNode ('a'), 5: CharNode ('a')
            });
            
            // Act
            var result = text2.ToDeleteSteps();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteTextDiff>());
            var step = (StepDiff.DeleteTextDiff)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.ParentId, Is.EqualTo("Test-0"));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Length, Is.EqualTo(2));
            }
        }

        [Test]
        public void Elements_ReturnDeleteElementDiff()
        {
            // Arrange
            var builder = new TreeBuilder();
            TestInlineElementNode testElement = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                testElement = t.TestInlineElementNode(); // 1: Target Element
            });
            
            // Act
            var result = testElement.ToDeleteSteps();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteElementDiff>());
            var step = (StepDiff.DeleteElementDiff)result;
            Assert.That(step.TargetId, Is.EqualTo("Test-1"));
        }
        
        [Test]
        public void UnknownNodes_ThrowArgumentException()
        {
            // Arrange - Single node setup
            var testNode = TestNode.Empty();
            testNode.Parent = TestNode.Empty();
            
            // Assert
            Assert.Throws<ArgumentException>(Act);
            return;

            // Act
            void Act() => testNode.ToDeleteSteps();
        }
        
        [Test]
        public void FindsDeletedNode_AndReturnsExpectedStep()
        {
            // Arrange
            var builder = new TreeBuilder();
            TextNode text2 = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.Text("a");
                t.Text("a").Delete();
                text2 = t.Text("a"); // 3: TextNode, 4: CharNode ('a')
            });
            
            var node2 = text2.ChildNodes.First(); // Target second CharNode (ID: "4")
            node2.Delete();
            
            // Act
            var result = node2.ToDeleteSteps();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteTextDiff>());
            var step = (StepDiff.DeleteTextDiff)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.ParentId, Is.EqualTo("Test-0"));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Length, Is.EqualTo(1));
            }
        }
    }
    
    private class ToMoveStep
    {
        [Test]
        public void CharNode_ReturnsExpectedSteps()
        {
            // Arrange
            var builder = new TreeBuilder();
            TextNode text2 = null!;
            TextNode newSibling = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.Text("a");
                text2 = t.Text("b");
                t.Text("c"); 
            });
            builder.TestInlineElementNode(t =>
            {
                newSibling = t.Text("1");
                t.Text("2");
            });

            var node = text2.ChildNodes[0];
            
            // Act
            var result = node.ToMoveStep(n =>
            {
                n.Origin = newSibling.ChildNodes[0];
                n.RightOrigin = newSibling.ChildNodes[0].RightOrigin;
                newSibling.InsertChild(n);
            });
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
            Assert.That(result[1], Is.TypeOf<StepDiff.InsertTextDiff>());
            
            var step1 = (StepDiff.DeleteTextDiff)result[0];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step1.ParentId, Is.EqualTo("Test-0"));
                Assert.That(step1.Offset, Is.EqualTo(1));
                Assert.That(step1.Length, Is.EqualTo(1));
            }
            
            var step2 = (StepDiff.InsertTextDiff)result[1];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step2.ParentId, Is.EqualTo("Test-7"));
                Assert.That(step2.Offset, Is.EqualTo(1));
                Assert.That(step2.Text, Is.EqualTo("b"));
            }
        }
        
        [Test]
        public void TextNode_ReturnsExpectedSteps()
        {
            // Arrange
            var builder = new TreeBuilder();
            TextNode text2 = null!;
            TextNode newSibling = null!;
            
            // 0: Parent Element
            builder.TestInlineElementNode(t =>
            {
                t.Text("a");
                text2 = t.Text("b");
                t.Text("c"); 
            });
            var newParent = builder.TestInlineElementNode(t =>
            {
                newSibling = t.Text("1");
                t.Text("2");
            });
            
            // Act
            var result = text2.ToMoveStep(node =>
            {
                node.Origin = newSibling;
                node.RightOrigin = newSibling.RightOrigin;
                newParent.InsertChild(node);
            });
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.TypeOf<StepDiff.DeleteTextDiff>());
            Assert.That(result[1], Is.TypeOf<StepDiff.InsertTextDiff>());
            
            var step1 = (StepDiff.DeleteTextDiff)result[0];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step1.ParentId, Is.EqualTo("Test-0"));
                Assert.That(step1.Offset, Is.EqualTo(1));
                Assert.That(step1.Length, Is.EqualTo(1));
            }
            
            var step2 = (StepDiff.InsertTextDiff)result[1];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step2.ParentId, Is.EqualTo("Test-7"));
                Assert.That(step2.Offset, Is.EqualTo(1));
                Assert.That(step2.Text, Is.EqualTo("b"));
            }
        }

        [Test]
        public void InlineElement_ReturnsExpectedSteps()
        {
            // Arrange
            var builder = new TreeBuilder();
            TestInlineElementNode testElement = null!;
            TextNode targetSibling = null!;
            
            // 0: Parent Element
            builder.TestBlockElementNode(t =>
            {
                t.TestInlineElementNode();
                testElement = t.TestInlineElementNode();
                t.TestInlineElementNode();
            });
            var targetParent = builder.TestBlockElementNode(t =>
            {
                targetSibling = t.Text("a");
                t.Text("b");
            });

            // Act
            var result = testElement.ToMoveStep(node =>
            {
                node.Origin = targetSibling;
                node.RightOrigin = targetSibling.RightOrigin;
                targetParent.InsertChild(node);
            });
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.Null);
            Assert.That(result[1], Is.TypeOf<StepDiff.MoveInlineDiff>());
            
            var step = (StepDiff.MoveInlineDiff)result[1];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.TargetNodeId, Is.EqualTo("Test-2"));
                Assert.That(step.TargetParentId, Is.EqualTo("Test-4"));
                Assert.That(step.TargetOffset, Is.EqualTo(1));
            }
        }
        
        [Test]
        public void BlockElement_ReturnsExpectedSteps()
        {
            // Arrange
            var builder = new TreeBuilder();
            TestBlockElementNode testElement = null!;
            TestBlockElementNode targetSibling = null!;
            
            // 0: Parent Element
            builder.TestBlockElementNode(t =>
            {
                t.TestBlockElementNode();
                testElement = t.TestBlockElementNode();
                t.TestBlockElementNode();
            });
            var targetParent = builder.TestBlockElementNode(t =>
            {
                targetSibling = t.TestBlockElementNode();
                t.TestBlockElementNode();
            });

            // Act
            var result = testElement.ToMoveStep(node =>
            {
                node.Origin = targetSibling;
                node.RightOrigin = targetSibling.RightOrigin;
                targetParent.InsertChild(node);
            });
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.Null);
            Assert.That(result[1], Is.TypeOf<StepDiff.MoveBlockDiff>());
            
            var step = (StepDiff.MoveBlockDiff)result[1];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.TargetNodeId, Is.EqualTo("Test-2"));
                Assert.That(step.TargetParentId, Is.EqualTo("Test-4"));
                Assert.That(step.PreviousSiblingId, Is.EqualTo("Test-5"));
            }
        }
        
        [Test]
        public void UnknownNodes_ThrowArgumentException()
        {
            // Arrange - Single node setup
            var testNode = TestNode.Empty();
            testNode.Parent = TestNode.Empty();
            
            // Assert
            Assert.Throws<ArgumentException>(Act);
            return;

            // Act
            void Act() => testNode.ToMoveStep(_ => {});
        }
    }
}