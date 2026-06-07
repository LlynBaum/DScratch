using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

public class DNodeFactoryTests
{
    [Test]
    public void ReturnsParagraphNode_WithNewId()
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var testNode = new ParagraphNode(new NodeId(), null, null);
        var testNode2 = new ParagraphNode(new NodeId(), null, null);
        var result = factory.Paragraph(testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.IdValue, Is.EqualTo(0));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
        }
    }
    
    [Test]
    public void ReturnsTextNode_WithNewId()
    {
        var testNode = new TestNode(new NodeId(), null, null);
        var testNode2 = new TestNode(new NodeId(), null, null);

        var nodeIdGen = new TestNodeIdGenerator();
        var factory = new DNodeFactory(nodeIdGen);

        var result = factory.String("abc", testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.IdValue, Is.EqualTo(0));
            Assert.That(result.TextContent, Is.EqualTo("abc"));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
            
            Assert.That(nodeIdGen.GetNextId().IdValue, Is.EqualTo(4));
        }
    }
}