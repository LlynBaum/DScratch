using DScratch.Nodes;
using DScratch.Rendering;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Rendering;

public class AttributeRenderExtensionsTest
{
    [Test]
    public void ReturnsNull_WhenNotAttributesDefined()
    {
        // Arrange
        var linkNode = new TestNode(new NodeId(), null, null);

        // Act
        var result = linkNode.ToAttributeUpdate();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnsStep_WithAttributesFromNode()
    {
        // Arrange
        var linkNode = new LinkNode(new NodeId("Test", 1), null, null, "dscratch.darki.dev", "_self");

        // Act
        var result = linkNode.ToAttributeUpdate();

        // Assert
        Assert.That(result, Is.TypeOf<StepDiff.UpdateAttributesDiff>());
        var update = (StepDiff.UpdateAttributesDiff)result;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(update.NodeId, Is.EqualTo(linkNode.Id.Value));
            Assert.That(update.Attributes, Is.EquivalentTo(new Dictionary<string, string>
            {
                { "href", "dscratch.darki.dev" },
                { "target", "_self" }
            }));
        }
    }
}