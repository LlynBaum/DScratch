using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

public class TreeWalkerTests
{
    private readonly TreeBuilder treeBuilder = new TreeBuilder();
    
    [Test]
    public void MoveNext_FiltersExpectedNodes()
    {
        // Arrange
        TextNode testNode1 = null!;
        TextNode child = null!;
        TextNode testNode3 = null!;

        var paragraph = treeBuilder.Paragraph(p =>
        {
            testNode1 = p.Text(t =>
            {
                child = ((TreeBuilder)t).Text(t2 => t2.Char('a'));
            });
            p.TestInlineElementNode();
            testNode3 = p.Text("a"); 
            p.Text("a").Delete();
        });
        
        // Act & Assert
        var walker = new TreeWalker<TextNode>(paragraph);
        
        Assert.That(walker.NextNode(), Is.EqualTo(testNode1));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(testNode1));
        
        Assert.That(walker.NextNode(), Is.EqualTo(child));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(child));
        
        Assert.That(walker.NextNode(), Is.EqualTo(testNode3));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(testNode3));

        Assert.That(walker.NextNode(), Is.Null);
        Assert.That(walker.Current, Is.Null);
    }
    
    [Test]
    public void MoveNext_FiltersExpectedNodes_IncludesDeleted()
    {
        // Arrange
        TextNode testNode1 = null!;
        TextNode child = null!;
        TextNode testNode3 = null!;
        TextNode testNode4 = null!;

        var paragraph = treeBuilder.Paragraph(p =>
        {
            testNode1 = p.Text(t =>
            {
                child = ((TreeBuilder)t).Text(t2 => t2.Char('a'));
            });
            p.TestInlineElementNode();
            testNode3 = p.Text("");
            testNode4 = p.Text("");
            testNode4.Delete();
        });
        
        // Act & Assert
        var walker = new TreeWalker<TextNode>(paragraph, true);
        
        Assert.That(walker.NextNode(), Is.EqualTo(testNode1));
        Assert.That(walker.Current, Is.EqualTo(testNode1));
        
        Assert.That(walker.NextNode(), Is.EqualTo(child));
        Assert.That(walker.Current, Is.EqualTo(child));
        
        Assert.That(walker.NextNode(), Is.EqualTo(testNode3));
        Assert.That(walker.Current, Is.EqualTo(testNode3));

        Assert.That(walker.NextNode(), Is.EqualTo(testNode4));
        Assert.That(walker.Current, Is.EqualTo(testNode4));
        
        Assert.That(walker.NextNode(), Is.Null);
        Assert.That(walker.Current, Is.Null);
    }
    
    [Test]
    public void NextSibling_FiltersExpectedNodes()
    {
        // Arrange
        TextNode testNode1 = null!;
        TextNode testNode3 = null!;

        treeBuilder.Paragraph(p =>
        {
            testNode1 = p.Text("");
            p.TestInlineElementNode();
            testNode3 = p.Text("");
        });
        
        // Act & Assert
        var walker = new TreeWalker<TextNode>(testNode1);
        
        Assert.That(walker.Current, Is.EqualTo(testNode1));
        
        Assert.That(walker.NextSibling(), Is.EqualTo(testNode3));
        Assert.That(walker.Current, Is.Not.Null);
        Assert.That(walker.Current, Is.EqualTo(testNode3));

        Assert.That(walker.NextSibling(), Is.Null);
        Assert.That(walker.Current, Is.Null);
    }
    
    [Test]
    public void FirstChild_FiltersExpectedNodes()
    {
        // Arrange
        TextNode testNode2 = null!;

        var paragraph = treeBuilder.Paragraph(p =>
        {
            p.TestInlineElementNode();
            testNode2 = p.Text("");
            p.Text("");
        });
        
        // Act & Assert
        var walker = new TreeWalker<TextNode>(paragraph);
        
        Assert.That(walker.FirstChild(), Is.EqualTo(testNode2));
        Assert.That(walker.Current, Is.EqualTo(testNode2));
    }
    
    [Test]
    public void DoesOnlySeekWithinParent()
    {
        // Arrange
        var paragraph = treeBuilder.Paragraph(p =>
        {
            p.Text("");
            p.Text("");
        });
        
        // Act & Assert
        var walker = new TreeWalker<TextNode>(paragraph);

        walker.NextNode();
        walker.NextNode();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(walker.NextNode(), Is.Null);
            Assert.That(walker.Current, Is.Null);
        }
    }
}