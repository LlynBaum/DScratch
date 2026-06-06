using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class CharNodeTests
{
    [Test]
    public void InsertChild_ThrowsException()
    {
        var node = new CharNode('a', new NodeId(), null, null);
        Assert.Throws<InvalidOperationException>(Act);
        return;
        
        void Act() => node.InsertChild(new TestNode(new NodeId(), null, null));
    }

    [Test]
    public void Value_ReturnsExpectedCharacter()
    {
        var node = new CharNode('a', new NodeId(), null, null);
        Assert.That(node.Value, Is.EqualTo('a'));
    }
}