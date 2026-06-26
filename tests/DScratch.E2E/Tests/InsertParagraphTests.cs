using DScratch.E2E.Framework;

namespace DScratch.E2E.Tests;

public class InsertParagraphTests : PlaywrightTestBase
{
    [Test]
    public async Task InsertParagraph_CreatesNewParagraph_AfterEmptyParagraph()
    {
        await Editor.ClickAsync();

        await Page.EnterAsync();
        await Expect(Editor.Paragraph).ToHaveCountAsync(2);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task InsertParagraph_AtEndOfParagraph_CreatesNewParagraphAfterCurrent()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("a");
        await Page.SetCursorAsync("Darki-2", 1);

        await Page.EnterAsync();
        await Expect(Editor.Paragraph).ToHaveCountAsync(2);
        await Expect(Editor.Paragraph.First.TextSpan).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    [Test]
    public async Task InsertParagraph_AtStartOfParagraph_CreatesNewParagraphBeforeCurrent()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("a");
        await Page.SetCursorAsync("Darki-2", 0);

        await Page.EnterAsync();
        await Expect(Editor.Paragraph).ToHaveCountAsync(2);
        await Expect(Editor.Paragraph.Last.TextSpan).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task InsertParagraph_InMiddleOfParagraph_SplitsTextIntoTwo()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("ab");
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("ab");
        await Page.SetCursorAsync("Darki-2", 1);

        await Page.EnterAsync();
        await Expect(Editor.Paragraph).ToHaveCountAsync(2);
        await Expect(Editor.Paragraph.First.TextSpan).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.Last.TextSpan).ToHaveTextAsync("b");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-4"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    // TODO: test with selection
}