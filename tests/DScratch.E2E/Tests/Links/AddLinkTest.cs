using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests.Links;

public class AddLinkTest : PlaywrightTestBase
{
    [Test]
    public async Task AddLink_SelectionDirectionNone_AddsLinkWithDisplayText()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");

        await EditorMenu.ClickAddLink();

        await Expect(Page.Locator("#add-link-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .display-text")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .link-url")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover button")).ToBeDisabledAsync();

        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Expect(Page.Locator("#add-link-popover button")).ToBeDisabledAsync();
        await Page.Locator("#add-link-popover .link-url").FillAsync("dscratch.darki.dev");
        await Expect(Page.Locator("#add-link-popover button")).Not.ToBeDisabledAsync();

        await Page.Locator("#add-link-popover button").ClickAsync();
        await Expect(Page.Locator("#add-link-popover")).Not.ToBeVisibleAsync();

        await Expect(Editor.Paragraph).ToContainTextAsync("abctest");
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("abc");
        await Expect(Editor.Paragraph.Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(Editor.Paragraph.Link.TextSpan).ToHaveTextAsync("test");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-4"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(4));
        }
    }
    
    [Test]
    public async Task AddLink_WithSelection_AddsLink()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-1",
            AnchorOffset = 1,
            FocusId = "Darki-1",
            FocusOffset = 3
        });
        
        await EditorMenu.ClickAddLink();

        await Expect(Page.Locator("#add-link-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .display-text")).Not.ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .link-url")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover button")).ToBeDisabledAsync();

        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Expect(Page.Locator("#add-link-popover button")).Not.ToBeDisabledAsync();

        await Page.Locator("#add-link-popover button").ClickAsync();
        await Expect(Page.Locator("#add-link-popover")).Not.ToBeVisibleAsync();

        await Expect(Editor.Paragraph).ToContainTextAsync("abc");
        await Expect(Editor.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(Editor.Paragraph.Link.TextSpan).ToHaveTextAsync("bc");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-2"));
            Assert.That(selection.FocusOffset, Is.EqualTo(2));
        }
    }
}