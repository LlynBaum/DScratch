using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class DeleteStepTests
{
    [Test]
    public void DeletesNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        DNode node3 = null!;
        builder.TestNode(t => // ID "1"
        {
            node3 = t.TestInlineElementNode(); // ID "2"
            t.TestNode(); // ID "3"
            t.TestNode(); // ID "4"
        });
        
        // Act
        var step = new DeleteStep(node3);
        step.Execute();
            
        // Assert
        Assert.That(node3.IsDeleted, Is.True);
    }
    
    [Test]
    public void DeletesNode_TextNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(); // ID "0"
        
        // ID "1": TextNode, IDs "2", "3", "4": CharNodes
        TextNode textNode = null!;
        var parent = builder.TestInlineElementNode(t =>
        {
            textNode = t.Text("abc");
            t.Text("a");
            t.Text("a");
        }); 
        
        // Act
        var steps = new DeleteStep(textNode);
        var diffs = steps.Execute();
        
        // Assert
        Assert.That(parent.ActiveChildNodes.Count(), Is.EqualTo(2));
        Assert.That(textNode.IsDeleted, Is.True);
        Assert.That(diffs, Has.Count.EqualTo(1));
        Assert.That(diffs.Single(), Is.TypeOf<StepDiff.DeleteTextDiff>());
        
        var step = (StepDiff.DeleteTextDiff)diffs.Single();
        Assert.That(step.Offset, Is.EqualTo(0));
        Assert.That(step.Length, Is.EqualTo(3));
    }
}