using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

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
        var builder = new TreeBuilder();
        DNode node1 = builder.TestNode(); // ID "0"
        DNode node3 = null!;
        builder.TestNode(t => // ID "1"
        {
            node3 = t.TestInlineElementNode(); // ID "2"
            t.TestNode(); // ID "3"
            t.TestNode(); // ID "4"
        });
        
        Document.Page.Root = node1;
        
        // Act
        var step = new DeleteStep(node3);
        step.Execute(Document);
            
        // Assert
        Assert.That(node3.IsDeleted, Is.True);
    }
    
    [Test]
    public void DeletesNodeAtPathAndOffset_TextNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node1 = builder.TestNode(); // ID "0"
        
        // ID "1": TextNode, IDs "2", "3", "4": CharNodes
        TextNode node2 = builder.Text("aaa"); 
        
        Document.Page.Root = node1;
        var node3 = node2.ChildNodes.First(); // Targets first CharNode (ID "2")
        
        // Act
        var steps = new DeleteStep(node2);
        var diffs = steps.Execute(Document);
        
        // Assert
        Assert.That(node3.IsDeleted, Is.True);
        Assert.That(diffs, Has.Count.EqualTo(1));
        Assert.That(diffs.Single(), Is.TypeOf<StepDiff.DeleteTextDiff>());
        
        var step = (StepDiff.DeleteTextDiff)diffs.Single();
        Assert.That(step.Offset, Is.EqualTo(0));
        Assert.That(step.Length, Is.EqualTo(3));
    }
}