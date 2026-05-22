using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.Transactions.Steps;

public class DeleteStepTests
{
    private DScratchDocument Document { get; set; }

    [SetUp]
    public void SetUp()
    {
        Document = new DScratchDocument();
    }
    
    [Test]
    public void DeletesNodeAtPathAndOffset()
    {
        // Arrange
        var node3 = new TestInlineElementNode("3", null, null);
        var node4 = new TestNode("4", node3, null);
        node3.RightOrigin = node4;
        var node5 = new TestNode("5", node4, null);
        node4.RightOrigin = node5;
        
        var node1 = new TestNode("1", null, null);
        var node2 = new TestNode("2", node1, null, [node3, node4, node5]);
        node1.RightOrigin = node2;

        node3.Parent = node2;
        node4.Parent = node2;
        node5.Parent = node2;
        
        Document.Page.Root = node1;
        
        // Act
        var step = new DeleteStep(node3);
        step.Execute(Document);
            
        // Assert
        Assert.That(node3.IsDeleted, Is.True);
    }
}