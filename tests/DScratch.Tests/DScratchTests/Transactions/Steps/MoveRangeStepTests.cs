using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using NUnit.Framework.Constraints;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class MoveRangeStepTests
{
    private TreeBuilder builder;
    private TestTransactionFake transactionDummy;

    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        transactionDummy = new TestTransactionFake(builder.IdGenerator);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent()
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
        var step = new MoveRangeStep(text2.Id, text5.Id, newParent, newSibling.LastId);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text1, text5]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(newSibling));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(text2));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(text4));
            Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.Null);

            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(text2.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text2.TextContent, Is.EqualTo("b"));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2.Id));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text3.TextContent, Is.EqualTo("c"));
        
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3.Id));
            Assert.That(text4.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text4.TextContent, Is.EqualTo("d"));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
            
        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text2, 
            text3,
            text4
        ]));

        AssertSteps(result, 3);
    }

    [Test]
    public void GivenNode_IsMovedToNewParent_WithNoEnd()
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
        var step = new MoveRangeStep(text3.Id, null, newParent, newSibling.LastId);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text1, text2]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(newSibling));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(text4));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(text5));
            Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.Null);
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text3.TextContent, Is.EqualTo("c"));
            
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3.Id));
            Assert.That(text4.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text4.TextContent, Is.EqualTo("d"));
        
            Assert.That(text5.Parent, Is.EqualTo(newParent));
            Assert.That(text5.Origin, Is.EqualTo(text4.Id));
            Assert.That(text5.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text5.TextContent, Is.EqualTo("e"));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
            
        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text3,
            text4,
            text5
        ]));
        
        AssertSteps(result, 3);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_WithNoStart()
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
        var step = new MoveRangeStep(null, text4.Id, newParent, newSibling.LastId);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text4, text5]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(newSibling));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(text1));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(text2));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.Null);
            
            Assert.That(text1.Parent, Is.EqualTo(newParent));
            Assert.That(text1.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(text1.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text1.TextContent, Is.EqualTo("a"));
        
            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.EqualTo(text1.Id));
            Assert.That(text2.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text2.TextContent, Is.EqualTo("b"));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2.Id));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling2.Id));
            Assert.That(text3.TextContent, Is.EqualTo("c"));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(newSibling.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
        
        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text1,
            text2,
            text3
        ]));
        
        AssertSteps(result, 3);
    }
    
        [Test]
    public void GivenNode_IsMovedToNewParent_NoTargetSibling()
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
        var step = new MoveRangeStep(text2.Id, text5.Id, newParent, null);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text1, text5]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(text2));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(text4));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(sibling1));
            Assert.That(newParent.ChildNodes[4], Is.EqualTo(sibling2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.Null);
            Assert.That(text2.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text2.TextContent, Is.EqualTo("b"));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2.Id));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text3.TextContent, Is.EqualTo("c"));
        
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3.Id));
            Assert.That(text4.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text4.TextContent, Is.EqualTo("d"));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text2,
            text3,
            text4
        ]));
        
        AssertSteps(result, 3);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_WithNoEnd_NoTargetSibling()
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
        var step = new MoveRangeStep(text3.Id, null, newParent, null);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text1, text2, text4]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(text5));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(sibling1));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(sibling2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.Null);
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text3.TextContent, Is.EqualTo("c"));
        
            Assert.That(text5.Parent, Is.EqualTo(newParent));
            Assert.That(text5.Origin, Is.EqualTo(text3.Id));
            Assert.That(text5.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text5.TextContent, Is.EqualTo("e"));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
        
        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text3,
            text5
        ]));
        
        AssertSteps(result, 2);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_WithNoEnd_AndTargetSibling()
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
        var step = new MoveRangeStep(text3.Id, null, newParent, sibling2.LastId);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text1, text2, text4]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(sibling1));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(sibling2));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(text5));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(sibling2.Id));
            Assert.That(text3.RightOrigin, Is.Null);
            Assert.That(text3.TextContent, Is.EqualTo("c"));
        
            Assert.That(text5.Parent, Is.EqualTo(newParent));
            Assert.That(text5.Origin, Is.EqualTo(text3.Id));
            Assert.That(text5.RightOrigin, Is.Null);
            Assert.That(text5.TextContent, Is.EqualTo("e"));
        }
        
        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text3,
            text5
        ]));
        
        AssertSteps(result, 2);
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_WithNoStart_NoTargetSibling()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text2 = null!;
        TextNode text3 = null!;
        TextNode text4 = null!;
        TextNode text5 = null!;
        TextNode sibling1 = null!;
        TextNode sibling2 = null!;
        
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
        var step = new MoveRangeStep(null, text4.Id, newParent, null);
        var result = step.Execute(transactionDummy, builder.Document);

        // Assert
        Assert.That(parent.ChildNodes, Is.EquivalentTo([text2, text4, text5]));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0], Is.EqualTo(text1));
            Assert.That(newParent.ChildNodes[1], Is.EqualTo(text3));
            Assert.That(newParent.ChildNodes[2], Is.EqualTo(sibling1));
            Assert.That(newParent.ChildNodes[3], Is.EqualTo(sibling2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(newParent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text1.TextContent, Is.EqualTo("a"));
        
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text1.Id));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling1.Id));
            Assert.That(text3.TextContent, Is.EqualTo("c"));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.Null);
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1.Id));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            text1,
            text3
        ]));
        
        AssertSteps(result, 2);
    }

    [Test]
    public void GivenNode_IsSplitNodeMovedToNewParent_AndTargetSiblingIsSplit()
    {
        // Arrange
        TextNode text = null!;
        TextNode sibling = null!;
        
        var parent = builder.TestInlineElementNode(t =>
        {
            text = t.Text("abcd");
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            sibling = t.Text("12");
        });
        
        // Act
        var step = new MoveRangeStep(
            start: new NodeId(text.Id.Client, text.Id.Clock + 1), 
            end: new NodeId(text.Id.Client, text.Id.Clock + 3), 
            targetParent: newParent, 
            targetOriginId: sibling.Id);
        
        var result = step.Execute(transactionDummy, builder.Document);
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
        Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("d"));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)newParent.ChildNodes[0]).TextContent, Is.EqualTo("1"));
            Assert.That(((TextNode)newParent.ChildNodes[1]).TextContent, Is.EqualTo("bc"));
            Assert.That(((TextNode)newParent.ChildNodes[2]).TextContent, Is.EqualTo("2"));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[0].Origin, Is.Null);
            Assert.That(newParent.ChildNodes[0].RightOrigin, Is.EqualTo(newParent.ChildNodes[2].Id));
            
            Assert.That(newParent.ChildNodes[1].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[1].Origin, Is.EqualTo(newParent.ChildNodes[0].Id));
            Assert.That(newParent.ChildNodes[1].RightOrigin, Is.EqualTo(newParent.ChildNodes[2].Id));
            
            Assert.That(newParent.ChildNodes[2].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[2].Origin, Is.EqualTo(newParent.ChildNodes[0].Id));
            Assert.That(newParent.ChildNodes[2].RightOrigin, Is.Null);
        }

        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            newParent.ChildNodes[0],
            newParent.ChildNodes[2],
            text,
            parent.ChildNodes[1],
            newParent.ChildNodes[1]
        ]));
        
        AssertSteps(result, 1, [
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.SplitTextTypes
        ]);
    }
    
    [Test]
    public void GivenNodes_AreSplitAndMovedToNewParent_AndTargetSiblingIsSplit()
    {
        // Arrange
        TextNode text1 = null!;
        TextNode text3 = null!;
        TextNode sibling = null!;
        
        var parent = builder.TestInlineElementNode(t =>
        {
            text1 = t.Text("ab");
            t.Text("cd");
            text3 = t.Text("ef");
        });
        
        var newParent = builder.TestInlineElementNode(t =>
        {
            sibling = t.Text("12");
        });
        
        // Act
        var step = new MoveRangeStep(
            start: new NodeId(text1.Id.Client, text1.Id.Clock + 1), 
            end: text3.LastId,
            targetParent: newParent, 
            targetOriginId: sibling.Id);
        
        var result = step.Execute(transactionDummy, builder.Document);
        
        builder.Print();
        
        // Assert
        Assert.That(parent.ChildNodes, Has.Count.EqualTo(2));
        Assert.That(((TextNode)parent.ChildNodes[0]).TextContent, Is.EqualTo("a"));
        Assert.That(((TextNode)parent.ChildNodes[1]).TextContent, Is.EqualTo("f"));
        
        Assert.That(newParent.ChildNodes, Has.Count.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((TextNode)newParent.ChildNodes[0]).TextContent, Is.EqualTo("1"));
            Assert.That(((TextNode)newParent.ChildNodes[1]).TextContent, Is.EqualTo("b"));
            Assert.That(((TextNode)newParent.ChildNodes[2]).TextContent, Is.EqualTo("cd"));
            Assert.That(((TextNode)newParent.ChildNodes[3]).TextContent, Is.EqualTo("e"));
            Assert.That(((TextNode)newParent.ChildNodes[4]).TextContent, Is.EqualTo("2"));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newParent.ChildNodes[0].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[0].Origin, Is.Null);
            Assert.That(newParent.ChildNodes[0].RightOrigin, Is.EqualTo(newParent.ChildNodes[4].Id));
        
            Assert.That(newParent.ChildNodes[1].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[1].Origin, Is.EqualTo(newParent.ChildNodes[0].LastId));
            Assert.That(newParent.ChildNodes[1].RightOrigin, Is.EqualTo(newParent.ChildNodes[4].Id));
            
            Assert.That(newParent.ChildNodes[2].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[2].Origin, Is.EqualTo(newParent.ChildNodes[1].LastId));
            Assert.That(newParent.ChildNodes[2].RightOrigin, Is.EqualTo(newParent.ChildNodes[4].Id));
            
            Assert.That(newParent.ChildNodes[3].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[3].Origin, Is.EqualTo(newParent.ChildNodes[2].LastId));
            Assert.That(newParent.ChildNodes[3].RightOrigin, Is.EqualTo(newParent.ChildNodes[4].Id));
            
            Assert.That(newParent.ChildNodes[4].Parent, Is.EqualTo(newParent));
            Assert.That(newParent.ChildNodes[4].Origin, Is.EqualTo(newParent.ChildNodes[0].LastId));
            Assert.That(newParent.ChildNodes[4].RightOrigin, Is.Null);
        }

        Assert.That(transactionDummy.ChangedNodes, Is.EquivalentTo([
            // Sibling Split
            newParent.ChildNodes[0],
            newParent.ChildNodes[4],
            // Start Split
            parent.ChildNodes[0],
            newParent.ChildNodes[1],
            // End Split
            newParent.ChildNodes[3],
            parent.ChildNodes[1],
            // Moves
            newParent.ChildNodes[2]
        ]));
        
        AssertSteps(result, 3, [
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.SplitTextTypes,
            ..AssertStepDiffHelper.SplitTextTypes
        ]);
    }
    
    private static void AssertSteps(IReadOnlyList<StepDiff?> result, int elementsMovedCount, IEnumerable<IResolveConstraint>? additionalAsserts = null)
    {
        var steps = AssertStepDiffHelper.Repeat<StepDiff.MoveDiff>(elementsMovedCount);
        if (additionalAsserts is not null) steps = [..additionalAsserts, ..steps];
        AssertHelper.ThatStepsEqualTo(result, expected: [.. steps]);
    }
}