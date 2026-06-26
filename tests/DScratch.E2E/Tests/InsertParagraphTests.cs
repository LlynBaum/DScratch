using DScratch.E2E.Framework;
using DScratch.Interactions;

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
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
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
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
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
    
    [Test]
    public async Task InsertParagraph_DeleteSelection_AndCreatesParagraph()
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

        await Page.EnterAsync();

        await Expect(Editor.Paragraph).ToHaveCountAsync(2);
        await Expect(Editor.Paragraph.First.TextSpan.First).ToHaveTextAsync("ab");
        await Expect(Editor.Paragraph.Last.TextSpan.Last).ToHaveTextAsync("gh");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-16"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
}