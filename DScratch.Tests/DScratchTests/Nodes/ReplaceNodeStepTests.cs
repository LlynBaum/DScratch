using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Nodes;

public class ReplaceNodeStepTests
{
    private TreeBuilder builder;
    private INodeFactory nodeFactory;
    
    [SetUp]
    public void Setup()
    {
        builder = new TreeBuilder();
        nodeFactory = new DNodeFactory(new TestNodeIdGenerator());
    }
    
    [Test]
    public void ReplaceNodeStep_MakesExpectedChanges()
    {
        // Arrange
        DNode child = null!;
        var origin = builder.TestNode();
        var nodeToReplace = builder.Paragraph(t => child = t.Text("a"));
        var rightOrigin = builder.TestNode();
        
        // Act
        new ReplaceNodeStep(nodeToReplace, n => nodeFactory.HeadingFrom(n, HeadingLevel.Level1))
            .Execute(new TestTransactionFake(), builder.CreateDocument());
        
        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
        Assert.That(builder.Root.ChildNodes[1], Is.TypeOf<HeadingNode>());

        var testee = (HeadingNode)builder.Root.ChildNodes[1];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(testee.Id, Is.EqualTo(nodeToReplace.Id));
            Assert.That(testee.HeadingLevel, Is.EqualTo(HeadingLevel.Level1));
            Assert.That(testee.Parent, Is.EqualTo(builder.Root));
            Assert.That(testee.Origin, Is.EqualTo(origin));
            Assert.That(testee.RightOrigin, Is.EqualTo(rightOrigin));
            Assert.That(testee.ChildNodes, Is.EquivalentTo([child]));

            Assert.That(nodeToReplace.Parent, Is.Null);
            Assert.That(nodeToReplace.Origin, Is.Null);
            Assert.That(nodeToReplace.RightOrigin, Is.Null);
        }
    }
}