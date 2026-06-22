using DScratch.Tests.E2E.Framework;

namespace DScratch.Tests.E2E;

public class InsertTextTests : PlaywrightTestBase
{
    [Test]
    public async Task WriteText_IntoEmptyParagraph_AppendsCharactersAndAdvancesCursor()
    {
        const string text = "DScratch";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();

        await Editor.Paragraph.FillAsync(text);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync(text);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }
    
    [Test]
    public async Task WriteText_IntoEmptyParagraph_AppendsCharactersAndAdvancesCursor_Sequentially()
    {
        const string text = "DScratch";
        
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();

        await Editor.Paragraph.PressSequentiallyAsync(text);
        await Expect(Editor.Paragraph.TextSpan).ToHaveTextAsync(text);
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }
}