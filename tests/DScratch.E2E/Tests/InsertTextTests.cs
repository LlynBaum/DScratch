using DScratch.E2E.Framework;

namespace DScratch.E2E.Tests;

public class InsertTextTests : PlaywrightTestBase
{
    [Test]
    public async Task WriteText_IntoEmptyParagraph_AppendsCharactersAndAdvancesCursor()
    {
        const string text = "DScratch";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();
        
        await Page.TypeAtCurrentCursorAsync(text);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync(text);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }

    [Test]
    public async Task WriteText_AtStartOfText_PrependsCharactersAndAdvancesCursor()
    {
        const string text = "DScratch";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");

        await Page.SetCursorAsync("Darki-2", 0);
        await Page.TypeAtCurrentCursorAsync(text);

        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync(text);
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }
    
    [Test]
    public async Task WriteText_AtEndOfText_PrependsCharactersAndAdvancesCursor()
    {
        const string text = "DScratch";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");

        await Page.SetCursorAsync("Darki-2", 1);
        await Page.TypeAtCurrentCursorAsync(text);

        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("a" + text);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length + 1));
        }
    }
    
    [Test]
    public async Task WriteText_InMiddleOfText_PrependsCharactersAndAdvancesCursor()
    {
        const string text = "DScratch";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("ab");

        await Page.SetCursorAsync("Darki-2", 1);
        await Page.TypeAtCurrentCursorAsync(text);

        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync(text);
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("b");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }
    
    // TODO: test with selection
}