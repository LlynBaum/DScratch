using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Transactions;

public class StepDiffTests
{
    [Test]
    public void ExpectedType_InsertTextDiff()
    {
        var step = new StepDiff.InsertTextDiff("", 0, "");
        Assert.That(step.Type, Is.EqualTo("insertText"));
    }
    
    [Test]
    public void ExpectedType_DeleteTextDiff()
    {
        var step = new StepDiff.DeleteTextDiff("", 0, 0);
        Assert.That(step.Type, Is.EqualTo("deleteText"));
    }
    
    [Test]
    public void ExpectedType_InsertElementDiff()
    {
        var step = new StepDiff.InsertElementDiff("", null, "", "");
        Assert.That(step.Type, Is.EqualTo("insertElement"));
    }
    
    [Test]
    public void ExpectedType_DeleteElementDiff()
    {
        var step = new StepDiff.DeleteElementDiff("");
        Assert.That(step.Type, Is.EqualTo("deleteElement"));
    }
    
    [Test]
    public void ExpectedType_MoveDiff()
    {
        var step = new StepDiff.MoveDiff("", "", null);
        Assert.That(step.Type, Is.EqualTo("move"));
    }
}