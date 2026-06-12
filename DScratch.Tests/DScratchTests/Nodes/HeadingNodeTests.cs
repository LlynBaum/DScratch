using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class HeadingNodeTests
{
    [Test]
    [TestCase(HeadingLevel.Level1, 1)]
    [TestCase(HeadingLevel.Level2, 2)]
    [TestCase(HeadingLevel.Level3, 3)]
    [TestCase(HeadingLevel.Level4, 4)]
    [TestCase(HeadingLevel.Level5, 5)]
    [TestCase(HeadingLevel.Level6, 6)]
    public void TagName_ReturnsExpectedDomElementName(HeadingLevel headingLevel, int tagNumber)
    {
        var heading = new HeadingNode(headingLevel, new NodeId(), null, null);
        Assert.That(heading.TagName, Is.EqualTo($"h{tagNumber}"));
    }
    
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotText()
    {
        // Arrange
        var heading = new HeadingNode(HeadingLevel.Level1, new NodeId(), null, null);
        var node = new TestNode(new NodeId(), null, null)
        {
            Parent = heading
        };

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => heading.InsertChild(node);
    }
}