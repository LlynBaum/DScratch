namespace DScratch.Tests.DScratchTests;

public class NodeIdTests
{
    [Test]
    public void Value_ReturnsCorrectNodeIdValue()
    {
        var nodeId = new NodeId("Test", 1);
        Assert.That(nodeId.Value, Is.EqualTo("Test-1"));
        Assert.That(nodeId.Clock, Is.EqualTo(1));
        Assert.That(nodeId.Client, Is.EqualTo("Test"));
    }
    
    [Test]
    public void FromString_ConstructsExpectedNodeId()
    {
        var path = NodeId.FromString("Test-1");
        Assert.That(path.Client, Is.EqualTo("Test"));
        Assert.That(path.Clock, Is.EqualTo(1));
    }
    
    [Test]
    public void Root_ReturnsRootId()
    {
        var nodeId = NodeId.Root;
        Assert.That(nodeId.Value, Is.EqualTo("Root"));
        Assert.That(nodeId.Client, Is.EqualTo("Root"));
    }
}