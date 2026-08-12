using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests.Links;

public class EditLinkTest : PlaywrightTestBase
{
    [Test]
    public async Task RemoveLink_RemovesLinkFromCurrentCursorPosition()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        
        await Expect(Page.Locator("#link-settings-popover")).Not.ToBeVisibleAsync();
         
        await EditorMenu.ClickAddLink();
        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Page.Locator("#add-link-popover .link-url").FillAsync("dscratch.darki.dev");
        await Page.Locator("#add-link-popover button").ClickAsync();
        
        await Expect(Page.Locator("#link-settings-popover")).ToBeVisibleAsync();
        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-6",
            AnchorOffset = 4,
            FocusId = "Darki-6",
            FocusOffset = 2
        });
        await Expect(Page.Locator("#link-settings-popover")).Not.ToBeVisibleAsync();
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-6",
            AnchorOffset = 3,
            FocusId = "Darki-6",
            FocusOffset = 3
        });
        
        await Expect(Editor.Paragraph.Link.TextSpan).ToHaveTextAsync("test");
        await Expect(Page.Locator("#link-settings-popover")).ToBeVisibleAsync();
        await Page.Locator("#link-settings-popover").Locator("button.remove-link").ClickAsync();
        
        await Expect(Editor.Paragraph).ToContainTextAsync("abctest");
        await Expect(Editor.Paragraph.Link).ToHaveCountAsync(0);
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(3));
        }
    }
}