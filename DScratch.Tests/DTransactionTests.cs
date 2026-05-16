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
    
    private class InsertAt : DTransactionTests
    {
        
    }
    
    private class FindNode : DTransactionTests
    {
        [Test]
        public void ReturnsExpectedNode_FromGivenPath()
        {
            // Arrange
            DefaultNodes();
            
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
            DefaultNodes();
            
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
            var result = Transaction.FindNode<TestNode>(new NodePath(["2"]));
            
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
    
    private class FindNodeInternal : DTransactionTests
    {
        [Test]
        public void Find_FirstNodePath()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["1"]));
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("1"));
        }
        
        [Test]
        public void Find_SecondNode()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["2"]));
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("2"));
        }
        
        [Test]
        public void Find_FirstChild()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["2", "3"]));
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("3"));
        }
        
        [Test]
        public void Find_SecondChild()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["2", "4"]));
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("4"));
        }
        
        [Test]
        public void Find_LastChild()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["2", "5"]));
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("5"));
        }
        
        [Test]
        public void ReturnsNull_WhenNodeDoesNotExist()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["abc"]));
            
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void ReturnsNull_WhenChildNodeDoesNotExist()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            var result = DTransaction.FindNodeInternal(Document.Page.Root, new NodePath(["1", "abc"]));
            
            // Assert
            Assert.That(result, Is.Null);
        }
    }

    private void DefaultNodes()
    {
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
    }
}