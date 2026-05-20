namespace DScratch.Tests.Transactions.Steps;

public class InsertStepTests
{
    // TODO
        private class InsertAt : DTransactionTests
    {
        [Test]
        public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath()
        {
            // Arrange
            DefaultNodes();
            var node = new TestNode("-1", null, null, null, null);
            
            // Act
            Transaction.Insert(node, new NodePath(["2", "3"]), 0);
            
            // Assert
            var insertedNode = Document.Page.Root.RightOrigin?.FirstChild?.FirstChild;
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(insertedNode?.Id, Is.EqualTo("-1"));
                
                Assert.That(node.Parent?.Id, Is.EqualTo("3"));
                Assert.That(node.Origin, Is.Null);
                Assert.That(node.RightOrigin, Is.Null);
            }
        }

        [Test]
        public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AsFirstChild()
        {
            // Arrange
            DefaultNodes();
            var node = new TestNode("-1", null, null, null, null);
            
            // Act
            Transaction.Insert(node, new NodePath(["2", "4"]), 0);
            
            // Assert
            var insertedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.FirstChild;
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(insertedNode?.Id, Is.EqualTo("-1"));
                
                Assert.That(node.Parent?.Id, Is.EqualTo("4"));
                Assert.That(node.Origin, Is.Null);
                Assert.That(node.RightOrigin?.Id, Is.EqualTo("6"));
            }
        }
        
        [Test]
        public void GivenNode_IsInsertedAsChildOfNodeFromGivenPath_AfterExistingChild()
        {
            // Arrange
            DefaultNodes();
            var node = new TestNode("-1", null, null, null, null);
            
            // Act
            Transaction.Insert(node, new NodePath(["2", "4"]), 1);
            
            // Assert
            var insertedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.FirstChild?.RightOrigin;
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(insertedNode?.Id, Is.EqualTo("-1"));
                
                Assert.That(node.Parent?.Id, Is.EqualTo("4"));
                Assert.That(node.Origin?.Id, Is.EqualTo("6"));
                Assert.That(node.RightOrigin, Is.Null);
            }
        }
    }
}