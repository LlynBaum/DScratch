using DScratch.Nodes;

namespace DScratch.Tests.Nodes;

public class DParagraphNodeTests
{
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNodCharNode()
    {
        var paragraph = new DParagraphNode("1");
        var node = new TestNode("2", null, null, null);

        Assert.Throws<InvalidOperationException>(Act);
        return;

        void Act() => paragraph.InsertChild(node);
    }
    
    [Test]
    public void InsertChild_UpdatesTextValue_AsExpected()
    {
        var paragraph = new DParagraphNode("1");
        var char1 = new DCharNode('a', "2", null, null);
        var char3 = new DCharNode('c', "3", char1, null);
        var char2 = new DCharNode('b', "4", char1, char3);
        
        paragraph.InsertChild(char1);
        paragraph.InsertChild(char3);
        paragraph.InsertChild(char2);
        
        Assert.Multiple(() =>
        {
            Assert.That(paragraph.Value, Is.EqualTo("abc"));
        });
    }
}