using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using NUnit.Framework.Constraints;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class MoveRangeStepTests
{
    private TreeBuilder builder;
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        transactionFake = new TestTransactionFake(builder.IdGenerator);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode newSibling = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text3 = t.Text("c");
            text4 = t.Text("d"); 
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            newSibling = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(text2, text4, newParent, newSibling);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.False);
            Assert.That(text2.IsDeleted, Is.True);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.True);
            Assert.That(text5.IsDeleted, Is.False);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        Assert.That(newParent.ChildNodes[0], Is.EqualTo(newSibling));
        Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.Null);

            var node1 = (TextNode)newParent.ChildNodes[1];
            Assert.That(node1.Parent, Is.EqualTo(newParent));
            Assert.That(node1.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(node1.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node1.TextContent, Is.EqualTo("b"));
            
            var node2 = (TextNode)newParent.ChildNodes[2];
            Assert.That(node2.Parent, Is.EqualTo(newParent));
            Assert.That(node2.Origin, Is.EqualTo(newParent.ChildNodes[1].Id));
            Assert.That(node2.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node2.TextContent, Is.EqualTo("c"));
        
            var node3 = (TextNode)newParent.ChildNodes[3];
            Assert.That(node3.Parent, Is.EqualTo(newParent));
            Assert.That(node3.Origin, Is.EqualTo(newParent.ChildNodes[2].Id));
            Assert.That(node3.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node3.TextContent, Is.EqualTo("d"));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
            
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text2, 
            newParent.ChildNodes[1],
            text3,
            newParent.ChildNodes[2], 
            text4,
            newParent.ChildNodes[3]
        ]));

        AssertSteps(result, 3);
    }

    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoEnd()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode newSibling = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text3 = t.Text("c");
            text4 = t.Text("d"); 
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            newSibling = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(text3, null, newParent, newSibling);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.False);
            Assert.That(text2.IsDeleted, Is.False);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.True);
            Assert.That(text5.IsDeleted, Is.True);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        Assert.That(newParent.ChildNodes[0], Is.EqualTo(newSibling));
        Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.Null);
            
            var node2 = (TextNode)newParent.ChildNodes[1];
            Assert.That(node2.Parent, Is.EqualTo(newParent));
            Assert.That(node2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(node2.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node2.TextContent, Is.EqualTo("c"));
            
            var node3 = (TextNode)newParent.ChildNodes[2];
            Assert.That(node3.Parent, Is.EqualTo(newParent));
            Assert.That(node3.Origin, Is.EqualTo(node2.Id));
            Assert.That(node3.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node3.TextContent, Is.EqualTo("d"));
        
            var node4 = (TextNode)newParent.ChildNodes[3];
            Assert.That(node4.Parent, Is.EqualTo(newParent));
            Assert.That(node4.Origin, Is.EqualTo(node3.Id));
            Assert.That(node4.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node4.TextContent, Is.EqualTo("e"));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
            
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text3,
            newParent.ChildNodes[1],
            text4, 
            newParent.ChildNodes[2], 
            text5, 
            newParent.ChildNodes[3]]));
        
        AssertSteps(result, 3);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoStart()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode newSibling = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text3 = t.Text("c");
            text4 = t.Text("d"); 
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            newSibling = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(null, text3, newParent, newSibling);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.True);
            Assert.That(text2.IsDeleted, Is.True);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.False);
            Assert.That(text5.IsDeleted, Is.False);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        Assert.That(newParent.ChildNodes[0], Is.EqualTo(newSibling));
        Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.Null);
            
            var node1 = (TextNode)newParent.ChildNodes[1];
            Assert.That(node1.Parent, Is.EqualTo(newParent));
            Assert.That(node1.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(node1.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node1.TextContent, Is.EqualTo("a"));
        
            var node2 = (TextNode)newParent.ChildNodes[2];
            Assert.That(node2.Parent, Is.EqualTo(newParent));
            Assert.That(node2.Origin, Is.EqualTo(node1.Id));
            Assert.That(node2.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node2.TextContent, Is.EqualTo("b"));
            
            var node3 = (TextNode)newParent.ChildNodes[3];
            Assert.That(node3.Parent, Is.EqualTo(newParent));
            Assert.That(node3.Origin, Is.EqualTo(node2.Id));
            Assert.That(node3.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(node3.TextContent, Is.EqualTo("c"));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
        
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text1,
            newParent.ChildNodes[1],
            text2, 
            newParent.ChildNodes[2],
            text3, 
            newParent.ChildNodes[3]
        ]));
        
        AssertSteps(result, 3);
    }
    
        [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_NoTargetSibling()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode sibling1 = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text3 = t.Text("c");
            text4 = t.Text("d"); 
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            sibling1 = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(text2, text4, newParent, null);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.False);
            Assert.That(text2.IsDeleted, Is.True);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.True);
            Assert.That(text5.IsDeleted, Is.False);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        Assert.That(newParent.ChildNodes[3], Is.EqualTo(sibling1));
        Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));

        using (Assert.EnterMultipleScope())
        {
            var node1 = (TextNode)newParent.ChildNodes[0];
            Assert.That(node1.Parent, Is.EqualTo(newParent));
            Assert.That(node1.Origin, Is.Null);
            Assert.That(node1.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node1.TextContent, Is.EqualTo("b"));
            
            var node2 = (TextNode)newParent.ChildNodes[1];
            Assert.That(node2.Parent, Is.EqualTo(newParent));
            Assert.That(node2.Origin, Is.EqualTo(node1.Id));
            Assert.That(node2.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node2.TextContent, Is.EqualTo("c"));
        
            var node3 = (TextNode)newParent.ChildNodes[2];
            Assert.That(node3.Parent, Is.EqualTo(newParent));
            Assert.That(node3.Origin, Is.EqualTo(node2.Id));
            Assert.That(node3.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node3.TextContent, Is.EqualTo("d"));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text2,
            newParent.ChildNodes[0],
            text3, 
            newParent.ChildNodes[1], 
            text4, 
            newParent.ChildNodes[2]]));
        
        AssertSteps(result, 3);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoEnd_NoTargetSibling()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode sibling1 = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text3 = t.Text("c");
            text4 = t.Text("d");
            text4.Delete();
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            sibling1 = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(text3, null, newParent, null);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.False);
            Assert.That(text2.IsDeleted, Is.False);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.True);
            Assert.That(text5.IsDeleted, Is.True);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(4));
        Assert.That(newParent.ChildNodes[2], Is.EqualTo(sibling1));
        Assert.That(newParent.ChildNodes[3], Is.EqualTo(sibling2));

        using (Assert.EnterMultipleScope())
        {
            var node0 = (TextNode)newParent.ChildNodes[0];
            Assert.That(node0.Parent, Is.EqualTo(newParent));
            Assert.That(node0.Origin, Is.Null);
            Assert.That(node0.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node0.TextContent, Is.EqualTo("c"));
        
            var node1 = (TextNode)newParent.ChildNodes[1];
            Assert.That(node1.Parent, Is.EqualTo(newParent));
            Assert.That(node1.Origin, Is.EqualTo(node0.Id));
            Assert.That(node1.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node1.TextContent, Is.EqualTo("e"));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
        
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text3,
            newParent.ChildNodes[0],
            text5, 
            newParent.ChildNodes[1]
        ]));
        
        AssertSteps(result, 2);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoEnd_AndTargetSibling()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode sibling1 = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text3 = t.Text("c");
            text4 = t.Text("d");
            text4.Delete();
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            sibling1 = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(text3, null, newParent, sibling2);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.False);
            Assert.That(text2.IsDeleted, Is.False);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.True);
            Assert.That(text5.IsDeleted, Is.True);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(4));
        Assert.That(newParent.ChildNodes[0], Is.EqualTo(sibling1));
        Assert.That(newParent.ChildNodes[1], Is.EqualTo(sibling2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
            
            var node3 = (TextNode)newParent.ChildNodes[2];
            Assert.That(node3.Parent, Is.EqualTo(newParent));
            Assert.That(node3.Origin, Is.EqualTo(sibling2.Id));
            Assert.That(node3.RightOrigin, Is.Null);
            Assert.That(node3.TextContent, Is.EqualTo("c"));
        
            var node4 = (TextNode)newParent.ChildNodes[3];
            Assert.That(node4.Parent, Is.EqualTo(newParent));
            Assert.That(node4.Origin, Is.EqualTo(node3.Id));
            Assert.That(node4.RightOrigin, Is.Null);
            Assert.That(node4.TextContent, Is.EqualTo("e"));
        }
        
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text3,
            newParent.ChildNodes[2],
            text5, 
            newParent.ChildNodes[3]
        ]));
        
        AssertSteps(result, 2);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoStart_NoTargetSibling()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode sibling1 = null!;
        TextNode sibling2 = null!;
            
        // 0: Parent Element
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("a");
            text2 = t.Text("b");
            text2.Delete();
            text3 = t.Text("c");
            text4 = t.Text("d"); 
            text5 = t.Text("e"); 
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            sibling1 = t.Text("1");
            sibling2 = t.Text("2");
        });
        
        // Act
        var step = new MoveRangeStep(null, text3, newParent, null);
        var result = step.Execute(transactionFake, builder.CreateDocument());

        // Assert
        AssertUnchanged(parent);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.IsDeleted, Is.True);
            Assert.That(text2.IsDeleted, Is.True);
            Assert.That(text3.IsDeleted, Is.True);
            Assert.That(text4.IsDeleted, Is.False);
            Assert.That(text5.IsDeleted, Is.False);
        }
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(4));
        Assert.That(newParent.ChildNodes[2], Is.EqualTo(sibling1));
        Assert.That(newParent.ChildNodes[3], Is.EqualTo(sibling2));

        using (Assert.EnterMultipleScope())
        {
            var node0 = (TextNode)newParent.ChildNodes[0];
            Assert.That(node0.Parent, Is.EqualTo(newParent));
            Assert.That(node0.Origin, Is.Null);
            Assert.That(node0.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node0.TextContent, Is.EqualTo("a"));
        
            var node1 = (TextNode)newParent.ChildNodes[1];
            Assert.That(node1.Parent, Is.EqualTo(newParent));
            Assert.That(node1.Origin, Is.EqualTo(node0.Id));
            Assert.That(node1.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(node1.TextContent, Is.EqualTo("c"));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            text1,
            newParent.ChildNodes[0],
            text3, 
            newParent.ChildNodes[1]
        ]));
        
        AssertSteps(result, 2);
    }
    
    private static readonly IResolveConstraint[] OneMoveSteps = [
        Is.TypeOf<StepDiff.DeleteElementDiff>(),
        Is.TypeOf<StepDiff.InsertElementDiff>(),
        Is.TypeOf<StepDiff.InsertTextDiff>(),
        Is.TypeOf<StepDiff.UpdateMarksDiff>()
    ];
    
    private static void AssertSteps(IReadOnlyList<StepDiff?> result, int elementsMovedCount)
    {
        var steps = Enumerable.Range(0, elementsMovedCount).SelectMany(_ => OneMoveSteps);
        AssertHelper.ThatStepsEqualTo(result, expected: [.. steps]);
    }

    private static void AssertUnchanged(DNode parent)
    {
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            var node0 = parent.ChildNodes[0];
            Assert.That(node0.Parent, Is.EqualTo(parent));
            Assert.That(node0.Origin, Is.Null);
            Assert.That(node0.RightOrigin, Is.Null);
            
            var node1 = parent.ChildNodes[1];
            Assert.That(node1.Parent, Is.EqualTo(parent));
            Assert.That(node1.Origin, Is.EqualTo(node0.Id));
            Assert.That(node1.RightOrigin, Is.Null);
            
            var node2 = parent.ChildNodes[2];
            Assert.That(node2.Parent, Is.EqualTo(parent));
            Assert.That(node2.Origin, Is.EqualTo(node1.Id));
            Assert.That(node2.RightOrigin, Is.Null);
        
            var node3 = parent.ChildNodes[3];
            Assert.That(node3.Parent, Is.EqualTo(parent));
            Assert.That(node3.Origin, Is.EqualTo(node2.Id));
            Assert.That(node3.RightOrigin, Is.Null);
        
            var node4 = parent.ChildNodes[4];
            Assert.That(node4.Parent, Is.EqualTo(parent));
            Assert.That(node4.Origin, Is.EqualTo(node3.Id));
            Assert.That(node4.RightOrigin, Is.Null);
        }
    }
}