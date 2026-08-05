using DScratch.Marks;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests.Marks;

public class SetColorTest
{
    [Test]
    public void Execute_AddsColorMarkToAllNodes()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        
        // Act
        new SetColor("#fff").Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.AddMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.Color, "#fff"),
            (node2, MarkKey.Color, "#fff"),
            (node3, MarkKey.Color, "#fff"),
        ]));
    }
    
    [Test]
    public void Execute_OverwritesColorMarkToAllNodes()
    {
        // Arrange
        var transactionSpy = new DTransactionSpy();
        
        var node1 = TestNode.Empty();
        var node2 = TestNode.Empty();
        var node3 = TestNode.Empty();
        node1.SetMark(MarkKey.Color, "#000");
        node2.SetMark(MarkKey.Color, "#001");
        node3.SetMark(MarkKey.Color, "#002");
        
        // Act
        new SetColor("#fff").Execute(transactionSpy, node1, [node1, node2, node3]);
        
        // Assert
        Assert.That(transactionSpy.AddMarkCalls, Is.EquivalentTo([
            (node1, MarkKey.Color, "#fff"),
            (node2, MarkKey.Color, "#fff"),
            (node3, MarkKey.Color, "#fff"),
        ]));
    }
    
    [Test]
    public void AddPending_AddsMarkToPendingMarks()
    {
        // Arrange
        var service = new UserStateServiceFake();
        
        // Act
        new SetColor("#fff").AddPending(service);
        
        // Assert
        Assert.That(service.AddedMarks, Has.Count.EqualTo(1));
        Assert.That(service.AddedMarks.ContainsKey(MarkKey.Color), Is.True);
        Assert.That(service.AddedMarks[MarkKey.Color], Is.EqualTo("#fff"));
    }
}