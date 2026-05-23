using DScratch.Transactions;

namespace DScratch.Tests.Transactions;

public class StepDiffTests
{
    [Test]
    public void ExpectedType_InsertTextDiff()
    {
        var step = new StepDiff.InsertTextDiff([], 0, "");
        Assert.That(step.Type, Is.EqualTo("insertText"));
    }
    
    [Test]
    public void ExpectedType_DeleteTextDiff()
    {
        var step = new StepDiff.DeleteTextDiff([], 0, 0);
        Assert.That(step.Type, Is.EqualTo("deleteText"));
    }
    
    [Test]
    public void ExpectedType_InsertElementInlineDiff()
    {
        var step = new StepDiff.InsertElementInlineDiff([], 0, "", "");
        Assert.That(step.Type, Is.EqualTo("insertElementInline"));
    }
    
    [Test]
    public void ExpectedType_InsertElementBlockDiff()
    {
        var step = new StepDiff.InsertElementBlockDiff([], null, "", "");
        Assert.That(step.Type, Is.EqualTo("insertElementBlock"));
    }
    
    [Test]
    public void ExpectedType_DeleteElementDiff()
    {
        var step = new StepDiff.DeleteElementDiff([]);
        Assert.That(step.Type, Is.EqualTo("deleteElement"));
    }
    
    [Test]
    public void ExpectedType_MoveInlineDiff()
    {
        var step = new StepDiff.MoveInlineDiff([], [], 0);
        Assert.That(step.Type, Is.EqualTo("moveInline"));
    }
    
    [Test]
    public void ExpectedType_MoveBlockDiff()
    {
        var step = new StepDiff.MoveBlockDiff([], [], null);
        Assert.That(step.Type, Is.EqualTo("moveBlock"));
    }
}