using DScratch.Nodes;

namespace DScratch.Tests.Nodes;

public class CharNodeTests
{
    [Test]
    public void InsertChild_ThrowsException()
    {
        var node = new CharNode('a', "1", null, null, null);

        Assert.Throws<InvalidOperationException>(Act);
        return;
        
        void Act() => node.InsertChild(new TestNode("2", null, null, null, null));
    }
    
    [Test]
    public void DeleteChild_ThrowsException()
    {
        var node = new CharNode('a', "1", null, null, null);

        Assert.Throws<InvalidOperationException>(Act);
        return;
        
        void Act() => node.DeleteChild("2");
    }

    [Test]
    public void NextChar_ReturnsRightOrigin()
    {
        var node = new CharNode('a', "1", null, null, null);
        var node2 = new CharNode('a', "2", null, null, null);
        var node3 = new CharNode('a', "3", node2, node, null);

        var actual = node3.NextChar;
        
        Assert.That(actual?.Id, Is.EqualTo("1"));
    }
}