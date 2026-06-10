using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

public class DNodeFactoryTests
{
    [Test]
    public void Paragraph_ReturnsParagraphNode_WithNewId()
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var testNode = new ParagraphNode(new NodeId(), null, null);
        var testNode2 = new ParagraphNode(new NodeId(), null, null);
        var result = factory.Paragraph(testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.Clock, Is.EqualTo(0));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
        }
    }
    
    [Test]
    public void String_ReturnsTextNode_WithNewId()
    {
        var testNode = new TestNode(new NodeId(), null, null);
        var testNode2 = new TestNode(new NodeId(), null, null);

        var nodeIdGen = new TestNodeIdGenerator();
        var factory = new DNodeFactory(nodeIdGen);

        var result = factory.String("abc", testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.Clock, Is.EqualTo(0));
            Assert.That(result.TextContent, Is.EqualTo("abc"));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
            
            Assert.That(nodeIdGen.GetNextId().Clock, Is.EqualTo(3));
        }
    }
    
    [Test]
    public void String_ThrowsInvalidOperationException_WhenEmptyString()
    {
        var testNode = new TestNode(new NodeId(), null, null);
        var testNode2 = new TestNode(new NodeId(), null, null);

        var nodeIdGen = new TestNodeIdGenerator();
        var factory = new DNodeFactory(nodeIdGen);

        Assert.Throws<InvalidOperationException>(Act);
        return;
        
        void Act() => factory.String("", testNode, testNode2);
    }
}