using DScratch.Tests.E2E.Framework;

namespace DScratch.Tests.E2E.Tests;

public class SwitchBlockTypeTests : PlaywrightTestBase
{
    [Test]
    public async Task WriteText_IntoEmptyParagraph_AppendsCharactersAndAdvancesCursor()
    {
        // Click into the empty contenteditable area to focus it
        await Editor.ClickAsync();
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(text.Length));
        }
    }
}