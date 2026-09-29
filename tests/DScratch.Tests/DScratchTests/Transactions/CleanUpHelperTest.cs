using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Transactions;

[TestFixture]
public class CleanUpHelperTest
{
    private class CanMergeWithPrevious : CleanUpHelperTest
    {
        [Test]
        public void ReturnsTrue_ForTextRuns()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.True);
            Assert.That(node, Is.EqualTo(textNodeLeft));
        }
        
        [Test]
        public void ReturnsTrue_ForTextRuns_Deleted()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.Delete();
            textNodeRight.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.True);
            Assert.That(node, Is.EqualTo(textNodeLeft));
        }
        
        [Test]
        public void ReturnsFalse_FroDeletedMismatch()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void ReturnsFalse_WhenOriginIsNot_LastIdFromPrevious()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), new NodeId("whatever", 1), rightOriginId, "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void ReturnsFalse_FroRightOriginMismatch()
        {
            // Arrange
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, new NodeId("Darki", 1), "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, new NodeId("Luna", 2), "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        [TestCase(3)]
        [TestCase(5)]
        public void ReturnsFalse_WhenNodeIdsAreNotContinues(int clock)
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", clock), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }

        [Test] 
        public void ReturnsFalse_FroMarksMismatch()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.SetMark(MarkKey.Color, "#ffffff");
            textNodeRight.SetMark(MarkKey.Color, "#000000");

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithPrevious(textNodeRight, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
    }
    
    private class CanMergeWithNext : CleanUpHelperTest
    {
        [Test]
        public void ReturnsTrue_ForTextRuns()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.True);
            Assert.That(node, Is.EqualTo(textNodeRight));
        }
        
        [Test]
        public void ReturnsTrue_ForTextRuns_Deleted()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.Delete();
            textNodeRight.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.True);
            Assert.That(node, Is.EqualTo(textNodeRight));
        }
        
        [Test]
        public void ReturnsFalse_FroDeletedMismatch()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void ReturnsFalse_WhenOriginIsNot_LastIdFromPrevious()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), new NodeId("whatever", 1), rightOriginId, "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void ReturnsFalse_FroRightOriginMismatch()
        {
            // Arrange
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, new NodeId("Darki", 1), "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, new NodeId("Luna", 2), "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        [TestCase(3)]
        [TestCase(5)]
        public void ReturnsFalse_WhenNodeIdsAreNotContinues(int clock)
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", clock), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.Delete();

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }

        [Test] 
        public void ReturnsFalse_FroMarksMismatch()
        {
            // Arrange
            var rightOriginId = new NodeId("Darki", 1);
            var textNodeLeft = new TextNode(new NodeId("Test", 1), null, rightOriginId, "abc");
            var textNodeRight = new TextNode(new NodeId("Test", 4), textNodeLeft.LastId, rightOriginId, "def");
            
            textNodeLeft.SetMark(MarkKey.Color, "#ffffff");
            textNodeRight.SetMark(MarkKey.Color, "#000000");

            CreateParent(textNodeLeft, textNodeRight);
        
            // Act
            var result = CleanUpHelper.CanMergeWithNext(textNodeLeft, out var node);
        
            // Assert
            Assert.That(result, Is.False);
        }
    }

    private static void CreateParent(params DNode[] nodes)
    {
        var parent = TestNode.Empty();
        ((List<DNode>)parent.ChildNodes).AddRange(nodes);
        parent.ClaimChildNodes();
    }
}