using DScratch.Nodes;
using DScratch.Tests.Helpers;
using DScratch.Tests.Helpers.TestNodes;

namespace DScratch.Tests.DScratchTests;

[TestFixture]
public class TreeWalkerTests
{
    private readonly TreeBuilder treeBuilder = new TreeBuilder();
    
    private class MoveNext : TreeWalkerTests
    {
        [Test]
        public void MoveNext_FiltersExpectedNodes()
        {
            // Arrange
            TestInlineElementNode testNode1 = null!;
            TestInlineElementNode child = null!;
            TestInlineElementNode testNode3 = null!;

            var paragraph = treeBuilder.Paragraph(p =>
            {
                testNode1 = p.TestInlineElementNode(t =>
                {
                    child = t.TestInlineElementNode(t2 => t2.Text("a"));
                });
                testNode3 = p.TestInlineElementNode();
                p.Text("a"); 
                p.TestInlineElementNode().Delete();
            });
            
            // Act & Assert
            var walker = new TreeWalker<TestInlineElementNode>(paragraph);
            
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
            TestInlineElementNode testNode1 = null!;
            TestInlineElementNode child = null!;
            TestInlineElementNode testNode3 = null!;
            TestInlineElementNode testNode4 = null!;

            var paragraph = treeBuilder.Paragraph(p =>
            {
                testNode1 = p.TestInlineElementNode(t =>
                {
                    child = t.TestInlineElementNode(t2 => t2.Text("a"));
                });
                p.Text("a");
                testNode3 = p.TestInlineElementNode();
                testNode4 = p.TestInlineElementNode();
                testNode4.Delete();
            });
            
            // Act & Assert
            var walker = new TreeWalker<TestInlineElementNode>(paragraph, true);
            
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
        public void DoesOnlySeekWithinParent()
        {
            // Arrange
            var paragraph = treeBuilder.Paragraph(p =>
            {
                p.Text("a");
                p.Text("a");
            });
        
            // Act & Assert
            var walker = new TreeWalker<DNode>(paragraph);

            walker.NextNode();
            walker.NextNode();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(walker.NextNode(), Is.Null);
                Assert.That(walker.Current, Is.Null);
            }
        }
    }
    
    private class MovePrevious : TreeWalkerTests
    {
        [Test]
        public void MovePrevious_FiltersExpectedNodes()
        {
            // Arrange
            TestInlineElementNode testNode1 = null!;
            TestInlineElementNode child = null!;
            TestInlineElementNode testNode3 = null!;
            TestInlineElementNode testNode4 = null!;

            var paragraph = treeBuilder.Paragraph(p =>
            {
                testNode1 = p.TestInlineElementNode(t =>
                {
                    child = t.TestInlineElementNode(t2 => t2.Text("a"));
                });
                p.Text("a");
                testNode3 = p.TestInlineElementNode();
                testNode4 = p.TestInlineElementNode();
            });
            
            // Act & Assert
            var walker = new TreeWalker<TestInlineElementNode>(paragraph);
            
            Assert.That(walker.NextNode(), Is.EqualTo(testNode1));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(testNode1));
            
            Assert.That(walker.NextNode(), Is.EqualTo(child));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(child));
            
            Assert.That(walker.NextNode(), Is.EqualTo(testNode3));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(testNode3));

            Assert.That(walker.NextNode(), Is.EqualTo(testNode4));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(testNode4));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(testNode3));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(testNode3));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(child));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(child));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(testNode1));
            Assert.That(walker.Current, Is.Not.Null);
            Assert.That(walker.Current, Is.EqualTo(testNode1));
        }
        
        [Test]
        public void MovePrevious_FiltersExpectedNodes_IncludesDeleted()
        {
            // Arrange
            TestInlineElementNode testNode1 = null!;
            TestInlineElementNode child = null!;
            TestInlineElementNode testNode3 = null!;
            TestInlineElementNode testNode4 = null!;

            var paragraph = treeBuilder.Paragraph(p =>
            {
                testNode1 = p.TestInlineElementNode(t =>
                {
                    child = t.TestInlineElementNode(t2 => t2.Text("a"));
                });
                p.Text("a");
                testNode3 = p.TestInlineElementNode();
                testNode4 = p.TestInlineElementNode();
                testNode4.Delete();
            });
            
            // Act & Assert
            var walker = new TreeWalker<TestInlineElementNode>(paragraph, true);
            
            Assert.That(walker.NextNode(), Is.EqualTo(testNode1));
            Assert.That(walker.Current, Is.EqualTo(testNode1));
            
            Assert.That(walker.NextNode(), Is.EqualTo(child));
            Assert.That(walker.Current, Is.EqualTo(child));
            
            Assert.That(walker.NextNode(), Is.EqualTo(testNode3));
            Assert.That(walker.Current, Is.EqualTo(testNode3));

            Assert.That(walker.NextNode(), Is.EqualTo(testNode4));
            Assert.That(walker.Current, Is.EqualTo(testNode4));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(testNode3));
            Assert.That(walker.Current, Is.EqualTo(testNode3));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(child));
            Assert.That(walker.Current, Is.EqualTo(child));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(testNode1));
            Assert.That(walker.Current, Is.EqualTo(testNode1));
        }

        [Test]
        public void MovePrevious_ReturnsExpectedNodes()
        {
            TextNode textNode1 = null!;
            TextNode textNode2 = null!;
            TextNode textNode3 = null!;
            var parent = treeBuilder.TestInlineElementNode(t => 
            {
                t.Paragraph(txt =>
                {
                    textNode1 = txt.Text("a");
                    textNode2 = txt.Text("b");
                });
                t.Paragraph(txt => 
                {
                    textNode3 = txt.Text("c");
                });
            });
        
            // Act & Assert
            var walker = new TreeWalker<TextNode>(parent);
            walker.NextNode();
            walker.NextNode();
            walker.NextNode();
            
            Assert.That(walker.Current, Is.EqualTo(textNode3));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(textNode2));
            Assert.That(walker.Current, Is.EqualTo(textNode2));
            
            Assert.That(walker.MovePrevious(), Is.EqualTo(textNode1));
            Assert.That(walker.Current, Is.EqualTo(textNode1));
        }
        
        [Test]
        public void DoesOnlySeekWithinParent()
        {
            // Arrange
            var paragraph = treeBuilder.Paragraph(p =>
            {
                p.Text("a");
                p.Text("a");
            });
        
            // Act & Assert
            var walker = new TreeWalker<DNode>(paragraph);

            walker.NextNode();
            walker.MovePrevious();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(walker.MovePrevious(), Is.Null);
                Assert.That(walker.Current, Is.Null);
            }
        }
    }
    
    private class NextSibling : TreeWalkerTests
    {
        [Test]
        public void NextSibling_FiltersExpectedNodes()
        {
            // Arrange
            TextNode testNode1 = null!;
            TextNode testNode3 = null!;

            treeBuilder.Paragraph(p =>
            {
                testNode1 = p.Text("a");
                p.TestInlineElementNode();
                testNode3 = p.Text("a");
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
    }
    
    private class FirstChild : TreeWalkerTests
    {
        [Test]
        public void FirstChild_FiltersExpectedNodes()
        {
            // Arrange
            TextNode testNode2 = null!;

            var paragraph = treeBuilder.Paragraph(p =>
            {
                p.TestInlineElementNode();
                testNode2 = p.Text("a");
                p.Text("a");
            });

            // Act & Assert
            var walker = new TreeWalker<TextNode>(paragraph);

            Assert.That(walker.FirstChild(), Is.EqualTo(testNode2));
            Assert.That(walker.Current, Is.EqualTo(testNode2));
        }
    }
}