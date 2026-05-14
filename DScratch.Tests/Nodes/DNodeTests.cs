namespace DScratch.Tests.Nodes;

public class DNodeTests
{
    [Test]
    public void InsertChild_AddNodeAsFirstChild_WhenParentHasNoChildYet()
    {
        // Arrange
        var parent = new TestNode("1", null, null, null);
        var insert = new TestNode("2", null, null, null);
        
        // Act
        parent.InsertChild(insert);

        // Assert
        Assert.That(parent.FirstChild?.Id, Is.EqualTo(insert.Id));
    }
    
    [Test]
    public void InsertChild_AddNodeAsFirstChild_WhenNodeToInsertHasOriginNull()
    {
        // Arrange
        var parent = new TestNode("1", null, null, null);
        var node = new TestNode("2", null, null, null);
        parent.FirstChild = node;
        var insert = new TestNode("3", null, node, null);
        
        // Act
        parent.InsertChild(insert);

        // Assert
        Assert.That(parent.FirstChild.Id, Is.EqualTo(insert.Id));
    }
    
    [Test]
    public void InsertChild_AddNodeAfterOrigin_WhenNodeToInsertHasOrigin()
    {
        // Arrange
        var parent = new TestNode("1", null, null, null);
        var node = new TestNode("2", null, null, null);
        parent.FirstChild = node;
        var insert = new TestNode("3", node, null, null);
        
        // Act
        parent.InsertChild(insert);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.FirstChild.Id, Is.EqualTo(node.Id));
            Assert.That(parent.FirstChild.RightOrigin?.Id, Is.EqualTo(insert.Id));
        }
    }
    
    [Test]
    public void InsertChild_AddNodeAfterOrigin_AndBeforeRightOrigin()
    {
        // Arrange
        var parent = new TestNode("1", null, null, null);
        var node = new TestNode("2", null, null, null);
        parent.FirstChild = node;
        var node2 = new TestNode("3", node, null, null);
        node.RightOrigin = node2;

        var insert = new TestNode("4", node, node2, null);
        
        // Act
        parent.InsertChild(insert);
        
        // Assert
        var child1 = parent.FirstChild;
        var child2 = child1.RightOrigin;
        var child3 = child2?.RightOrigin;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(child1.Id, Is.EqualTo(node.Id));
            Assert.That(child1.RightOrigin?.Id, Is.EqualTo(insert.Id));
            
            Assert.That(child2?.Id, Is.EqualTo(insert.Id));
            Assert.That(child2.Origin?.Id, Is.EqualTo(node.Id));
            Assert.That(child2.RightOrigin?.Id, Is.EqualTo(node2.Id));
            
            Assert.That(child3?.Id, Is.EqualTo(node2.Id));
            Assert.That(child3.Origin?.Id, Is.EqualTo(insert.Id));
        }
    }
}