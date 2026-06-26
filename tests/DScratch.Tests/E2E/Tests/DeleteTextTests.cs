using DScratch.Tests.E2E.Framework;

namespace DScratch.Tests.E2E.Tests;

public class DeleteTextTests : PlaywrightTestBase
{
    [Test]
    public async Task DeleteBackward_DeletesCharacterToLeftOfCursor()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("abc");
        await Page.SetCursorAsync("Darki-2", 2);

        await Page.BackspaceAsync();
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("c");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    public async Task DeleteForward_DeletesCharacterToRightOfCursor()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("abc");
        await Page.SetCursorAsync("Darki-2", 1);

        await Page.DelAsync();
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("c");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    public async Task DeleteWordBackward_DeletesWordToLeftOfCursor()
    {
        const string text = "  abc  d";
        
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync(text);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync(text);
        await Page.SetCursorAsync("Darki-2", text.Length - 1);

        await Page.BackspaceAsync(true);
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("   ");
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("d");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(3));
        }
    }
    
    [Test]
    public async Task DeleteWordForward_DeletesWordToRightOfCursor()
    {
        const string text = "d  abc  ";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync(text);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync(text);
        await Page.SetCursorAsync("Darki-2", 1);

        await Page.DelAsync(true);
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("d");
        await Expect(Editor.Paragraph.TextSpan.Last).ToHaveTextAsync("   ");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteBackward_AtOnlyParagraph_DoesNoting(bool ctrl)
    {
        await Editor.ClickAsync();

        await Page.BackspaceAsync(ctrl);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-1"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteBackward_AtStartOfParagraph_DeletesParagraph(bool ctrl)
    {
        await Editor.ClickAsync();
        await Editor.PressAsync("Enter");

        await Page.BackspaceAsync(ctrl);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-1"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteBackward_AtStartOfParagraphWithText_DeletesParagraph(bool ctrl)
    {
        await Editor.ClickAsync();
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("a");
        await Page.SetCursorAsync("Darki-2", 0);

        await Page.BackspaceAsync(ctrl);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteForward_AtOnlyParagraph_DoesNoting(bool ctrl)
    {
        await Editor.ClickAsync();

        await Page.DelAsync(ctrl);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-1"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteForward_AtStartOfParagraph_DeletesParagraph(bool ctrl)
    {
        await Editor.ClickAsync();
        await Page.EnterAsync();
        await Page.SetCursorAsync("Darki-1", 0);

        await Page.DelAsync(ctrl);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-1"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteForward_AtStartOfParagraphWithText_DeletesParagraph(bool ctrl)
    {
        await Editor.ClickAsync();
        await Page.EnterAsync();
        await Page.SetCursorAsync("Darki-1", 0);
        await Page.TypeAtCurrentCursorAsync("a");

        await Page.BackspaceAsync(ctrl);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync("a");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    // TODO: test with selection
}