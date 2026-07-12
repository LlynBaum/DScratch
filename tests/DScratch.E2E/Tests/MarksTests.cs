using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests;

public class MarksTests : PlaywrightTestBase
{
    [Test]
    public async Task BoldButtons_MakesSelectionBold()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("test");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("test");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-7",
            FocusOffset = 2,
        });
        
        await MenuBar.ClickBold();

        await Expect(Editor.Paragraph.First.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(0)).Not.ToHaveCSSAsync("font-weight", "700");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(1)).ToHaveTextAsync("st");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(1)).ToHaveCSSAsync("font-weight", "700");
        
        await Expect(Editor.Paragraph.First.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.Last.TextSpan.Nth(0)).ToHaveCSSAsync("font-weight", "700");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(1)).ToHaveTextAsync("st");
        await Expect(Editor.Paragraph.Last.TextSpan.Nth(1)).Not.ToHaveCSSAsync("font-weight", "700");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-11"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-7"));
            Assert.That(selection.FocusOffset, Is.EqualTo(2));
        }
    }
    
    [Test]
    public async Task ItalicButtons_MakesSelectionItalic()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("test");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("test");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-7",
            FocusOffset = 2,
        });
        
        await MenuBar.ClickItalic();

        await Expect(Editor.Paragraph.First.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(0)).Not.ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(1)).ToHaveTextAsync("st");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(1)).ToHaveCSSAsync("font-style", "italic");
        
        await Expect(Editor.Paragraph.First.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.Last.TextSpan.Nth(0)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.First.TextSpan.Nth(1)).ToHaveTextAsync("st");
        await Expect(Editor.Paragraph.Last.TextSpan.Nth(1)).Not.ToHaveCSSAsync("font-style", "italic");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-11"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-7"));
            Assert.That(selection.FocusOffset, Is.EqualTo(2));
        }
    }
    
    [Test]
    public async Task ItalicAndBold_ClickBothAndBothStylesAreApplied()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("test");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-2",
            FocusOffset = 3
        });
        
        await MenuBar.ClickItalic();
        await MenuBar.ClickBold();

        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("t");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).Not.ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).Not.ToHaveCSSAsync("font-weight", "700");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("es");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-weight", "700");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("t");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).Not.ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).Not.ToHaveCSSAsync("font-weight", "700");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-6"));
            Assert.That(selection.FocusOffset, Is.EqualTo(2));
        }
    }
}