using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers.Common;

public class DeleteSelectionTests
{
    private TreeBuilder builder;
    private DTransaction transaction;

    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        transaction = new DTransaction(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator),
            nodeIdGenerator: builder.IdGenerator, 
            disableCleanUp: true);
    }

    [Test]
    public void DeletesExpectedNode_WithSameParent_SingleCharNodes()
    {
        // Arrange
        DNode start = null!;
        DNode end = null!;
        var parent = builder.TestBlockElementNode(t =>
        {
            t.Text("a");
            start = t.Text("b");
            t.Text("c");
            end = t.Text("d");
            t.Text("e");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 0, end.Id, 0);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
            Assert.That(parent.ChildNodes[2].IsDeleted, Is.True);
            Assert.That(parent.ChildNodes[3].IsDeleted, Is.False);
            Assert.That(parent.ChildNodes[4].IsDeleted, Is.False);
        }
    }
    
    [Test]
    public void DeletesExpectedNode_WithSameParent_OneTextNodes()
    {
        // Arrange
        DNode node = null!;
        var parent = builder.TestBlockElementNode(t =>
        {
            node = t.Text("abcd");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(node.Id, 1, node.Id, 3);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("bc"));
            
            Assert.That(parent.ChildNodes[2].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("d"));
        }
    }
    
    [Test]
    public void DeletesExpectedNode_WithSameParent_SingleCharOneTextNode()
    {
        // Arrange
        DNode start = null!;
        DNode end = null!;
        var parent = builder.TestBlockElementNode(t =>
        {
            start = t.Text("a");
            end = t.Text("b");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 0, end.Id, 0);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
        }
    }
    
    [Test]
    public void DeletesExpectedNode_WithSameParent_TwoCharsMidSplit()
    {
        // Arrange
        DNode start = null!;
        DNode end = null!;
        var parent = builder.TestBlockElementNode(t =>
        {
            start = t.Text("ab");
            end = t.Text("cd");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            
            Assert.That(parent.ChildNodes[2].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("c"));
            
            Assert.That(parent.ChildNodes[3].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("d"));
        }
    }
    
    [Test]
    public void DeletesExpectedNode_WithSameParent()
    {
        // Arrange
        DNode start = null!;
        DNode end = null!;
        var parent = builder.TestBlockElementNode(t =>
        {
            t.Text("a");
            start = t.Text("bc");
            t.Text("d");
            end = t.Text("ef");
            t.Text("g");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(7));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(parent.ChildNodes[1].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            
            Assert.That(parent.ChildNodes[2].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[2]).TextContent, Is.EqualTo("c"));
            
            Assert.That(parent.ChildNodes[3].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[3]).TextContent, Is.EqualTo("d"));
            
            Assert.That(parent.ChildNodes[4].IsDeleted, Is.True);
            Assert.That(((TextNode)parent.ChildNodes[4]).TextContent, Is.EqualTo("e"));
            
            Assert.That(parent.ChildNodes[5].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[5]).TextContent, Is.EqualTo("f"));
            
            Assert.That(parent.ChildNodes[6].IsDeleted, Is.False);
            Assert.That(((TextNode)parent.ChildNodes[6]).TextContent, Is.EqualTo("g"));
        }
    }
    
    [Test]
    public void DeletesExpectedNode_WithDifferentParent()
    {
        // Arrange
        DNode start = null!;
        DNode end = null!;
        var startParent = builder.TestBlockElementNode(t =>
        {
            t.Text("a");
            start = t.Text("bc");
            t.Text("d");
        });
        var midParent = builder.TestBlockElementNode(t =>
        {
            t.Text("e");
        });
        var endParent = builder.TestBlockElementNode(t =>
        {
            t.Text("f");
            end = t.Text("gh");
            t.Text("i");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 1, end.Id, 1);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(startParent.IsDeleted, Is.False);
            Assert.That(midParent.IsDeleted, Is.True);
            Assert.That(endParent.IsDeleted, Is.True);
            
            Assert.That(startParent.ChildNodes, Has.Count.EqualTo(6));
            Assert.That(midParent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(endParent.ChildNodes, Has.Count.EqualTo(2));
        }
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(startParent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(startParent.ChildNodes[1].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            
            Assert.That(startParent.ChildNodes[2].IsDeleted, Is.True);
            Assert.That(((TextNode)startParent.ChildNodes[2]).TextContent, Is.EqualTo("c"));
            
            Assert.That(startParent.ChildNodes[3].IsDeleted, Is.True);
            Assert.That(((TextNode)startParent.ChildNodes[3]).TextContent, Is.EqualTo("d"));
            
            Assert.That(startParent.ChildNodes[4].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[4]).TextContent, Is.EqualTo("h"));
            
            Assert.That(startParent.ChildNodes[5].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[5]).TextContent, Is.EqualTo("i"));
            
            Assert.That(endParent.ChildNodes[0].IsDeleted, Is.True);
            Assert.That(((TextNode)endParent.ChildNodes[0]).TextContent, Is.EqualTo("f"));
            
            Assert.That(endParent.ChildNodes[1].IsDeleted, Is.True);
            Assert.That(((TextNode)endParent.ChildNodes[1]).TextContent, Is.EqualTo("g"));
        }
    }
    
    [Test]
    public void DeletesExpectedNode_WithDifferentParent_Backwards()
    {
        // Arrange
        DNode start = null!;
        DNode end = null!;
        var startParent = builder.TestBlockElementNode(t =>
        {
            t.Text("a");
            start = t.Text("bc");
            t.Text("d");
        });
        var midParent = builder.TestBlockElementNode(t =>
        {
            t.Text("e");
        });
        var endParent = builder.TestBlockElementNode(t =>
        {
            t.Text("f");
            end = t.Text("gh");
            t.Text("i");
        });

        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(end.Id, 1, start.Id, 1, SelectionDirection.Backward);
        
        // Act
        DeleteSelection.Handle(keyPressInfo, transaction);
        transaction.Commit();

        // Assert
        Assert.That(builder.Root.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(startParent.IsDeleted, Is.False);
            Assert.That(midParent.IsDeleted, Is.True);
            Assert.That(endParent.IsDeleted, Is.True);
            
            Assert.That(startParent.ChildNodes, Has.Count.EqualTo(6));
            Assert.That(midParent.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(endParent.ChildNodes, Has.Count.EqualTo(2));
        }
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(startParent.ChildNodes[0].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
            
            Assert.That(startParent.ChildNodes[1].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            
            Assert.That(startParent.ChildNodes[2].IsDeleted, Is.True);
            Assert.That(((TextNode)startParent.ChildNodes[2]).TextContent, Is.EqualTo("c"));
            
            Assert.That(startParent.ChildNodes[3].IsDeleted, Is.True);
            Assert.That(((TextNode)startParent.ChildNodes[3]).TextContent, Is.EqualTo("d"));
            
            Assert.That(startParent.ChildNodes[4].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[4]).TextContent, Is.EqualTo("h"));
            
            Assert.That(startParent.ChildNodes[5].IsDeleted, Is.False);
            Assert.That(((TextNode)startParent.ChildNodes[5]).TextContent, Is.EqualTo("i"));
            
            Assert.That(endParent.ChildNodes[0].IsDeleted, Is.True);
            Assert.That(((TextNode)endParent.ChildNodes[0]).TextContent, Is.EqualTo("f"));
            
            Assert.That(endParent.ChildNodes[1].IsDeleted, Is.True);
            Assert.That(((TextNode)endParent.ChildNodes[1]).TextContent, Is.EqualTo("g"));
        }
    }
    
    // TODO: add support for Inline Elements
}