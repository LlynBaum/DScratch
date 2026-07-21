using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Nodes;

public class DNodeTests
{
    [Test]
    public void ActiveChildNodes_ReturnOnlyNonDeletedChildNodes()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(t =>
        {
            t.TestNode();
            t.TestNode().Delete();
            t.TestNode();
        });
        
        // Act
        var activeChildNodes = builder.FirstChild.ActiveChildNodes.Select(c => c.Id.Value).ToList();
        
        // Assert
        Assert.That(activeChildNodes, Is.EquivalentTo(["Test-1", "Test-3"]));
    }
    
    [Test]
    public void FirstAndLastChild_ReturnExpectedNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(t =>
        {
            t.TestNode();
            t.TestNode();
            t.TestNode();
        });
        
        // Act
        var first = builder.FirstChild.FirstChild;
        var last = builder.FirstChild.LastChild;
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(first?.Id.Clock, Is.EqualTo(1));
            Assert.That(last?.Id.Clock, Is.EqualTo(3));
        });
    }
    
    [Test]
    public void FirstChild_ReturnExpectedNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(t =>
        {
            t.TestNode().Delete();
            t.TestNode();
            t.TestNode();
        });
        
        // Act
        var first = builder.FirstChild.FirstChild;
        
        // Assert
        Assert.That(first?.Id.Clock, Is.EqualTo(2));
    }
    
    [Test]
    public void LastChild_ReturnExpectedNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        builder.TestNode(t =>
        {
            t.TestNode();
            t.TestNode();
            t.TestNode().Delete();
        });
        
        // Act
        var last = builder.FirstChild.LastChild;
        
        // Assert
        Assert.That(last?.Id.Clock, Is.EqualTo(2));
    }
    
    [Test]
    public void Delete_MarksItselfAndAllChildNodes_AsIsDeleted()
    {
        // Arrange
        var builder = new TreeBuilder();
        var nodes = new DNode[3];
        nodes[0] = builder.TestNode(t =>
        {
            nodes[1] = t.TestNode(t2 =>
            {
                nodes[2] = t2.TestNode();
            });
        });
        
        // Act
        nodes[0].Delete();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(nodes[0].IsDeleted, Is.True);
            Assert.That(nodes[1].IsDeleted, Is.True);
            Assert.That(nodes[2].IsDeleted, Is.True);
        });
    }
    
    [Test]
    public void Remove_RemovesNodeFromTree()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node = null!;
        DNode node2 = null!;
        DNode node3 = null!;

        var parent = builder.TestNode(t =>
        {
            node = t.TestNode();
            node2 = t.TestNode();
            node3 = t.TestNode();
        });
        
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
        
        var insertBuilder = new TreeBuilder();
        var insert = insertBuilder.TestNode();
        insert.Origin = null;
        insert.RightOrigin = node;
        
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
        
        var insertBuilder = new TreeBuilder();
        var insert = insertBuilder.TestNode();
        insert.Origin = node;
        insert.RightOrigin = null;
        
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

        var insertBuilder = new TreeBuilder();
        var insert = insertBuilder.TestNode();
        insert.Origin = node;
        insert.RightOrigin = node2;
        
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
        var builder = new TreeBuilder();
        DNode node2 = null!;
        var parent = builder.TestNode(t =>
        {
            t.TestNode();
            node2 = t.TestNode();
        });
        
        // Act
        var result = parent.IndexOf(node2);
        
        // Assert
        Assert.That(result, Is.EqualTo(1));
    }
    
    [Test]
    public void IndexOf_ReturnsNegativeOne_WhenChildIsDeleted()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node2 = null!;
        var parent = builder.TestNode(t =>
        {
            t.TestNode();
            node2 = t.TestNode();
        });
        
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
        var builder = new TreeBuilder();
        var parent = builder.TestNode(t =>
        {
            t.TestNode();
        });
        
        var insertBuilder = new TreeBuilder();
        var node2 = insertBuilder.TestNode();
        
        // Act
        var result = parent.IndexOf(node2);
        
        // Assert
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void ChildAt_ReturnsExpectedChild()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node2 = null!;
        var parent = builder.TestNode(t =>
        {
            t.TestNode();
            node2 = t.TestNode();
        });
        
        // Act
        var result = parent.ChildAt(1);
        
        // Assert
        Assert.That(result, Is.EqualTo(node2));
    }
    
    [Test]
    public void ChildAt_ReturnsNull_WhenIndexIsOutOfBounds()
    {
        // Arrange
        var builder = new TreeBuilder();
        var parent = builder.TestNode(t =>
        {
            t.TestNode();
            t.TestNode();
        });
        
        // Act
        var result = parent.ChildAt(2);
        
        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void ChildAt_ReturnsNull_WhenIndexIsBelowZero()
    {
        // Arrange
        var builder = new TreeBuilder();
        var parent = builder.TestNode(t =>
        {
            t.TestNode();
            t.TestNode();
        });
        
        // Act
        var result = parent.ChildAt(-1);
        
        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void SetMark_AddMarkToSet()
    {
        // Arrange
        var testNode = new TestNode(new NodeId("Test", 1), null, null);
        
        // Act
        testNode.SetMark(new Mark(MarkKey.Bold, "Ka"));
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.EqualTo(1));
        Assert.That(testNode.Marks.Single().Key, Is.EqualTo(MarkKey.Bold));
    }
    
    [Test]
    public void SetMark_OverridesExisting()
    {
        // Arrange
        var testNode = new TestNode(new NodeId("Test", 1), null, null);
        testNode.SetMark(new Mark(MarkKey.Color, "0"));
        
        // Act
        testNode.SetMark(new Mark(MarkKey.Color, "1"));
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.EqualTo(1));
        Assert.That(testNode.Marks.Single().Key, Is.EqualTo(MarkKey.Color));
        Assert.That(testNode.Marks.Single().Value, Is.EqualTo("1"));
    }
    
    [Test]
    public void RemoveMark_RemovesMarkFromSet()
    {
        // Arrange
        var testNode = new TestNode(new NodeId("Test", 1), null, null);
        testNode.SetMark(new Mark(MarkKey.Bold, "Ka"));
        
        // Act
        testNode.RemoveMark(MarkKey.Bold);
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.Zero);
    }
}