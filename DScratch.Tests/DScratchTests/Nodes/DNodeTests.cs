using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class DNodeTests
{
    [Test]
    public void ActiveChildNodes_ReturnOnlyNonDeletedChildNodes()
    {
        // Arrange
        var node1 = new TestNode("2", null, null);
        var node2 = new TestNode("3", null, null);
        var node3 = new TestNode("4", null, null);
        var parent = new TestNode("1", null, null, [node1, node2, node3]);
        node2.Delete();
        
        // Act
        var activeChildNodes = parent.ActiveChildNodes.Select(c => c.Id).ToList();
        
        // Assert
        Assert.That(activeChildNodes, Is.EquivalentTo(["2", "4"]));
    }
    
    [Test]
    public void FirstAndLastChild_ReturnExpectedNode()
    {
        // Arrange
        var node1 = new TestNode("2", null, null);
        var node2 = new TestNode("2", null, null);
        var node3 = new TestNode("2", null, null);
        var parent = new TestNode("1", null, null, [node1, node2, node3]);
        
        // Act
        var first = parent.FirstChild;
        var last = parent.LastChild;
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo(node1));
            Assert.That(last, Is.EqualTo(node3));
        });
    }
    
    [Test]
    public void FirstChild_ReturnExpectedNode()
    {
        // Arrange
        var node1 = new TestNode("2", null, null);
        var node2 = new TestNode("2", null, null);
        var node3 = new TestNode("2", null, null);
        var parent = new TestNode("1", null, null, [node1, node2, node3]);
        
        node1.Delete();
        
        // Act
        var first = parent.FirstChild;
        
        // Assert
        Assert.That(first, Is.EqualTo(node2));
    }
    
    [Test]
    public void LastChild_ReturnExpectedNode()
    {
        // Arrange
        var node1 = new TestNode("2", null, null);
        var node2 = new TestNode("2", null, null);
        var node3 = new TestNode("2", null, null);
        var parent = new TestNode("1", null, null, [node1, node2, node3]);
        
        node3.Delete();
        
        // Act
        var last = parent.LastChild;
        
        // Assert
        Assert.That(last, Is.EqualTo(node2));
    }
    
    [Test]
    public void Delete_MarksItselfAndAllChildNodes_AsIsDeleted()
    {
        // Arrange
        var node1 = new TestNode("2", null, null);
        var node2 = new TestNode("2", null, null, [node1]);
        var node3 = new TestNode("2", null, null, [node2]);
        
        // Act
        node3.Delete();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(node1.IsDeleted, Is.True);
            Assert.That(node2.IsDeleted, Is.True);
            Assert.That(node3.IsDeleted, Is.True);
        });
    }
    
    [Test]
    public void Remove_RemovesNodeFromTree()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        
        var node2 = new TestNode("3", node, null);
        node.RightOrigin = node2;
        
        var node3 = new TestNode("4", node2, null);
        node2.RightOrigin = node3;
        
        var parent = new TestNode("1", null, null, [node, node2, node3]);
        node.Parent = parent;
        node2.Parent = parent;
        node3.Parent = parent;
        
        // Act
        node2.Remove();
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        Assert.That(parent.ChildNodes, Is.EquivalentTo([node, node3]));
        Assert.That(node2.Origin, Is.Null);
        Assert.That(node2.RightOrigin, Is.Null);
        
        Assert.That(node.RightOrigin!.Id, Is.EqualTo(node3.Id));
        Assert.That(node3.Origin!.Id, Is.EqualTo(node.Id));
    }

    [Test]
    public void InsertChild_SetsParentToItSelf()
    {
        // Arrange
        var parent = new TestNode("1", null, null);
        var insert = new TestNode("2", null, null);
        
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
        var parent = new TestNode("1", null, null);
        var insert = new TestNode("2", null, null);
        
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
        var node = new TestNode("2", null, null);
        var parent = new TestNode("1", null, null, [node]);
        
        var insert = new TestNode("3", null, node);
        
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
        var node = new TestNode("2", null, null);
        var parent = new TestNode("1", null, null, [node]);
        
        var insert = new TestNode("3", node, null);
        
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
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", node, null);
        node.RightOrigin = node2;
        var parent = new TestNode("1", null, null, [node, node2]);

        var insert = new TestNode("4", node, node2);
        
        // Act
        parent.InsertChild(insert);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            var child1 = parent.ChildNodes[0];
            Assert.That(child1.Id, Is.EqualTo(node.Id));
            Assert.That(child1.RightOrigin?.Id, Is.EqualTo(insert.Id));
            
            var child2 = parent.ChildNodes[1];
            Assert.That(child2.Id, Is.EqualTo(insert.Id));
            Assert.That(child2.Origin?.Id, Is.EqualTo(node.Id));
            Assert.That(child2.RightOrigin?.Id, Is.EqualTo(node2.Id));
            
            var child3 = parent.ChildNodes[2];
            Assert.That(child3.Id, Is.EqualTo(node2.Id));
            Assert.That(child3.Origin?.Id, Is.EqualTo(insert.Id));
        }
    }

    [Test]
    public void IndexOf_ReturnsIndexOfGivenChildNode()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", node, null);
        node.RightOrigin = node2;
        var parent = new TestNode("1", null, null, [node, node2]);
        
        // Act
        var result = parent.IndexOf(node2);
        
        // Assert
        Assert.That(result, Is.EqualTo(1));
    }
    
    [Test]
    public void IndexOf_ReturnsNegativeOne_WhenChildIsDeleted()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", node, null);
        node.RightOrigin = node2;
        var parent = new TestNode("1", null, null, [node, node2]);
        
        node2.Delete();
        
        // Act
        var result = parent.IndexOf(node2);
        
        // Assert
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void IndexOf_ReturnsNegativeOne_WhenChildIsNotFound()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", null, null);
        var parent = new TestNode("1", null, null, [node]);
        
        // Act
        var result = parent.IndexOf(node2);
        
        // Assert
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void ChildAt_ReturnsExpectedChild()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", null, null);
        var parent = new TestNode("1", null, null, [node, node2]);
        
        // Act
        var result = parent.ChildAt(1);
        
        // Assert
        Assert.That(result, Is.EqualTo(node2));
    }
    
    [Test]
    public void ChildAt_ReturnsNull_WhenIndexIsOutOfBounds()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", null, null);
        var parent = new TestNode("1", null, null, [node, node2]);
        
        // Act
        var result = parent.ChildAt(2);
        
        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void ChildAt_ReturnsNull_WhenIndexIsBelowZero()
    {
        // Arrange
        var node = new TestNode("2", null, null);
        var node2 = new TestNode("3", null, null);
        var parent = new TestNode("1", null, null, [node, node2]);
        
        // Act
        var result = parent.ChildAt(-1);
        
        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetPath_ReturnsExpectedPathToNode()
    {
        // Arrange
        var node = new TestNode("3", null, null);
        var mid = new TestNode("2", null, null, [node]);
        node.Parent = mid;
        var parent = new TestNode("1", null, null, [mid]);
        mid.Parent = parent;
        
        // Act
        var path = node.GetPath();
        
        Assert.That(path, Has.Length.EqualTo(3));
        Assert.That(path.Path, Is.EquivalentTo(["1", "2", "3"]));
    }
    
    [Test]
    public void GetElementPath_ReturnsExpectedPathToNode()
    {
        // Arrange
        var node = new TestNode("4", null, null);
        var mid = new TestInlineElementNode("3", null, null, [node]);
        node.Parent = mid;
        var mid2 = new TestNode("2", null, null, [mid]);
        mid.Parent = mid2;
        var parent = new TestInlineElementNode("1", null, null, [mid2]);
        mid2.Parent = parent;
        
        // Act
        var path = node.GetElementPath();
        
        Assert.That(path, Has.Length.EqualTo(2));
        Assert.That(path.Path, Is.EquivalentTo(["1", "3"]));
    }
}