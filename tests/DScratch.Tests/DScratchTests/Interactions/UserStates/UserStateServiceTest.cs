using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Interactions.UserStates;

public class UserStateServiceTest
{
    private UserStateService service;

    [SetUp]
    public void SetUp()
    {
        service = new UserStateService();
    }

    [Test]
    public void AddPendingMark_AddsMarkToPendingMarks()
    {
        // Act
        service.AddPendingMark(new Mark(MarkKey.FontWeight));

        // Assert
        Assert.That(service.PendingMarks, Has.Count.EqualTo(1));
        Assert.That(service.PendingMarks.Single(), Is.EqualTo(new Mark(MarkKey.FontWeight)));
    }
    
    [Test]
    public void AddPendingMark_OverridesExistingPendingMarks()
    {
        // Arrange
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.Color, "a"));
        
        // Act
        service.AddPendingMark(new Mark(MarkKey.Color, "b"));

        // Assert
        Assert.That(service.PendingMarks, Has.Count.EqualTo(1));
        Assert.That(service.PendingMarks.Single(), Is.EqualTo(new Mark(MarkKey.Color, "b")));
    }

    [Test]
    public void RemovePendingMark_RemovesMarkFromPendingMarks()
    {
        // Arrange
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.FontWeight));
        
        // Act
        service.RemovePendingMark(new Mark(MarkKey.FontWeight));

        // Assert
        Assert.That(service.PendingMarks, Has.Count.Zero);
    }
    
    [Test]
    public void RemovePendingMark_AddsMarkToPendingRemovals_WhenMarkIsNotPending()
    {
        // Act
        service.RemovePendingMark(new Mark(MarkKey.FontWeight));

        // Assert
        Assert.That(service.PendingMarkRemovals, Has.Count.EqualTo(1));
        Assert.That(service.PendingMarkRemovals.Single(), Is.EqualTo(MarkKey.FontWeight));
    }
    
    [Test]
    public void UpdateState_MakesExpectedChanges_AndCallsOnStateChange()
    {
        // Arrange
        var isCalled = false;
        service.OnStateChange += () => isCalled = true;
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.FontWeight));
        ((HashSet<MarkKey>)service.PendingMarkRemovals).Add(MarkKey.FontStyle);

        var node = new TextNode(new NodeId(), null, null);
        node.SetMark(new Mark(MarkKey.FontStyle));
        
        // Act
        service.UpdateState(node);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isCalled, Is.True);
            Assert.That(service.PendingMarks, Has.Count.Zero);
            Assert.That(service.PendingMarkRemovals, Has.Count.Zero);
            Assert.That(service.ActiveMarks, Is.EquivalentTo([new Mark(MarkKey.FontStyle)]));
        }
    }
    
    [Test]
    public void UpdateState_DoesNotCopyMark_FromNonTextNode()
    {
        // Arrange
        var isCalled = false;
        service.OnStateChange += () => isCalled = true;
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.FontWeight));
        
        // Act
        service.UpdateState(TestNode.Empty());
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isCalled, Is.True);
            Assert.That(service.PendingMarks, Has.Count.Zero);
            Assert.That(service.ActiveMarks, Has.Count.Zero);
        }
    }
    
    [Test]
    public void PopPending_ReturnsPendingMarks_AndClearsPending()
    {
        // Arrange
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.FontWeight));
        
        // Act
        var marks = service.PopPending();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(marks, Is.EquivalentTo([new Mark(MarkKey.FontWeight)]));
            Assert.That(service.PendingMarks, Has.Count.Zero);
        }
    }
    
    [Test]
    public void PopPendingRemovals_ReturnsPendingRemovingMarks_AndClearsPending()
    {
        // Arrange
        ((HashSet<MarkKey>)service.PendingMarkRemovals).Add(MarkKey.FontWeight);
        
        // Act
        var marks = service.PopPendingRemovals();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(marks, Is.EquivalentTo([MarkKey.FontWeight]));
            Assert.That(service.PendingMarkRemovals, Has.Count.Zero);
        }
    }

    [Test]
    public void CheckMark_ReturnsTrue_WhenFoundAsPendingMark()
    {
        // Arrange
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.Color, "a"));
        
        // Act
        var found = service.CheckMark(MarkKey.Color, out var value);
        
        // Assert
        Assert.That(found, Is.True);
        Assert.That(value, Is.EqualTo("a"));
    }
    
    [Test]
    public void CheckMark_ReturnsTrue_WhenFoundAsActiveMark()
    {
        // Arrange
        ((HashSet<Mark>)service.ActiveMarks).Add(new Mark(MarkKey.Color, "a"));
        
        // Act
        var found = service.CheckMark(MarkKey.Color, out var value);
        
        // Assert
        Assert.That(found, Is.True);
        Assert.That(value, Is.EqualTo("a"));
    }
    
    [Test]
    public void CheckMark_ReturnsFalse_WhenNotFound()
    {
        // Act
        var found = service.CheckMark(MarkKey.Color, out var value);
        
        // Assert
        Assert.That(found, Is.False);
        Assert.That(value, Is.Null);
    }
    
    [Test]
    public void CheckMark_ReturnsFalse_WhenPendingMarkRemovalIsSet()
    {
        // Arrange
        ((HashSet<Mark>)service.ActiveMarks).Add(new Mark(MarkKey.FontWeight));
        ((HashSet<MarkKey>)service.PendingMarkRemovals).Add(MarkKey.FontWeight);
        
        // Act
        var found = service.CheckMark(MarkKey.FontWeight, out var value);
        
        // Assert
        Assert.That(found, Is.False);
        Assert.That(value, Is.Null);
    }
}