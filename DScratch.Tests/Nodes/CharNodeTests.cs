using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Nodes;

public class CharNodeTests
{
    [Test]
    public void InsertChild_ThrowsException()
    {
        var node = new CharNode('a', "1", null, null, null);
        Assert.Throws<InvalidOperationException>(Act);
        return;
        
        void Act() => node.InsertChild(new TestNode("2", null, null, null));
    }

    [Test]
    public void Value_ReturnsExpectedCharacter()
    {
        var node = new CharNode('a', "1", null, null, null);
        Assert.That(node.Value, Is.EqualTo('a'));
    }
}