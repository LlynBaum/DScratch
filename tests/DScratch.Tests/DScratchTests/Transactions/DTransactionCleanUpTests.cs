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
        [TestCase(true, Modification.Delete)]
        [TestCase(false, Modification.Insert)]
        public void Notify_SecondNode(bool deleted, Modification modification)
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

            Transaction.AddCursorPosition(modifiedNode.Id, 2);
        
            // Act
            Transaction.NotifyNodeChange(new ModifiedNode(modifiedNode, modification));
            var result = Transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(modifiedNode.Parent, Is.Null);
                Assert.That(modifiedNode.Origin, Is.Null);
                Assert.That(modifiedNode.RightOrigin, Is.Null);
                Assert.That(node.TextContent, Is.EqualTo("abcdef"));
            }
            
            Assert.That(result.ModifiedNodes, Has.Count.EqualTo(1));
            var modifiedNodes = result.ModifiedNodes.Single();
            using (Assert.EnterMultipleScope())
            {
                var expectedNode = deleted ? modifiedNode : node;
                Assert.That(modifiedNodes.Node, Is.EqualTo(expectedNode));

                var expected = deleted ? Modification.Delete : Modification.Changed;
                Assert.That(modifiedNodes.Modification, Is.EqualTo(expected));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, node.Id, 5);
        }
    
        [Test]
        [TestCase(true, Modification.Delete)]
        [TestCase(false, Modification.Insert)]
        public void Notify_FirstNode(bool deleted, Modification modification)
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
            
            Transaction.AddCursorPosition(node.Id, 2);
        
            // Act
            Transaction.NotifyNodeChange(new ModifiedNode(modifiedNode, modification));
            var result = Transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(node.Parent, Is.Null);
                Assert.That(node.Origin, Is.Null);
                Assert.That(node.RightOrigin, Is.Null);
                Assert.That(modifiedNode.TextContent, Is.EqualTo("abcdef"));
            }
            
            Assert.That(result.ModifiedNodes, Has.Count.EqualTo(1));
            var modifiedNodes = result.ModifiedNodes.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(modifiedNodes.Node, Is.EqualTo(modifiedNode));
                Assert.That(modifiedNodes.Modification, Is.EqualTo(modification));
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, modifiedNode.Id, 5);
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
            Transaction.NotifyNodeChange(new ModifiedNode(modifiedNode, Modification.Insert));
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
                modifiedNode = t.Text("abc");
                t.Text("def").Delete();
            });
        
            // Act
            Transaction.NotifyNodeChange(new ModifiedNode(modifiedNode, Modification.Insert));
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
            Transaction.NotifyNodeChange(new ModifiedNode(modifiedNode, Modification.Insert));
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
            Transaction.NotifyNodeChange(new ModifiedNode(modifiedNode, Modification.Insert));
            Transaction.Commit();

            // Assert
            Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        }
    }
}