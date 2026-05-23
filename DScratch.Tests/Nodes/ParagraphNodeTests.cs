using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Nodes;

public class ParagraphNodeTests
{
    [Test]
    public void TagName_ReturnsExpectedDomElementName()
    {
        var paragraph = new ParagraphNode("1", null, null);
        Assert.That(paragraph.TagName, Is.EqualTo("p"));
    }
    
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotText()
    {
        // Arrange
        var paragraph = new ParagraphNode("1", null, null);
        var node = new TestNode("2", null, null)
        {
            Parent = paragraph
        };

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => paragraph.InsertChild(node);
    }
}