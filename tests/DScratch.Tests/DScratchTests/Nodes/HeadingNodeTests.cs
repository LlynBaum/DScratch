using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class HeadingNodeTests
{
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotText()
    {
        // Arrange
        var heading = new HeadingNode(HeadingLevel.Level1, new NodeId(), null, null);
        var node = new TestNode(new NodeId(), null, null);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => heading.InsertChild(node);
    }
}