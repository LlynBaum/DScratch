using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Transactions;

[TestFixture]
public class DTransactionCleanUpTests
{
    private DScratchDocument Document { get; set; }
    private DTransaction Transaction { get; set; }
    private TreeBuilder TreeBuilder { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        TreeBuilder = new TreeBuilder();
        Document = TreeBuilder.CreateDocument();
        Transaction = new DTransaction(Document, new DNodeFactory(TreeBuilder.IdGenerator), new TestNodeIdGenerator(), false);
    }

    private class MergeContinuesTextNodes : DTransactionCleanUpTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Notify_SecondNode(bool deleted)
        {
            // Arrange
            TextNode node = null!;
            TextNode modifiedNode = null!;
            TreeBuilder.Paragraph(t =>
            {
                node = t.Text("abc");
                modifiedNode = t.Text("def");
            });

            if (deleted)
            {
                node.Delete();
                modifiedNode.Delete();
            }
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            Transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(modifiedNode.Parent, Is.Null);
                Assert.That(modifiedNode.Origin, Is.Null);
                Assert.That(modifiedNode.RightOrigin, Is.Null);
            }
        
            using (Assert.EnterMultipleScope())
            {
                Assert.That(node.TextContent, Is.EqualTo("abcdef"));
            }
        }
    
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Notify_FirstNode(bool deleted)
        {
            // Arrange
            TextNode node = null!;
            TextNode modifiedNode = null!;
            TreeBuilder.Paragraph(t =>
            {
                modifiedNode = t.Text("abc");
                node = t.Text("def");
            });
            
            if (deleted)
            {
                node.Delete();
                modifiedNode.Delete();
            }
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            Transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(node.Parent, Is.Null);
                Assert.That(node.Origin, Is.Null);
                Assert.That(node.RightOrigin, Is.Null);
            }
        
            using (Assert.EnterMultipleScope())
            {
                Assert.That(modifiedNode.TextContent, Is.EqualTo("abcdef"));
            }
        }
    }
    
    private class DoNotMergeContinuesNodesInDifferentState : DTransactionCleanUpTests
    {
        [Test]
        public void Notify_SecondNode()
        {
            // Arrange
            TextNode modifiedNode = null!;
            var parent = TreeBuilder.Paragraph(t =>
            {
                t.Text("abc").Delete();
                modifiedNode = t.Text("def");
            });
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            Transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        }
    
        [Test]
        public void Notify_FirstNode()
        {
            // Arrange
            TextNode modifiedNode = null!;
            var parent = TreeBuilder.Paragraph(t =>
            {
                t.Text("abc").Delete();
                modifiedNode = t.Text("def");
            });
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            Transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        }
    }
    
    private class DoNotMergeNonContinuesNodes : DTransactionCleanUpTests
    {
        [Test]
        public void Notify_SecondNode()
        {
            // Arrange
            TextNode modifiedNode = null!;
            var parent = TreeBuilder.Paragraph(t =>
            {
                t.Text("abc");
                TreeBuilder.IdGenerator.TakeIds(1);
                modifiedNode = t.Text("def");
            });
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            Transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        }
    
        [Test]
        public void Notify_FirstNode()
        {
            // Arrange
            TextNode modifiedNode = null!;
            var parent = TreeBuilder.Paragraph(t =>
            {
                t.Text("abc");
                TreeBuilder.IdGenerator.TakeIds(1);
                modifiedNode = t.Text("def");
            });
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            Transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        }
    }
}