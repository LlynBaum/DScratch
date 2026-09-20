using DScratch.Marks;
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
        
        var insert = new TestNode(new NodeId(), null, node);
        
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
        
        var insert = new TestNode(new NodeId(), node, null);
        
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

        var insert = new TestNode(new NodeId(), node, node2);
        
        // Act
        parent.InsertChild(insert);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            var child1 = parent.ChildNodes[0];
            Assert.That(child1.Id, Is.EqualTo(node.Id));
            Assert.That(child1.RightOrigin?.Id, Is.Null);
            
            var child2 = parent.ChildNodes[1];
            Assert.That(child2.Id, Is.EqualTo(insert.Id));
            Assert.That(child2.Origin?.Id, Is.EqualTo(node.Id));
            Assert.That(child2.RightOrigin?.Id, Is.EqualTo(node2.Id));
            
            var child3 = parent.ChildNodes[2];
            Assert.That(child3.Id, Is.EqualTo(node2.Id));
            Assert.That(child3.Origin?.Id, Is.EqualTo(node.Id));
        }
    }

    [Test]
    public void NextSibling_ReturnsExpectedNodes()
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
        
        
        // Act & Assert
        var first = parent.FirstChild!;
        Assert.That(first, Is.EqualTo(node));
        
        var next = first.NextSibling();
        Assert.That(next, Is.EqualTo(node2));
        
        var next2 = next.NextSibling();
        Assert.That(next2, Is.Null);
    }
    
    [Test]
    public void PreviousSibling_ReturnsExpectedNodes()
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
        
        
        // Act & Assert
        var last = parent.LastChild!;
        Assert.That(last, Is.EqualTo(node2));
        
        var prev = last.PreviousSibling();
        Assert.That(prev, Is.EqualTo(node));
        
        var prev2 = prev.PreviousSibling();
        Assert.That(prev2, Is.Null);
    }
    
    [Test]
    public void SetMark_AddMarkToSet()
    {
        // Arrange
        var testNode = new TestNode(new NodeId("Test", 1), null, null);
        
        // Act
        testNode.SetMark(MarkKey.FontWeight, "Ka");
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.EqualTo(1));
        Assert.That(testNode.Marks.Single().Key, Is.EqualTo(MarkKey.FontWeight));
    }
    
    [Test]
    public void SetMark_OverridesExisting()
    {
        // Arrange
        var testNode = new TestNode(new NodeId("Test", 1), null, null);
        testNode.SetMark(MarkKey.Color, "0");
        
        // Act
        testNode.SetMark(MarkKey.Color, "1");
        
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
        testNode.SetMark(MarkKey.FontWeight, "Ka");
        
        // Act
        testNode.RemoveMark(MarkKey.FontWeight);
        
        // Assert
        Assert.That(testNode.Marks, Has.Count.Zero);
    }

    [Test]
    public void GetComputedMarks_ReturnsExpectedMarks()
    {
        // Arrange
        DNode node = null!;
        var builder = new TreeBuilder();
        var block = builder.TestBlockElementNode(t =>
        {
            var inline = t.TestInlineElementNode(tt =>
            {
                node = tt.Text("a");
                node.SetMark(MarkKey.FontWeight, "400");
            });
            
            inline.SetMark(MarkKey.Color, "#fff");
            inline.SetMark(MarkKey.FontWeight, "700");
        });
        block.SetMark(MarkKey.Color, "#abc");
        block.SetMark(MarkKey.FontStyle, "italic");
        
        // Act
        var result = node.GetComputedMarks();
        
        // Assert
        Assert.That(result, Is.EquivalentTo(new Dictionary<MarkKey, string>
        {
            { MarkKey.FontWeight, "400" },
            { MarkKey.FontStyle, "italic" },
            { MarkKey.Color, "#fff" },
        }));
    }
}