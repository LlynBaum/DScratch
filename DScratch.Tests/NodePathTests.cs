namespace DScratch.Tests;

public class NodePathTests
{
    [Test]
    public void ReturnsTheLengthOfThePath()
    {
        var path = new NodePath(["1", "2"]);
        
        Assert.That(path.Length, Is.EqualTo(2));
    }
    
    [Test]
    public void ReturnsPathAsArray()
    {
        var path = new NodePath(["1", "2"]);
        
        Assert.That(path.Path, Is.EquivalentTo(["1", "2"]));
    }
    
    [Test]
    public void ReturnsPathPartAtIndex()
    {
        var path = new NodePath(["1", "2"]);

        var result1 = path[0];
        var result2 = path[1];
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.EqualTo("1"));
            Assert.That(result2, Is.EqualTo("2"));
        }
    }
    
    [Test]
    public void ReturnsPathAsString()
    {
        var path = new NodePath(["1", "2"]);
        
        Assert.That(path.ToString(), Is.EqualTo("1__2"));
    }
}