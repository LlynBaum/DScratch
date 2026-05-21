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
        DefaultNodes();
    }
    
    [Test]
    public void DeletesNodeAtPathAndOffset()
    {
        // Arrange
        DefaultNodes();
        var step = new DeleteStep(new NodePath(["2"]), 2);
            
        // Act
        step.Execute(Document);
            
        // Assert
        var deletedNode = Document.Page.Root.RightOrigin?.FirstChild?.RightOrigin?.RightOrigin!;
        Assert.That(deletedNode.IsDeleted, Is.True);
    }
    
    private void DefaultNodes()
    {
        var node3 = new TestNode("3", null, null, null);
        var node4 = new TestNode("4", node3, null, null);
        var node5 = new TestNode("5", node4, null, null);
        node4.RightOrigin = node5;
        
        var node1 = new TestNode("1", null, null, null);
        var node2 = new TestNode("2", node1, null, null, [node3, node4, node5]);
        node1.RightOrigin = node2;

        node3.Parent = node2;
        node4.Parent = node2;
        node5.Parent = node2;
        
        Document.Page.Root = node1;
    }
}