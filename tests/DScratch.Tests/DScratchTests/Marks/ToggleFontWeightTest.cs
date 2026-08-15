using DScratch.Marks;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Marks;

public class ToggleFontWeightTest
{
    [Test]
    public void Execute_SetsAllToBold_WhenGivenAnchorHasNotFontWeightMark()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        
        // Act
        new ToggleFontWeight().Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.UpdateMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.FontWeight, "bold"),
            (node2, MarkKey.FontWeight, "bold"),
            (node3, MarkKey.FontWeight, "bold"),
        ]));
    }
    
    [Test]
    public void Execute_SetsAllToBold_WhenGivenAnchorIsNormal()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        node1.SetMark(MarkKey.FontWeight, "normal");
        node2.SetMark(MarkKey.FontWeight, "bold");
        node3.SetMark(MarkKey.FontWeight, "normal");
        
        // Act
        new ToggleFontWeight().Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.UpdateMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.FontWeight, "bold"),
            (node2, MarkKey.FontWeight, "bold"),
            (node3, MarkKey.FontWeight, "bold"),
        ]));
    }
    
    [Test]
    public void Execute_SetsAllToNormal_WhenGivenAnchorIsBold()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        node1.SetMark(MarkKey.FontWeight, "bold");
        node2.SetMark(MarkKey.FontWeight, "bold");
        node3.SetMark(MarkKey.FontWeight, "normal");
        
        // Act
        new ToggleFontWeight().Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.UpdateMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.FontWeight, "normal"),
            (node2, MarkKey.FontWeight, "normal"),
            (node3, MarkKey.FontWeight, "normal"),
        ]));
    }
    
    [Test]
    public void AddPending_AddsBold_WhenMarkIsNotFound()
    {
        // Arrange
        var service = new UserStateServiceFake();
        
        // Act
        new ToggleFontWeight().AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.FontWeight), Is.True);
        Assert.That(service.AddedMarks[MarkKey.FontWeight], Is.EqualTo("bold"));
    }
    
    [Test]
    public void AddPending_AddsBold_WhenFoundMarkIsNormal()
    {
        // Arrange
        var service = new UserStateServiceFake();
        service.AddedMarks.Add(MarkKey.FontWeight, "normal");
        
        // Act
        new ToggleFontWeight().AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.FontWeight), Is.True);
        Assert.That(service.AddedMarks[MarkKey.FontWeight], Is.EqualTo("bold"));
    }
    
    [Test]
    public void AddPending_AddsNormal_WhenFoundMarkIsBold()
    {
        // Arrange
        var service = new UserStateServiceFake();
        service.AddedMarks.Add(MarkKey.FontWeight, "bold");
        
        // Act
        new ToggleFontWeight().AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.FontWeight), Is.True);
        Assert.That(service.AddedMarks[MarkKey.FontWeight], Is.EqualTo("normal"));
    }
}