using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

[TestFixture]
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
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertElementDiff>());
            
            var step = (StepDiff.InsertElementDiff)result.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.ParentId, Is.EqualTo("Root"));
                Assert.That(step.PreviousSiblingId, Is.Null);
            }
        }
        
        [Test]
        public void WhenParentIsNull_OriginIsFirstActiveElement()
        {
            // Arrange - Single node, manual setup is fine
            var builder = new TreeBuilder();

            var origin = builder.TestBlockElementNode();
            builder.TestBlockElementNode().Delete();
            var node = builder.TestBlockElementNode();

            // Act
            var result = node.ToInsertSteps();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertElementDiff>());
            
            var step = (StepDiff.InsertElementDiff)result.Single();
            Assert.That(step.PreviousSiblingId, Is.EqualTo(origin.Id.Value));
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
                    p.Text("a"); // 3: TextNode
                });
            });
            
            // Act
            var result = paragraph.ToInsertSteps();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(3));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.TypeOf<StepDiff.InsertElementDiff>());
                Assert.That(result[1], Is.TypeOf<StepDiff.InsertElementDiff>());
                Assert.That(result[2], Is.TypeOf<StepDiff.InsertTextDiff>());
            }
            
            using (Assert.EnterMultipleScope())
            {
                var insertParagraph = (StepDiff.InsertElementDiff)result[0];
                Assert.That(insertParagraph.ParentId, Is.EqualTo("Test-0"));
                Assert.That(insertParagraph.PreviousSiblingId, Is.EqualTo("Test-1"));
                Assert.That(insertParagraph.NewNodeId, Is.EqualTo("Test-2"));
                Assert.That(insertParagraph.TagName, Is.EqualTo(paragraph.TagName));
                
                var textRun = (StepDiff.InsertElementDiff)result[1];
                Assert.That(textRun.ParentId, Is.EqualTo("Test-2"));
                Assert.That(textRun.PreviousSiblingId, Is.Null);
                Assert.That(textRun.NewNodeId, Is.EqualTo("Test-3"));
                Assert.That(textRun.TagName, Is.EqualTo(paragraph.FirstChild!.TagName));
                
                var insertText = (StepDiff.InsertTextDiff)result[2];
                Assert.That(insertText.ParentId, Is.EqualTo("Test-3"));
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
                    p.Text("a"); // 3: TextNode
                    
                    // 4: TestInlineElementNode
                    p.TestInlineElementNode(te =>
                    {
                        te.Text("b"); // 5: TextNode
                    });
                    
                    p.Text("c"); // 6: TextNode
                });
            });
            
            // Act
            var result = paragraph.ToInsertSteps();

            // Assert
            Assert.That(result, Has.Length.EqualTo(8));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.TypeOf<StepDiff.InsertElementDiff>());
                
                Assert.That(result[1], Is.TypeOf<StepDiff.InsertElementDiff>());
                Assert.That(result[2], Is.TypeOf<StepDiff.InsertTextDiff>());
                
                Assert.That(result[3], Is.TypeOf<StepDiff.InsertElementDiff>());
                
                Assert.That(result[4], Is.TypeOf<StepDiff.InsertElementDiff>());
                Assert.That(result[5], Is.TypeOf<StepDiff.InsertTextDiff>());
                
                Assert.That(result[6], Is.TypeOf<StepDiff.InsertElementDiff>());
                Assert.That(result[7], Is.TypeOf<StepDiff.InsertTextDiff>());
            }
            
            using (Assert.EnterMultipleScope())
            {
                var insertParagraph = (StepDiff.InsertElementDiff)result[0];
                Assert.That(insertParagraph.ParentId, Is.EqualTo("Test-0"));
                Assert.That(insertParagraph.PreviousSiblingId, Is.EqualTo("Test-1"));
                Assert.That(insertParagraph.NewNodeId, Is.EqualTo("Test-2"));
                
                var leftTextSpan = (StepDiff.InsertElementDiff)result[1];
                Assert.That(leftTextSpan.ParentId, Is.EqualTo("Test-2"));
                Assert.That(leftTextSpan.PreviousSiblingId, Is.Null);
                Assert.That(leftTextSpan.NewNodeId, Is.EqualTo("Test-3"));
                
                var insertLeftText = (StepDiff.InsertTextDiff)result[2];
                Assert.That(insertLeftText.ParentId, Is.EqualTo("Test-3"));
                Assert.That(insertLeftText.Offset, Is.EqualTo(0));
                Assert.That(insertLeftText.Text, Is.EqualTo("a"));
                
                var insertTestElement = (StepDiff.InsertElementDiff)result[3];
                Assert.That(insertTestElement.ParentId, Is.EqualTo("Test-2"));
                Assert.That(insertTestElement.PreviousSiblingId, Is.EqualTo("Test-3"));
                Assert.That(insertTestElement.NewNodeId, Is.EqualTo("Test-4"));
                
                var innerTextSpan = (StepDiff.InsertElementDiff)result[4];
                Assert.That(innerTextSpan.ParentId, Is.EqualTo("Test-4"));
                Assert.That(innerTextSpan.PreviousSiblingId, Is.Null);
                Assert.That(innerTextSpan.NewNodeId, Is.EqualTo("Test-5"));
                
                var insertText = (StepDiff.InsertTextDiff)result[5];
                Assert.That(insertText.ParentId, Is.EqualTo("Test-5"));
                Assert.That(insertText.Offset, Is.EqualTo(0));
                Assert.That(insertText.Text, Is.EqualTo("b"));
                
                var insertRightTextSpan = (StepDiff.InsertElementDiff)result[6];
                Assert.That(insertRightTextSpan.ParentId, Is.EqualTo("Test-2"));
                Assert.That(insertRightTextSpan.PreviousSiblingId, Is.EqualTo("Test-4"));
                Assert.That(insertRightTextSpan.NewNodeId, Is.EqualTo("Test-6"));
                
                var insertRightText = (StepDiff.InsertTextDiff)result[7];
                Assert.That(insertRightText.ParentId, Is.EqualTo("Test-6"));
                Assert.That(insertRightText.Offset, Is.EqualTo(0));
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
    }
    
    private class ToMoveStep
    {
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
            testElement.Origin = targetSibling;
            testElement.RightOrigin = targetSibling.RightOrigin;
            targetParent.InsertChild(testElement);
            var result = testElement.ToMoveStep();
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<StepDiff.MoveDiff>());
            
            var step = (StepDiff.MoveDiff)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.TargetNodeId, Is.EqualTo("Test-2"));
                Assert.That(step.TargetParentId, Is.EqualTo("Test-4"));
                Assert.That(step.PreviousSiblingId, Is.EqualTo("Test-5"));
            }
        }
    }
}