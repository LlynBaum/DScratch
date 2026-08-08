using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests;

public class InsertTextTests : PlaywrightTestBase
{
    [Test]
    public async Task WriteText_IntoEmptyParagraph_AppendsCharactersAndAdvancesCursor()
    {
        const string text = "DScratch";
        
        await Editor.ClickAsync();
        
        await Page.TypeAtCurrentCursorAsync(text);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync(text);
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
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
        
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");

        await Page.SetCursorAsync("Darki-2", 0);
        await Page.TypeAtCurrentCursorAsync(text);

        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync(text);
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
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
        
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");

        await Page.SetCursorAsync("Darki-2", 1);
        await Page.TypeAtCurrentCursorAsync(text);

        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("a" + text);
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
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
        
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("ab");

        await Page.SetCursorAsync("Darki-2", 1);
        await Page.TypeAtCurrentCursorAsync(text);

        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync(text);
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("b");

        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-5"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }

    [Test]
    public async Task WriteText_ReplaceTextSelectionWithTypedText()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abcd");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-2",
            FocusOffset = 3
        });

        await Page.TypeAtCurrentCursorAsync("f");

        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("f");
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("d");

        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-10"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task WriteText_ReplaceTextSelectionWithTypedText_AndMergeParagraphs()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abcd");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("wtf");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("efgh");
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-11",
            FocusOffset = 2
        });

        await Page.TypeAtCurrentCursorAsync("x");

        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("ab");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("x");
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("gh");

        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-19"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
        }
    }
}