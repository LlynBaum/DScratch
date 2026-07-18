using DScratch.Interactions;
using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Tests.DScratchTests.Interactions.Helpers;
using DScratch.Tests.Helpers;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Interactions.EventHandlers.Common;

[TestFixture]
public class EventWithSelectionBaseTests
{
    private Testee testee;
    private TreeBuilder builder;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        testee = new Testee(new DScratchService(builder.CreateDocument(), new DNodeFactory(builder.IdGenerator), new UserStateService()));
    }

    [Test]
    public void FindsExpectedTextNode_FromGivenKeyPressInfo_WithAnchorIdForTextNode()
    {
        // Arrange
        var paragraph = builder.Paragraph(t => t.Text("a"));
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(paragraph.FirstChild!.Id, 0);
        
        // Act
        testee.Handle(keyPressInfo);
        
        // Arrange
        testee.AssertNoneSelectionHandled(paragraph.FirstChild);
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void FindsExpectedTextNode_FromGivenKeyPressInfo_WithAnchorIdToParagraph(int offset)
    {
        // Arrange
        var paragraph = builder.Paragraph(t => t.Text("ab"));
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(paragraph.Id, offset);
        
        // Act
        testee.Handle(keyPressInfo);
        
        // Arrange
        testee.AssertNoneSelectionHandled(paragraph.FirstChild!);
    }
    
    [Test]
    public void InvokesHandleEmptyBlock_WhenNoTextNodeIsPresent()
    {
        // Arrange
        var paragraph = builder.Paragraph();
        var keyPressInfo = KeyPressInfoHelper.GetKeyPressInfoDirectionNone(paragraph.Id, 0);
        
        // Act
        testee.Handle(keyPressInfo);
        
        // Arrange
        testee.AssertEmptyBlockHandled(paragraph);
    }

    private class Testee(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
    {
        private bool noneSelectionHandled;
        private TextNode? noneSelectionTextNode;
        
        private bool emptyBlockHandled;
        private DNode? emptyBlockAnchorNode;
        
        protected override DNodeSearchResult HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction,
            TextNode anchorTextNode)
        {
            noneSelectionHandled = true;
            noneSelectionTextNode = anchorTextNode;
            return DNodeSearchResult.Empty;
        }

        protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
        {
            emptyBlockHandled = true;
            emptyBlockAnchorNode = anchorNode;
        }

        public void AssertNoneSelectionHandled(DNode expectedNode)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(noneSelectionHandled, Is.True);
                Assert.That(noneSelectionTextNode, Is.Not.Null);
            }
            Assert.That(noneSelectionTextNode, Is.EqualTo(expectedNode));
        }
        
        public void AssertEmptyBlockHandled(DNode expectedNode)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(emptyBlockHandled, Is.True);
                Assert.That(emptyBlockAnchorNode, Is.Not.Null);
            }
            Assert.That(emptyBlockAnchorNode, Is.EqualTo(expectedNode));
        }
    }
}