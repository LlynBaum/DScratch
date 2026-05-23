using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using NUnit.Framework.Internal;

namespace DScratch.Tests.Transactions.Steps;

public class StepHelpersTests
{
    private class ToInsert
    {
        [Test]
        public void WhenParentIsNull_FallbackToRoot()
        {
            // Arrange
            var node = new TestInlineElementNode("1", null, null);

            // Act
            var result = node.ToInsert();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertElementInlineDiff>());
            
            var step = (StepDiff.InsertElementInlineDiff)result.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.Parent, Is.EquivalentTo(["root"]));
                Assert.That(step.Offset, Is.EqualTo(0));
            }
        }
        
        [Test]
        public void CharNode_ReturnsInsertTextDiff()
        {
            // Arrange
            var existing = new CharNode('a', "3", null, null);
            var node = new CharNode('a', "2", existing, null);
            existing.RightOrigin = node;
            
            var parent = new TestInlineElementNode("1", null, null, [existing, node]);
            existing.Parent = parent;
            node.Parent = parent;

            // Act
            var result = node.ToInsert();
            
            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertTextDiff>());
            
            var step = (StepDiff.InsertTextDiff)result.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.Parent, Is.EquivalentTo(["1"]));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Text, Is.EqualTo("a"));
            }
        }
        
        [Test]
        public void Element_ReturnsInsertElementDiff()
        {
            // Arrange
            var charNode = new CharNode('a', "4", null, null);
            var testNode = new TextNode("3", null, null, [charNode]);
            charNode.Parent = testNode;

            var paragraph = new ParagraphNode("2", null, null, [testNode]);
            testNode.Parent = paragraph;
            
            var parent = new TestInlineElementNode("1", null, null, [TestNode.Empty(), paragraph]);
            paragraph.Parent = parent;
            
            // Act
            var result = paragraph.ToInsert();
            
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
                Assert.That(insertParagraph.Parent, Is.EquivalentTo(["1"]));
                Assert.That(insertParagraph.PreviousSibling, Is.Null);
                Assert.That(insertParagraph.NewNodeId, Is.EqualTo(paragraph.Id));
                Assert.That(insertParagraph.TagName, Is.EqualTo(paragraph.TagName));
                
                var insertText = (StepDiff.InsertTextDiff)result[1];
                Assert.That(insertText.Parent, Is.EquivalentTo(["1", paragraph.Id]));
                Assert.That(insertText.Offset, Is.EqualTo(0));
                Assert.That(insertText.Text, Is.EqualTo("a"));
            }
        }
        
        [Test]
        public void TextElement_ReturnsExpectedSteps()
        {
            // Arrange
            var charNode1 = new CharNode('a', "4", null, null);
            var leftNode = new TextNode("3", null, null, [charNode1]);
            charNode1.Parent = leftNode;

            var charNode2 = new CharNode('b', "5", null, null);
            var node = new TextNode("6", null, null, [charNode2]);
            var testElement = new TestInlineElementNode("7", leftNode, null, [node]);
            leftNode.RightOrigin = testElement;
            charNode2.Parent = node;
            node.Parent = testElement;
            
            var charNode3 = new CharNode('c', "8", null, null);
            var rightNode = new TextNode("9", testElement, null, [charNode3]);
            testElement.RightOrigin = rightNode;
            charNode3.Parent = rightNode;

            var paragraph = new ParagraphNode("2", null, null, [leftNode, testElement, rightNode]);
            leftNode.Parent = paragraph;
            testElement.Parent = paragraph;
            rightNode.Parent = paragraph;
            
            var parent = new TestInlineElementNode("1", null, null, [TestNode.Empty(), paragraph]);
            paragraph.Parent = parent;
            
            // Act
            var result = paragraph.ToInsert();

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
                Assert.That(insertParagraph.Parent, Is.EquivalentTo(["1"]));
                Assert.That(insertParagraph.PreviousSibling, Is.Null);
                Assert.That(insertParagraph.NewNodeId, Is.EqualTo(paragraph.Id));
                Assert.That(insertParagraph.TagName, Is.EqualTo(paragraph.TagName));
                
                var insertLeftText = (StepDiff.InsertTextDiff)result[1];
                Assert.That(insertLeftText.Parent, Is.EquivalentTo(["1", paragraph.Id]));
                Assert.That(insertLeftText.Offset, Is.EqualTo(0));
                Assert.That(insertLeftText.Text, Is.EqualTo("a"));
                
                var insertTestElement = (StepDiff.InsertElementInlineDiff)result[2];
                Assert.That(insertTestElement.Parent, Is.EquivalentTo(["1", paragraph.Id]));
                Assert.That(insertTestElement.Offset, Is.EqualTo(1));
                Assert.That(insertTestElement.NewNodeId, Is.EqualTo("7"));
                Assert.That(insertTestElement.TagName, Is.EqualTo(testElement.TagName));
                
                var insertText = (StepDiff.InsertTextDiff)result[3];
                Assert.That(insertText.Parent, Is.EquivalentTo(["1", paragraph.Id, testElement.Id]));
                Assert.That(insertText.Offset, Is.EqualTo(0));
                Assert.That(insertText.Text, Is.EqualTo("b"));
                
                var insertRightText = (StepDiff.InsertTextDiff)result[4];
                Assert.That(insertRightText.Parent, Is.EquivalentTo(["1", paragraph.Id]));
                Assert.That(insertRightText.Offset, Is.EqualTo(2));
                Assert.That(insertRightText.Text, Is.EqualTo("c"));
            }
        }
        
        [Test]
        public void UnknownNodes_ThrowArgumentException()
        {
            // Arrange
            var testNode = TestNode.Empty();
            testNode.Parent = TestNode.Empty();
            
            //Assert
            Assert.Throws<ArgumentException>(Act);
            return;

            // Act
            void Act() => testNode.ToInsert();
        }
    }
    
    private class ToDelete
    {
        [Test]
        public void CharNode_ReturnsDeleteTextDiff()
        {
            // Arrange
            var node = new CharNode('a', "2", null, null);
            var parent = new TestInlineElementNode("1", null, null, [TestNode.Empty(), node]);
            node.Parent = parent;
            
            // Act
            var result = node.ToDelete();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteTextDiff>());
            var step = (StepDiff.DeleteTextDiff)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.Parent, Is.EquivalentTo(["1"]));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Length, Is.EqualTo(1));
            }
        }

        [Test]
        public void TextNode_ReturnDeleteTextDiff()
        {
            // Arrange
            var charNode = new CharNode('a', "4", null, null);
            var charNode2 = new CharNode('b', "5", null, null);
            var testNode = new TextNode("3", null, null, [charNode, charNode2]);
            charNode.Parent = testNode;
            charNode2.Parent = testNode;
            
            var parent = new TestInlineElementNode("1", null, null, [TestNode.Empty(), testNode]);
            testNode.Parent = parent;
            
            // Act
            var result = testNode.ToDelete();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteTextDiff>());
            var step = (StepDiff.DeleteTextDiff)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.Parent, Is.EquivalentTo(["1"]));
                Assert.That(step.Offset, Is.EqualTo(1));
                Assert.That(step.Length, Is.EqualTo(2));
            }
        }

        [Test]
        public void Elements_ReturnDeleteElementDiff()
        {
            // Arrange
            var testElement = new TestInlineElementNode("2", null, null);
            var parent = new TestInlineElementNode("1", null, null, [testElement]);
            testElement.Parent = parent;
            
            // Act
            var result = testElement.ToDelete();
            
            // Assert
            Assert.That(result, Is.TypeOf<StepDiff.DeleteElementDiff>());
            var step = (StepDiff.DeleteElementDiff)result;
            Assert.That(step.Path, Is.EquivalentTo(["1", "2"]));
        }
        
        [Test]
        public void UnknownNodes_ThrowArgumentException()
        {
            // Arrange
            var testNode = TestNode.Empty();
            testNode.Parent = TestNode.Empty();
            
            //Assert
            Assert.Throws<ArgumentException>(Act);
            return;

            // Act
            void Act() => testNode.ToDelete();
        }
    }
}