using DScratch.Nodes;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.Helpers;

public class TreeBuilderTests
{
    [Test]
    public void CreatesExpectedResult()
    {
        // Act
        var treeBuilder = new TreeBuilder();
        
        TextNode textP1 = null!;
        var paragraph1 = treeBuilder.Paragraph(t =>
        {
            Assert.That(t.Root, Is.EqualTo(treeBuilder.Root));
            textP1 = t.Text("abc");
        });
        
        TextNode text1P2 = null!;
        TextNode text2P2 = null!;
        TextNode text3P2 = null!;
        var paragraph2 = treeBuilder.Paragraph(t =>
        {
            Assert.That(t.Root, Is.EqualTo(treeBuilder.Root));
            text1P2 = t.Text("t1");
            text2P2 = t.Text("t2");
            text3P2 = t.Text("t3");
        });
        
        TestInlineElementNode inlineElement = null!;
        TextNode textInlineElement = null!;
        ParagraphNode paragraph3 = null!;
        TextNode textP3 = null!;
        var blockElement = treeBuilder.TestBlockElementNode(t =>
        {
            Assert.That(t.Root, Is.EqualTo(treeBuilder.Root));
            inlineElement = t.TestInlineElementNode(t2 =>
            {
                Assert.That(t2.Root, Is.EqualTo(treeBuilder.Root));
                textInlineElement = t2.Text("inline text");
            });
            paragraph3 = t.Paragraph(t2 =>
            {
                Assert.That(t2.Root, Is.EqualTo(treeBuilder.Root));
                textP3 = t2.Text("abc");
            });
        });

        var testNode = treeBuilder.TestNode();
        
        // Assert
        using (Assert.EnterMultipleScope()) // Root Elements
        {
            Assert.That(paragraph1.Parent, Is.EqualTo(treeBuilder.Root));
            Assert.That(paragraph2.Parent, Is.EqualTo(treeBuilder.Root));
            Assert.That(blockElement.Parent, Is.EqualTo(treeBuilder.Root));
            Assert.That(testNode.Parent, Is.EqualTo(treeBuilder.Root));
        }
        
        using (Assert.EnterMultipleScope()) // paragraph1
        {
            Assert.That(paragraph1.Origin, Is.Null);
            Assert.That(paragraph1.RightOrigin, Is.EqualTo(paragraph2));
            
            Assert.That(paragraph1.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(paragraph1.FirstChild, Is.EqualTo(textP1));
            Assert.That(textP1.Parent, Is.EqualTo(paragraph1));
            Assert.That(textP1.TextContent, Is.EqualTo("abc"));
        }
        
        using (Assert.EnterMultipleScope()) // paragraph2
        {
            Assert.That(paragraph2.Origin, Is.EqualTo(paragraph1));
            Assert.That(paragraph2.RightOrigin, Is.EqualTo(blockElement));
            
            Assert.That(paragraph2.ChildNodes, Has.Count.EqualTo(3));
            Assert.That(paragraph2.ChildNodes, Is.EquivalentTo([text1P2, text2P2, text3P2]));
            
            Assert.That(text1P2.TextContent, Is.EqualTo("t1"));
            Assert.That(text1P2.Parent, Is.EqualTo(paragraph2));
            Assert.That(text1P2.Origin, Is.Null);
            Assert.That(text1P2.RightOrigin, Is.EqualTo(text2P2));
            
            Assert.That(text2P2.TextContent, Is.EqualTo("t2"));
            Assert.That(text2P2.Parent, Is.EqualTo(paragraph2));
            Assert.That(text2P2.Origin, Is.EqualTo(text1P2));
            Assert.That(text2P2.RightOrigin, Is.EqualTo(text3P2));
            
            Assert.That(text3P2.TextContent, Is.EqualTo("t3"));
            Assert.That(text3P2.Parent, Is.EqualTo(paragraph2));
            Assert.That(text3P2.Origin, Is.EqualTo(text2P2));
            Assert.That(text3P2.RightOrigin, Is.Null);
        }
        
        using (Assert.EnterMultipleScope()) // blockElement
        {
            Assert.That(blockElement.Origin, Is.EqualTo(paragraph2));
            Assert.That(blockElement.RightOrigin, Is.EqualTo(testNode));
            
            Assert.That(blockElement.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(blockElement.ChildNodes, Is.EquivalentTo([(DNode)inlineElement, paragraph3]));
            
            Assert.That(inlineElement.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(inlineElement.ChildNodes, Is.EquivalentTo([textInlineElement]));
            Assert.That(inlineElement.Origin, Is.Null);
            Assert.That(inlineElement.RightOrigin, Is.EqualTo(paragraph3));
            Assert.That(textInlineElement.Parent, Is.EqualTo(inlineElement));
            Assert.That(textInlineElement.TextContent, Is.EqualTo("inline text"));
            
            
            Assert.That(paragraph3.ChildNodes, Has.Count.EqualTo(1));
            Assert.That(paragraph3.ChildNodes, Is.EquivalentTo([textP3]));
            Assert.That(paragraph3.Origin, Is.EqualTo(inlineElement));
            Assert.That(paragraph3.RightOrigin, Is.Null);
            Assert.That(textP3.Parent, Is.EqualTo(paragraph3));
            Assert.That(textP3.TextContent, Is.EqualTo("abc"));
        }
        
        using (Assert.EnterMultipleScope()) // Root Elements
        {
            Assert.That(testNode.Origin, Is.EqualTo(blockElement));
            Assert.That(testNode.RightOrigin, Is.Null);
        }
    }
}