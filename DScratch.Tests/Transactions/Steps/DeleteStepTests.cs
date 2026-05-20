namespace DScratch.Tests.Transactions.Steps;

public class DeleteStepTests
{
    // TODO
    private class DeleteNode : DTransactionTests
    {
        [Test]
        public void DeletesNodeAtPathAndOffset()
        {
            // Arrange
            DefaultNodes();
            
            // Act
            Transaction.DeleteNode(new NodePath(["2"]), 2);
            
            // Assert
            var deletedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.RightOrigin!;
            Assert.That(deletedNode.IsDeleted, Is.True);
        }
    }
}