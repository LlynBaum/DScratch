using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;
using DScratch.TreeVisualizers;

namespace DScratch.Tests.DScratchTests.Nodes;

public class TextNodeTests
{
    [Test]
    public void Length_ReturnsLengthCombines_FromNotDeletedChildNodes()
    {
        // Arrange
        var testNode = new TreeBuilder().Text("abc");
        
        // Act
        var result = testNode.Length;

        // Assert
        Assert.That(result, Is.EqualTo(3));
    }
    
    [Test]
    public void TextContent_ReturnsTextContentCombines_FromNotDeletedChildNodes()
    {
        // Arrange
        var testNode = new TreeBuilder().Text(t =>
        {
            t.Char('a');
            t.Char('b').Delete();
            t.Char('c');
        });
        
        // Act
        var result = testNode.TextContent;

        // Assert
        Assert.That(result, Is.EqualTo("ac"));
    }
    
    [Test]
    public void Split_CreatesExpectedNode()
    {
        // Arrange
        CharNode char1 = null!;
        CharNode char2 = null!;
        CharNode char3 = null!;
        var testNode = new TreeBuilder()
            .Paragraph(t =>
            {
                t.Text();
                t.Text(txt =>
                {
                    char1 = txt.Char('a');
                    char2 = txt.Char('b');
                    char3 = txt.Char('c');
                });
                t.Text();
            });

        new TreeVisualizer(testNode).Print();
        
        var textNode = (TextNode)testNode.ChildNodes[1];
        
        // Act
        var result = textNode.Split(1, "-1");
        
        new TreeVisualizer(testNode).Print();

        // Assert
        Assert.That(testNode.ChildNodes, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo("-1"));

            Assert.That(textNode.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(result.ChildNodes, Has.Count.EqualTo(2));

            Assert.That(textNode.TextContent, Is.EqualTo("a"));
            Assert.That(result.TextContent, Is.EqualTo("bc"));

            Assert.That(textNode.Parent, Is.EqualTo(testNode));
            Assert.That(result.Parent, Is.EqualTo(testNode));
            
            Assert.That(char1.Parent, Is.EqualTo(textNode));
            Assert.That(char1.Origin, Is.Null);
            Assert.That(char1.RightOrigin, Is.Null);
            Assert.That(char2.Parent, Is.EqualTo(result));
            Assert.That(char2.Origin, Is.Null);
            Assert.That(char2.RightOrigin, Is.EqualTo(char3));
            Assert.That(char3.Parent, Is.EqualTo(result));
            Assert.That(char3.Origin, Is.EqualTo(char2));
            Assert.That(char3.RightOrigin, Is.Null);

            Assert.That(testNode.FirstChild!.Origin, Is.Null);
            Assert.That(testNode.FirstChild.RightOrigin, Is.EqualTo(textNode));
            
            Assert.That(textNode.Origin, Is.EqualTo(testNode.FirstChild));
            Assert.That(textNode.RightOrigin, Is.EqualTo(result));
            Assert.That(result.Origin, Is.EqualTo(textNode));
            Assert.That(result.RightOrigin, Is.EqualTo(testNode.LastChild));
            
            Assert.That(testNode.LastChild!.Origin, Is.EqualTo(result));
            Assert.That(testNode.LastChild.RightOrigin, Is.Null);
        }
    }
    
    [Test]
    public void InsertChild_ThrowsInvalidOperationException_WhenNodeIsNotCharNode()
    {
        // Arrange
        var testNode = new TextNode("1", null, null);
        var node = new TestNode("2", null, null)
        {
            Parent = testNode
        };

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        return;

        // Act
        void Act() => testNode.InsertChild(node);
    }
}