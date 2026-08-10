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
    public void ParagraphFrom_ReturnsParagraphNode_AsCopyOfGivenNode()
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var id = new NodeId("Test", 1);
        var origin = TestNode.Empty();
        var rightOrigin = TestNode.Empty();
        var child = TestNode.Empty();
        
        var testNode = new ParagraphNode(id, origin, rightOrigin, [child]);
        var result = factory.ParagraphFrom(testNode);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Origin, Is.EqualTo(origin));
            Assert.That(result.RightOrigin, Is.EqualTo(rightOrigin));
            Assert.That(result.ChildNodes, Is.EquivalentTo([child]));
        }
    }
    
    [Test]
    [TestCase(HeadingLevel.Level1)]
    [TestCase(HeadingLevel.Level2)]
    [TestCase(HeadingLevel.Level3)]
    [TestCase(HeadingLevel.Level4)]
    [TestCase(HeadingLevel.Level5)]
    [TestCase(HeadingLevel.Level6)]
    public void Heading_ReturnsHeadingNode_WithNewId(HeadingLevel headingLevel)
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var testNode = new ParagraphNode(new NodeId(), null, null);
        var testNode2 = new ParagraphNode(new NodeId(), null, null);
        var result = factory.Heading(headingLevel, testNode, testNode2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.Clock, Is.EqualTo(0));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
            Assert.That(result.HeadingLevel, Is.EqualTo(headingLevel));
        }
    }
    
    [Test]
    public void HeadingFrom_ReturnsHeadingNode_AsCopyOfGivenNode()
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var id = new NodeId("Test", 1);
        var origin = TestNode.Empty();
        var rightOrigin = TestNode.Empty();
        var child = TestNode.Empty();
        
        var testNode = new HeadingNode(HeadingLevel.Level1, id, origin, rightOrigin, [child]);
        var result = factory.HeadingFrom(testNode, HeadingLevel.Level2);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Origin, Is.EqualTo(origin));
            Assert.That(result.RightOrigin, Is.EqualTo(rightOrigin));
            Assert.That(result.ChildNodes, Is.EquivalentTo([child]));
            Assert.That(result.HeadingLevel, Is.EqualTo(HeadingLevel.Level2));
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

    [Test]
    public void Link_ReturnsLinkNode_WithNewId()
    {
        var testNode = new TestNode(new NodeId(), null, null);
        var testNode2 = new TestNode(new NodeId(), null, null);
        
        var nodeIdGen = new TestNodeIdGenerator();
        var factory = new DNodeFactory(nodeIdGen);
        
        var result = factory.LinkNode(testNode, testNode2, "www.ggogle.com", "_blank");
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id.Clock, Is.EqualTo(0));
            Assert.That(result.Href, Is.EqualTo("www.ggogle.com"));
            Assert.That(result.Target, Is.EqualTo("_blank"));
            Assert.That(result.Origin, Is.EqualTo(testNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode2));
        }
    }
}