using DScratch.Interactions.UserStates;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Tests.Helpers;
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
    public void ActiveMarks_AreComputedMarks()
    {
        // Arrange
        TextNode node = null!;
        var block = new TreeBuilder().Paragraph(t =>
        {
            node = t.Text("a");
        });
        
        block.SetMark(MarkKey.FontStyle, "italic");
        node.SetMark(MarkKey.FontWeight, "bold");
        service.UpdateState(node);
        
        // Act
        var result = service.ActiveMarks;
        
        // Assert
        Assert.That(result.Keys, Is.EquivalentTo([MarkKey.FontStyle, MarkKey.FontWeight]));
    }

    [Test]
    public void AddPendingMark_AddsMarkToPendingMarks()
    {
        // Act
        service.AddPendingMark(MarkKey.FontWeight, "bold");

        // Assert
        Assert.That(service.PendingMarks, Has.Count.EqualTo(1));
        Assert.That(service.PendingMarks[MarkKey.FontWeight], Is.EqualTo("bold"));
    }
    
    [Test]
    public void AddPendingMark_OverridesExistingPendingMarks()
    {
        // Arrange
        service.AddPendingMark(MarkKey.Color, "a");
        
        // Act
        service.AddPendingMark(MarkKey.Color, "b");

        // Assert
        Assert.That(service.PendingMarks, Has.Count.EqualTo(1));
        Assert.That(service.PendingMarks[MarkKey.Color], Is.EqualTo("b"));
    }
    
    [Test]
    public void UpdateState_MakesExpectedChanges_AndCallsOnStateChange()
    {
        // Arrange
        var isCalled = false;
        service.OnStateChange += () => isCalled = true;
        service.AddPendingMark(MarkKey.FontWeight, "bold");

        var node = new TextNode(new NodeId(), null, null);
        node.SetMark(MarkKey.FontStyle, "italic");
        
        // Act
        service.UpdateState(node);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isCalled, Is.True);
            Assert.That(service.PendingMarks, Has.Count.Zero);
            Assert.That(service.ActiveMarks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontStyle, "italic" } }));
        }
    }
    
    [Test]
    public void UpdateState_DoesNotCopyMark_FromNonTextNode()
    {
        // Arrange
        var isCalled = false;
        service.OnStateChange += () => isCalled = true;
        service.AddPendingMark(MarkKey.FontWeight, "bold");
        
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
        service.AddPendingMark(MarkKey.FontWeight, "bold");
        
        // Act
        var marks = service.PopPending();
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(marks, Is.EquivalentTo(new Dictionary<MarkKey, string> { { MarkKey.FontWeight, "bold" } }));
            Assert.That(service.PendingMarks, Has.Count.Zero);
        }
    }

    [Test]
    public void CheckMark_ReturnsTrue_WhenFoundAsPendingMark()
    {
        // Arrange
        service.AddPendingMark(MarkKey.Color, "a");
        
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
        var node = new TextNode(new NodeId(), null, null);
        node.SetMark(MarkKey.Color, "a");
        service.UpdateState(node);
        
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