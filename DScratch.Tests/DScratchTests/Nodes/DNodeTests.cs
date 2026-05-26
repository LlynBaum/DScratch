using DScratch.Nodes;
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
        var activeChildNodes = builder.FirstChild.ActiveChildNodes.Select(c => c.Id).ToList();
        
        // Assert
        Assert.That(activeChildNodes, Is.EquivalentTo(["1", "3"]));
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
            Assert.That(first?.Id, Is.EqualTo("1"));
            Assert.That(last?.Id, Is.EqualTo("3"));
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
        Assert.That(first?.Id, Is.EqualTo("2"));
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
        Assert.That(last?.Id, Is.EqualTo("2"));
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
        
        var insert = new TestNode("insert", null, null);
        
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
        
        var insert = new TestNode("insert", null, null);
        
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
    public void GetPath_ReturnsExpectedPathToNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node = null!;
        builder.TestNode(t =>
        {
            t.TestNode(t2 =>
            {
                node = t2.TestNode();
            });
        });
        
        // Act
        var path = node.GetPath();
        
        // Assert
        Assert.That(path, Has.Length.EqualTo(4));
        Assert.That(path.Path, Is.EquivalentTo(["root", "0", "1", "2"]));
    }
    
    [Test]
    public void GetElementPath_ReturnsExpectedPathToNode()
    {
        // Arrange
        var builder = new TreeBuilder();
        DNode node = null!;
        builder.TestInlineElementNode(t =>
        {
            t.TestNode(t2 =>
            {
                t2.TestInlineElementNode(t3 =>
                {
                    node = t3.TestNode();
                });
            });
        });
        
        // Act
        var path = node.GetElementPath();
        
        // Assert
        Assert.That(path.Path, Is.EquivalentTo(["root", "0", "2"]));
    }
}