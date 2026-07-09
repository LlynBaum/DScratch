using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class DeleteStepTests
{
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        transactionFake = new TestTransactionFake();
    }
    
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(node3.IsDeleted, Is.True);
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
                new ModifiedNode(node3, Modification.Delete),
        ]));
        }
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
        steps.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ActiveChildNodes.Count(), Is.EqualTo(2));
            Assert.That(textNode.IsDeleted, Is.True);
            Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
                new ModifiedNode(textNode, Modification.Delete),
            ]));
        }
    }
}