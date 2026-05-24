using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

public class DNodeFactoryTests
{
    [Test]
    public void ReturnsCharNode_WithNewId()
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var testNode = new CharNode('a', "1", null, null);
        var testNode2 = new CharNode('a', "2", null, null);
        var result = factory.Char('a', testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.EqualTo('a'));
            Assert.That(result.Id, Is.EqualTo("0"));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
        }
    }

    [Test]
    public void ReturnsTextNode_WithNewId()
    {
        var testNode = new TestNode("1", null, null);
        var testNode2 = new TestNode("2", null, null);
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var result = factory.String("abc", testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo("0"));
            Assert.That(result.TextContent, Is.EqualTo("abc"));
            Assert.That(result.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));

            var child1 = result.ChildNodes[0];
            Assert.That(child1, Is.TypeOf<CharNode>());
            Assert.That((child1 as CharNode)!.Value, Is.EqualTo('a'));
            
            var child2 = result.ChildNodes[1];
            Assert.That(child2, Is.TypeOf<CharNode>());
            Assert.That((child2 as CharNode)!.Value, Is.EqualTo('b'));
            
            var child3 = result.ChildNodes[2];
            Assert.That(child3, Is.TypeOf<CharNode>());
            Assert.That((child3 as CharNode)!.Value, Is.EqualTo('c'));
        }
    }
}