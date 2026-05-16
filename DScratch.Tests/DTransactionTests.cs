using DScratch.Nodes;

namespace DScratch.Tests;

public class DTransactionTests
{
    private DScratchDocument Document { get; set; }
    private DTransaction Transaction { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
        Transaction = new DTransaction(Document);
    }
    
    private class FindNode : DTransactionTests
    {
        [Test]
        public void ReturnsExpectedNode_FromGivenPath()
        {
            // Arrange
            var node1 = new TestNode("1", null, null, null, null);
            var node2 = new TestNode("2", node1, null, null, null);
            node1.RightOrigin = node2;
            
            var node3 = new TestNode("3", null, null, node2, null);
            node2.FirstChild = node3;
            var node4 = new TestNode("4", node3, null, node2, null);
            node3.RightOrigin = node4;
            var node5 = new TestNode("5", node4, null, node2, null);
            node4.RightOrigin = node5;

            Document.Page.Root = node1;
            
            // Act
            var result = Transaction.FindNode(new NodePath(["2", "4"]));
            
            // Assert
            Assert.That(result, Is.Not.Null, "Expected to find a node at path \"2__4\".");
            Assert.That(result.Id, Is.EqualTo("4"));
        }
        
        [Test]
        public void ReturnsExpectedNode_WhenNodeWasOfExpectedType()
        {
            // Arrange
            var node1 = new TestNode("1", null, null, null, null);
            var node2 = new TestNode("2", node1, null, null, null);
            node1.RightOrigin = node2;
            
            var node3 = new TestNode("3", null, null, node2, null);
            node2.FirstChild = node3;
            var node4 = new TestNode("4", node3, null, node2, null);
            node3.RightOrigin = node4;
            var node5 = new TestNode("5", node4, null, node2, null);
            node4.RightOrigin = node5;

            Document.Page.Root = node1;
            
            // Act
            var result = Transaction.FindNode<TestNode>(new NodePath(["2", "4"]));
            
            // Assert
            Assert.That(result, Is.Not.Null, "Expected to find a node at path \"2__4\".");
            Assert.That(result.Id, Is.EqualTo("4"));
        }
        
        [Test]
        public void ReturnsNull_WhenNodeWasNotFound()
        {
            // Arrange
            Document.Page.Root = new TestNode("1", null, null, null, null);
            
            // Act
            var result = Transaction.FindNode<TestNode>(new NodePath(["1"]));
            
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void ThrowsArgumentException_WhenNodeWasNotOfExpectedType()
        {
            Document.Page.Root = new TestNode("1", null, null, null, null);

            Assert.Throws<ArgumentException>(Act);
            return;

            void Act() => Transaction.FindNode<DCharNode>(new NodePath(["1"]));
        }
    }
    
    private class InsertAt : DTransactionTests
    {
        
    }
}