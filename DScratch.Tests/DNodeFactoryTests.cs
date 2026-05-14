namespace DScratch.Tests;

public class DNodeFactoryTests
{
    [Test]
    public void ReturnsDCharNode_WithNewId()
    {
        var factory = new DNodeFactory(new TestNodeIdGenerator());

        var result = factory.Char('a');
        
        Assert.That(result.Value, Is.EqualTo('a'));
        Assert.That(result.Id, Is.EqualTo("0"));
    }
}