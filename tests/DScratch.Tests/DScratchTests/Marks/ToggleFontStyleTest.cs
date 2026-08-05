using DScratch.Marks;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Marks;

public class ToggleFontStyleTest
{
    [Test]
    public void Execute_SetsAllToItalic_WhenGivenAnchorHasNotFontStyleMark()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        
        // Act
        new ToggleFontStyle().Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.AddMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.FontStyle, "italic"),
            (node2, MarkKey.FontStyle, "italic"),
            (node3, MarkKey.FontStyle, "italic"),
        ]));
    }
    
    [Test]
    public void Execute_SetsAllToItalic_WhenGivenAnchorIsNormal()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        node1.SetMark(MarkKey.FontStyle, "normal");
        node2.SetMark(MarkKey.FontStyle, "italic");
        node3.SetMark(MarkKey.FontStyle, "normal");
        
        // Act
        new ToggleFontStyle().Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.AddMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.FontStyle, "italic"),
            (node2, MarkKey.FontStyle, "italic"),
            (node3, MarkKey.FontStyle, "italic"),
        ]));
    }
    
    [Test]
    public void Execute_SetsAllToNormal_WhenGivenAnchorIsItalic()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        node1.SetMark(MarkKey.FontStyle, "italic");
        node2.SetMark(MarkKey.FontStyle, "italic");
        node3.SetMark(MarkKey.FontStyle, "normal");
        
        // Act
        new ToggleFontStyle().Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.AddMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.FontStyle, "normal"),
            (node2, MarkKey.FontStyle, "normal"),
            (node3, MarkKey.FontStyle, "normal"),
        ]));
    }
    
    [Test]
    public void AddPending_AddsItalic_WhenMarkIsNotFound()
    {
        // Arrange
        var service = new UserStateServiceFake();
        
        // Act
        new ToggleFontStyle().AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.FontStyle), Is.True);
        Assert.That(service.AddedMarks[MarkKey.FontStyle], Is.EqualTo("italic"));
    }
    
    [Test]
    public void AddPending_AddsItalic_WhenFoundMarkIsNormal()
    {
        // Arrange
        var service = new UserStateServiceFake();
        service.AddedMarks.Add(MarkKey.FontStyle, "normal");
        
        // Act
        new ToggleFontStyle().AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.FontStyle), Is.True);
        Assert.That(service.AddedMarks[MarkKey.FontStyle], Is.EqualTo("italic"));
    }
    
    [Test]
    public void AddPending_AddsNormal_WhenFoundMarkIsItalic()
    {
        // Arrange
        var service = new UserStateServiceFake();
        ((Dictionary<MarkKey, string>)service.AddedMarks).Add(MarkKey.FontStyle, "italic");
        
        // Act
        new ToggleFontStyle().AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.FontStyle), Is.True);
        Assert.That(service.AddedMarks[MarkKey.FontStyle], Is.EqualTo("normal"));
    }
}