using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

public class UpdateMarkHandlerTest
{
    private TreeBuilder builder;
    private ITransaction transaction;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        transaction = new DTransaction(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator),
            nodeIdGenerator: builder.IdGenerator, 
            disableCleanUp: false);
    }

    private class ToggleAction : UpdateMarkHandlerTest
    {
        [Test]
        public void DoesNothing_WithSelectionDirectionNone()
        {
            // Arrange
            TextNode start = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
            });

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(start.Id, 2);
            
            // Act
            UpdateMarkHandler.Execute(transaction, keyPressInfo.Selection, new Mark(MarkKey.Bold), UpdateMarkAction.Toggle);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(1));
                Assert.That(start.Marks, Has.Count.Zero);
            }
        }
        
        [Test]
        public void RemovesMarks_FromSelection_WhenAnchorHasGivenMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            start.SetMark(new Mark(MarkKey.Bold));
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            UpdateMarkHandler.Execute(transaction, keyPressInfo.Selection, new Mark(MarkKey.Bold), UpdateMarkAction.Toggle);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
                
                Assert.That(start.Origin, Is.Not.Null);
                Assert.That(start.Origin, Is.TypeOf<TextNode>());
                Assert.That(end.RightOrigin, Is.Not.Null);
                Assert.That(end.RightOrigin, Is.TypeOf<TextNode>());
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(((TextNode)start.Origin!).TextContent, Is.EqualTo("ab"));
                Assert.That(start.TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(((TextNode)start.Origin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                
                Assert.That(start.Marks, Is.Empty);
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.Empty);
                Assert.That(((TextNode)end.Origin!).Marks, Is.Empty);
                Assert.That(end.Marks, Is.Empty);
            }
        }
        
        [Test]
        public void AddMarks_FromSelection_WhenAnchorDoesNotHaveGivenMark()
        {
            // Arrange
            TextNode start = null!;
            TextNode markedNode = null!;
            TextNode end = null!;
            var parent1 = builder.Paragraph(t =>
            {
                start = t.Text("abc");
                t.Text("def");
            });
            var parent2 = builder.Paragraph(t =>
            {
                markedNode = t.Text("123");
                end = t.Text("456");
            });
            
            markedNode.SetMark(new Mark(MarkKey.Bold));

            var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfo(start.Id, 2, end.Id, 1);
            
            // Act
            UpdateMarkHandler.Execute(transaction, keyPressInfo.Selection, new Mark(MarkKey.Bold), UpdateMarkAction.Toggle);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parent1.ChildNodes, Has.Count.EqualTo(3));
                Assert.That(parent2.ChildNodes, Has.Count.EqualTo(3));
                
                Assert.That(start.Origin, Is.Not.Null);
                Assert.That(start.Origin, Is.TypeOf<TextNode>());
                Assert.That(end.RightOrigin, Is.Not.Null);
                Assert.That(end.RightOrigin, Is.TypeOf<TextNode>());
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(((TextNode)start.Origin!).TextContent, Is.EqualTo("ab"));
                Assert.That(start.TextContent, Is.EqualTo("c"));
                Assert.That(end.TextContent, Is.EqualTo("4"));
                Assert.That(((TextNode)end.RightOrigin!).TextContent, Is.EqualTo("56"));
                
                Assert.That(((TextNode)start.Origin!).Marks, Is.Empty);
                Assert.That(((TextNode)end.RightOrigin!).Marks, Is.Empty);
                
                Assert.That(start.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)start.RightOrigin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(((TextNode)end.Origin!).Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
                Assert.That(end.Marks, Is.EquivalentTo([new Mark(MarkKey.Bold)]));
            }
        }
    }
}