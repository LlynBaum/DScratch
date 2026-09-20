using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class LinkNodeTest
{
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotText()
    {
        // Arrange
        var paragraph = new LinkNode(new NodeId(), null, null, "dscratch.darki.dev", "_self");
        var node = new TestNode(new NodeId(), null, null);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => paragraph.InsertChild(node);
    }
}