using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.DScratchTests.Transactions.Steps;

public class MoveRangeStepTests
{
    private TestTransactionFake transactionFake;

    [SetUp]
    public void SetUp()
    {
        transactionFake = new TestTransactionFake();
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(parent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(parent));
            Assert.That(text5.Origin, Is.EqualTo(text1));
            Assert.That(text5.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.EqualTo(text2));
            
            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.EqualTo(newSibling));
            Assert.That(text2.RightOrigin, Is.EqualTo(text3));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2));
            Assert.That(text3.RightOrigin, Is.EqualTo(text4));
        
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3));
            Assert.That(text4.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(text4));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }
            
        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text2, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text4, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoEnd()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(parent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(text2));
        
            Assert.That(text2.Parent, Is.EqualTo(parent));
            Assert.That(text2.Origin, Is.EqualTo(text1));
            Assert.That(text2.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.EqualTo(text3));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(newSibling));
            Assert.That(text3.RightOrigin, Is.EqualTo(text4));
            
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3));
            Assert.That(text4.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(newParent));
            Assert.That(text5.Origin, Is.EqualTo(text4));
            Assert.That(text5.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(text5));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text4, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text5, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoStart()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text4.Parent, Is.EqualTo(parent));
            Assert.That(text4.Origin, Is.Null);
            Assert.That(text4.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(parent));
            Assert.That(text5.Origin, Is.EqualTo(text4));
            Assert.That(text5.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newSibling.Parent, Is.EqualTo(newParent));
            Assert.That(newSibling.Origin, Is.Null);
            Assert.That(newSibling.RightOrigin, Is.EqualTo(text1));
            
            Assert.That(text1.Parent, Is.EqualTo(newParent));
            Assert.That(text1.Origin, Is.EqualTo(newSibling));
            Assert.That(text1.RightOrigin, Is.EqualTo(text2));
            
            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.EqualTo(text1));
            Assert.That(text2.RightOrigin, Is.EqualTo(text3));
        
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(text3));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text1, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text2, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_NoTargetSibling()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(parent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(parent));
            Assert.That(text5.Origin, Is.EqualTo(text1));
            Assert.That(text5.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.Null);
            Assert.That(text2.RightOrigin, Is.EqualTo(text3));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2));
            Assert.That(text3.RightOrigin, Is.EqualTo(text4));
        
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3));
            Assert.That(text4.RightOrigin, Is.EqualTo(sibling1));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.EqualTo(text4));
            Assert.That(sibling1.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text2, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text4, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoEnd_NoTargetSibling()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(parent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(text2));
        
            Assert.That(text2.Parent, Is.EqualTo(parent));
            Assert.That(text2.Origin, Is.EqualTo(text1));
            Assert.That(text2.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.Null);
            Assert.That(text3.RightOrigin, Is.EqualTo(text4));
            
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3));
            Assert.That(text4.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(newParent));
            Assert.That(text5.Origin, Is.EqualTo(text4));
            Assert.That(text5.RightOrigin, Is.EqualTo(sibling1));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.EqualTo(text5));
            Assert.That(sibling1.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text4, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text5, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoEnd_AndTargetSibling()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(parent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(text2));
        
            Assert.That(text2.Parent, Is.EqualTo(parent));
            Assert.That(text2.Origin, Is.EqualTo(text1));
            Assert.That(text2.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.Null);
            Assert.That(sibling1.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1));
            Assert.That(sibling2.RightOrigin, Is.EqualTo(text3));
            
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(sibling2));
            Assert.That(text3.RightOrigin, Is.EqualTo(text4));
            
            Assert.That(text4.Parent, Is.EqualTo(newParent));
            Assert.That(text4.Origin, Is.EqualTo(text3));
            Assert.That(text4.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(newParent));
            Assert.That(text5.Origin, Is.EqualTo(text4));
            Assert.That(text5.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text4, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text5, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
    
    [Test]
    public void GivenNode_IsMovedToNewParent_AndSiblingsAreUpdated_WithNoStart_NoTargetSibling()
    {
        // Arrange
        var builder = new TreeBuilder();
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
        step.Execute(transactionFake, null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(text4.Parent, Is.EqualTo(parent));
            Assert.That(text4.Origin, Is.Null);
            Assert.That(text4.RightOrigin, Is.EqualTo(text5));
        
            Assert.That(text5.Parent, Is.EqualTo(parent));
            Assert.That(text5.Origin, Is.EqualTo(text4));
            Assert.That(text5.RightOrigin, Is.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text1.Parent, Is.EqualTo(newParent));
            Assert.That(text1.Origin, Is.Null);
            Assert.That(text1.RightOrigin, Is.EqualTo(text2));
            
            Assert.That(text2.Parent, Is.EqualTo(newParent));
            Assert.That(text2.Origin, Is.EqualTo(text1));
            Assert.That(text2.RightOrigin, Is.EqualTo(text3));
        
            Assert.That(text3.Parent, Is.EqualTo(newParent));
            Assert.That(text3.Origin, Is.EqualTo(text2));
            Assert.That(text3.RightOrigin, Is.EqualTo(sibling1));
            
            Assert.That(sibling1.Parent, Is.EqualTo(newParent));
            Assert.That(sibling1.Origin, Is.EqualTo(text3));
            Assert.That(sibling1.RightOrigin, Is.EqualTo(sibling2));
            
            Assert.That(sibling2.Parent, Is.EqualTo(newParent));
            Assert.That(sibling2.Origin, Is.EqualTo(sibling1));
            Assert.That(sibling2.RightOrigin, Is.Null);
        }

        Assert.That(transactionFake.ChangedNodes, Is.EquivalentTo([
            new ModifiedNode(text1, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text2, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed),
            new ModifiedNode(text3, Modification.Delete),
            new ModifiedNode(newParent, Modification.Changed)
        ]));
    }
}