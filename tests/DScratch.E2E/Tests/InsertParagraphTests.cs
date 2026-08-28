using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests;

public class InsertParagraphTests : PlaywrightTestBase
{
    [Test]
    public async Task InsertParagraph_CreatesNewParagraph_AfterEmptyParagraph()
    {
        await DefaultPage.ClickAsync();

        await Page.EnterAsync();
        await Expect(DefaultPage.Paragraph).ToHaveCountAsync(2);
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task InsertParagraph_AtEndOfParagraph_CreatesNewParagraphAfterCurrent()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");
        await Expect(DefaultPage.Paragraph.TextSpan).ToHaveTextAsync("a");
        await Page.SetCursorAsync("Darki-2", 1);

        await Page.EnterAsync();
        await Expect(DefaultPage.Paragraph).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph.First.TextSpan).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    [Test]
    public async Task InsertParagraph_AtStartOfParagraph_CreatesNewParagraphBeforeCurrent()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");
        await Expect(DefaultPage.Paragraph.TextSpan).ToHaveTextAsync("a");
        await Page.SetCursorAsync("Darki-2", 0);

        await Page.EnterAsync();
        await Expect(DefaultPage.Paragraph).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph.Last.TextSpan).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task InsertParagraph_InMiddleOfParagraph_SplitsTextIntoTwo()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("ab");
        await Expect(DefaultPage.Paragraph.TextSpan).ToHaveTextAsync("ab");
        await Page.SetCursorAsync("Darki-2", 1);

        await Page.EnterAsync();
        await Expect(DefaultPage.Paragraph).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph.First.TextSpan).ToHaveTextAsync("a");
        await Expect(DefaultPage.Paragraph.Last.TextSpan).ToHaveTextAsync("b");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-4"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    [Test]
    public async Task InsertParagraph_DeleteSelection_AndCreatesParagraph()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abcd");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("wtf");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("efgh");
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.Forward,
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-11",
            FocusOffset = 2
        });

        await Page.EnterAsync();

        await Expect(DefaultPage.Paragraph).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph.First.TextSpan.First).ToHaveTextAsync("ab");
        await Expect(DefaultPage.Paragraph.Last.TextSpan.Last).ToHaveTextAsync("gh");

        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-17"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
}