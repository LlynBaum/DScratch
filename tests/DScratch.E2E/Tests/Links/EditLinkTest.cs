using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests.Links;

public class EditLinkTest : PlaywrightTestBase
{
    [Test]
    public async Task RemoveLink_RemovesLinkFromCurrentCursorPosition()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        
        await Expect(Page.Locator("#link-settings-popover")).Not.ToBeVisibleAsync();
         
        await EditorMenu.ClickAddLink();
        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Page.Locator("#add-link-popover .link-url").FillAsync("dscratch.darki.dev");
        await Page.Locator("#add-link-popover button").ClickAsync();
        
        await Expect(Page.Locator("#link-settings-popover")).ToBeVisibleAsync();
        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.Backward,
            AnchorId = "Darki-6",
            AnchorOffset = 4,
            FocusId = "Darki-6",
            FocusOffset = 2
        });
        await Expect(Page.Locator("#link-settings-popover")).Not.ToBeVisibleAsync();
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.None,
            AnchorId = "Darki-6",
            AnchorOffset = 3,
            FocusId = "Darki-6",
            FocusOffset = 3
        });
        
        await Expect(DefaultPage.Paragraph.Link.TextSpan).ToHaveTextAsync("test");
        await Expect(Page.Locator("#link-settings-popover")).ToBeVisibleAsync();
        await Page.Locator("#link-settings-popover").Locator("button.remove-link").ClickAsync();
        
        await Expect(DefaultPage.Paragraph).ToContainTextAsync("abctest");
        await Expect(DefaultPage.Paragraph.Link).ToHaveCountAsync(0);
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(3));
        }
    }
    
    [Test]
    public async Task UpdateHrefOnLink_ChangesHrefOnLinkElementInDocument()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        
        await Expect(Page.Locator("#link-settings-popover")).Not.ToBeVisibleAsync();
         
        await EditorMenu.ClickAddLink();
        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Page.Locator("#add-link-popover .link-url").FillAsync("https://dscratch.darki.dev");
        await Page.Locator("#add-link-popover button").ClickAsync();
        
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("href", "https://dscratch.darki.dev");
        
        await Expect(Page.Locator("#link-settings-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#link-settings-popover").Locator("input.link-url")).ToHaveValueAsync("https://dscratch.darki.dev/");
        await Page.Locator("#link-settings-popover").Locator("input.link-url").ClearAsync();
        await Page.Locator("#link-settings-popover").Locator("input.link-url").FillAsync("https://www.google.com");
        await DefaultPage.ClickAsync();
        
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("href", "https://www.google.com");
    }
    
    [Test]
    public async Task UpdateTargetOnLink_ChangesTargetOnLinkElementInDocument()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        
        await Expect(Page.Locator("#link-settings-popover")).Not.ToBeVisibleAsync();
         
        await EditorMenu.ClickAddLink();
        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Page.Locator("#add-link-popover .link-url").FillAsync("https://dscratch.darki.dev");
        await Page.Locator("#add-link-popover button").ClickAsync();
        
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("target", "_self");
        
        await Expect(Page.Locator("#link-settings-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#link-settings-popover").Locator("input.edit-link-target")).Not.ToBeCheckedAsync();
        await Page.Locator("#link-settings-popover").Locator("input.edit-link-target").CheckAsync();
        await Expect(Page.Locator("#link-settings-popover").Locator("input.edit-link-target")).ToBeCheckedAsync();
        
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("target", "_blank");
    }
}