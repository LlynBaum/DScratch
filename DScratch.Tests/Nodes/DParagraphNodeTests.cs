using DScratch.Nodes;

namespace DScratch.Tests.Nodes;

public class DParagraphNodeTests
{
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNodCharNode()
    {
        // Arrange
        var paragraph = new DParagraphNode("1", null, null, null);
        var node = new TestNode("2", null, null, paragraph, null);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => paragraph.InsertChild(node);
    }
    
    [Test]
    public void InsertChild_UpdatesTextValue_AsExpected()
    {
        // Arrange
        var paragraph = new DParagraphNode("1", null, null, null);
        var char1 = new DCharNode('a', "2", null, null, paragraph);
        var char3 = new DCharNode('c', "3", char1, null, paragraph);
        var char2 = new DCharNode('b', "4", char1, char3, paragraph);
        
        // Act
        paragraph.InsertChild(char1);
        paragraph.InsertChild(char3);
        paragraph.InsertChild(char2);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(paragraph.Value, Is.EqualTo("abc"));
        });
    }

    [Test]
    public void DeleteChild_RemovesCharacterFromValueString()
    {
        // Arrange
        var paragraph = new DParagraphNode("1", null, null, null);
        var char1 = new DCharNode('a', "2", null, null, paragraph);
        var char3 = new DCharNode('c', "3", char1, null, paragraph);
        var char2 = new DCharNode('b', "4", char1, char3, paragraph);
        paragraph.InsertChild(char1);
        paragraph.InsertChild(char3);
        paragraph.InsertChild(char2);
        
        // Act
        paragraph.DeleteChild("4");
        
        // Assert
        Assert.That(paragraph.Value, Is.EqualTo("ac"));
    }
}