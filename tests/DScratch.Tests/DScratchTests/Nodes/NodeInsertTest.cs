using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class NodeInsertTest
{
      [Test]
    public void InsertChild_SetsParentToItSelf()
    {
        // Arrange
        var builder = new TreeBuilder();
        var parent = builder.TestNode();
        
        var insert = new TestNode(new NodeId(), null, null);
        
        // Act
        parent.InsertChild(insert);

        // Assert
        Assert.That(insert.Parent, Is.Not.Null);
        Assert.That(insert.Parent.Id, Is.EqualTo(parent.Id));
    }
    
    [Test]
    public void InsertChild_AddNodeAsFirstChild_WhenParentHasNoChildYet()
    {
        // Arrange
        var builder = new TreeBuilder();
        var parent = builder.TestNode();
        
        var insert = new TestNode(new NodeId(), null, null);
        
        // Act
        parent.InsertChild(insert);

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(1));
        Assert.That(parent.ChildNodes, Is.EquivalentTo([insert]));
    }
    
    [Test]
    public void InsertChild_AddNodeAsFirstChild_WhenNodeToInsertHasOriginNull()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node = null!;
        var parent = builder.TestNode(t =>
        {
            node = t.TestNode();
        });
        
        var insert = new TestNode(new NodeId(), null, node.Id);
        
        // Act
        parent.InsertChild(insert);

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        Assert.That(parent.ChildNodes, Is.EquivalentTo([insert, node]));
    }
    
    [Test]
    public void InsertChild_AddNodeAfterOrigin_WhenNodeToInsertHasOrigin()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node = null!;
        var parent = builder.TestNode(t =>
        {
            node = t.TestNode();
        });
        
        var insert = new TestNode(new NodeId(), node.Id, null);
        
        // Act
        parent.InsertChild(insert);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].Id, Is.EqualTo(node.Id));
            Assert.That(parent.ChildNodes[1].Id, Is.EqualTo(insert.Id));
        }
    }
    
    [Test]
    public void InsertChild_AddNodeAfterOrigin_AndBeforeRightOrigin()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node = null!;
        DNode node2 = null!;
        var parent = builder.TestNode(t =>
        {
            node = t.TestNode();
            node2 = t.TestNode();
        });

        var insert = new TestNode(new NodeId(), node.Id, node2.Id);
        
        // Act
        parent.InsertChild(insert);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            var child1 = parent.ChildNodes[0];
            Assert.That(child1.Id, Is.EqualTo(node.Id));
            Assert.That(child1.Origin, Is.Null);
            Assert.That(child1.RightOrigin, Is.Null);
            
            var child2 = parent.ChildNodes[1];
            Assert.That(child2.Id, Is.EqualTo(insert.Id));
            Assert.That(child2.Origin, Is.EqualTo(node.Id));
            Assert.That(child2.RightOrigin, Is.EqualTo(node2.Id));
            
            var child3 = parent.ChildNodes[2];
            Assert.That(child3.Id, Is.EqualTo(node2.Id));
            Assert.That(child3.Origin, Is.EqualTo(node.Id));
            Assert.That(child3.RightOrigin, Is.Null);
        }
    }
}