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

            Transaction.AddCursorPosition(modifiedNode.Id, 2);
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            var result = Transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(modifiedNode.Parent, Is.Null);
                Assert.That(modifiedNode.Origin, Is.Null);
                Assert.That(modifiedNode.RightOrigin, Is.Null);
                Assert.That(node.TextContent, Is.EqualTo("abcdef"));
            }

            if (!deleted)
            {
                AssertHelper.ThatStepsEqualTo(result.ModifiedNodes, expected: [
                    Is.TypeOf<StepDiff.InsertTextDiff>(),
                    Is.TypeOf<StepDiff.DeleteElementDiff>()
                ]);

                using (Assert.EnterMultipleScope())
                {
                    var insert = (StepDiff.InsertTextDiff)result.ModifiedNodes[0]!;
                    Assert.That(insert.ParentId, Is.EqualTo(node.Id.Value));
                    Assert.That(insert.Offset, Is.EqualTo(3));
                    Assert.That(insert.Text, Is.EqualTo("def"));
                
                    var delete = (StepDiff.DeleteElementDiff)result.ModifiedNodes[1]!;
                    Assert.That(delete.TargetId, Is.EqualTo(modifiedNode.Id.Value));
                }
            }
            
            AssertHelper.ThatCursorPositionEqualTo(result.CursorPosition, node.Id, 5);
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
            
            Transaction.AddCursorPosition(node.Id, 2);
        
            // Act
            Transaction.NotifyNodeChange(modifiedNode);
            var result = Transaction.Commit();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(node.Parent, Is.Null);
                Assert.That(node.Origin, Is.Null);
                Assert.That(node.RightOrigin, Is.Null);
                Assert.That(modifiedNode.TextContent, Is.EqualTo("abcdef"));
            }

            if (!deleted)
            {
                AssertHelper.ThatStepsEqualTo(result.ModifiedNodes, expected: [
                    Is.TypeOf<StepDiff.InsertTextDiff>(),
                    Is.TypeOf<StepDiff.DeleteElementDiff>()
                ]);
                
                using (Assert.EnterMultipleScope())
                {
                    var insert = (StepDiff.InsertTextDiff)result.ModifiedNodes[0]!;
                    Assert.That(insert.ParentId, Is.EqualTo(modifiedNode.Id.Value));
                    Assert.That(insert.Offset, Is.EqualTo(3));
                    Assert.That(insert.Text, Is.EqualTo("def"));
                
                    var delete = (StepDiff.DeleteElementDiff)result.ModifiedNodes[1]!;
                    Assert.That(delete.TargetId, Is.EqualTo(node.Id.Value));
                }
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