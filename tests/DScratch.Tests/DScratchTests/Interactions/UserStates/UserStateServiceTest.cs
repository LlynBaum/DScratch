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
        service.AddPendingMark(new Mark(MarkKey.Bold));

        // Assert
        Assert.That(service.PendingMarks, Has.Count.EqualTo(1));
        Assert.That(service.PendingMarks.Single(), Is.EqualTo(new Mark(MarkKey.Bold)));
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
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.Bold));
        
        // Act
        service.RemovePendingMark(new Mark(MarkKey.Bold));

        // Assert
        Assert.That(service.PendingMarks, Has.Count.Zero);
    }
    
    [Test]
    public void UpdateState_MakesExpectedChanges_AndCallsOnStateChange()
    {
        // Arrange
        var isCalled = false;
        service.OnStateChange += () => isCalled = true;
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.Bold));

        var node = new TextNode(new NodeId(), null, null);
        node.SetMark(new Mark(MarkKey.Italic));
        
        // Act
        service.UpdateState(node);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isCalled, Is.True);
            Assert.That(service.PendingMarks, Has.Count.Zero);
            Assert.That(service.ActiveMarks, Is.EquivalentTo([new Mark(MarkKey.Italic)]));
        }
    }
    
    [Test]
    public void UpdateState_DoesNotCopyMark_FromNonTextNode()
    {
        // Arrange
        var isCalled = false;
        service.OnStateChange += () => isCalled = true;
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.Bold));
        
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
        ((HashSet<Mark>)service.PendingMarks).Add(new Mark(MarkKey.Bold));
        
        // Act
        var marks = service.PopPending();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
            Assert.That(service.PendingMarks, Has.Count.Zero);
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
}