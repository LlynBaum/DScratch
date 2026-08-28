using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests.Links;

public class AddLinkTest : PlaywrightTestBase
{
    [Test]
    public async Task AddLink_SelectionDirectionNone_AddsLinkWithDisplayText()
    {
        await DefaultPage.ClickAsync();
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
        await Expect(Page.Locator("#add-link-popover input.link-target")).Not.ToBeCheckedAsync();

        await Page.Locator("#add-link-popover button").ClickAsync();
        await Expect(Page.Locator("#add-link-popover")).Not.ToBeVisibleAsync();

        await Expect(DefaultPage.Paragraph).ToContainTextAsync("abctest");
        await Expect(DefaultPage.Paragraph.TextSpan.First).ToHaveTextAsync("abc");
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("target", "_self");
        await Expect(DefaultPage.Paragraph.Link.TextSpan).ToHaveTextAsync("test");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(4));
        }
    }
    
    [Test]
    public async Task AddLink_WithSelection_AddsLink()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.Forward,
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-2",
            FocusOffset = 3
        });
        
        await EditorMenu.ClickAddLink();

        await Expect(Page.Locator("#add-link-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .display-text")).Not.ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .link-url")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover button")).ToBeDisabledAsync();

        await Page.Locator("#add-link-popover .link-url").FillAsync("dscratch.darki.dev");
        await Expect(Page.Locator("#add-link-popover button")).Not.ToBeDisabledAsync();

        await Page.Locator("#add-link-popover button").ClickAsync();
        await Expect(Page.Locator("#add-link-popover")).Not.ToBeVisibleAsync();

        await Expect(DefaultPage.Paragraph.AllChildren).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph).ToContainTextAsync("abc");
        await Expect(DefaultPage.Paragraph.TextSpan.First).ToHaveTextAsync("a");
        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(DefaultPage.Paragraph.Link.TextSpan).ToHaveTextAsync("bc");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-5"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(2));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-5"));
            Assert.That(selection.FocusOffset, Is.EqualTo(2));
        }
    }
    
    [Test]
    public async Task AddLink_WithSelectionMultiline_AddsLink()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("def");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("ghi");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.Forward,
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-10",
            FocusOffset = 1
        });
        
        await EditorMenu.ClickAddLink();

        await Expect(Page.Locator("#add-link-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .display-text")).Not.ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover .link-url")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover button")).ToBeDisabledAsync();

        await Page.Locator("#add-link-popover .link-url").FillAsync("dscratch.darki.dev");
        await Expect(Page.Locator("#add-link-popover button")).Not.ToBeDisabledAsync();

        await Page.Locator("#add-link-popover button").ClickAsync();
        await Expect(Page.Locator("#add-link-popover")).Not.ToBeVisibleAsync();

        await Expect(DefaultPage.Paragraph).ToHaveCountAsync(3);
        await Expect(DefaultPage.Paragraph.Nth(0)).ToContainTextAsync("abc");
        await Expect(DefaultPage.Paragraph.Nth(1)).ToContainTextAsync("def");
        await Expect(DefaultPage.Paragraph.Nth(2)).ToContainTextAsync("ghi");

        await Expect(DefaultPage.Paragraph.Nth(0).AllChildren).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph.Nth(0).TextSpan.First).ToHaveTextAsync("a");
        await Expect(DefaultPage.Paragraph.Nth(0).Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(DefaultPage.Paragraph.Nth(0).Link.TextSpan).ToHaveTextAsync("bc");
        
        await Expect(DefaultPage.Paragraph.Nth(1).AllChildren).ToHaveCountAsync(1);
        await Expect(DefaultPage.Paragraph.Nth(1).Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(DefaultPage.Paragraph.Nth(1).Link.TextSpan).ToHaveTextAsync("def");
        
        await Expect(DefaultPage.Paragraph.Nth(2).AllChildren).ToHaveCountAsync(2);
        await Expect(DefaultPage.Paragraph.Nth(2).Link).ToHaveAttributeAsync("href", "dscratch.darki.dev");
        await Expect(DefaultPage.Paragraph.Nth(2).Link.TextSpan).ToHaveTextAsync("g");
        await Expect(DefaultPage.Paragraph.Nth(2).TextSpan.Last).ToHaveTextAsync("hi");
        
        var selection = await GetCursorPositionAsync();
        Assert.That(selection, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-10"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-10"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    public async Task AddLinkWithTargetNewTab_AddsLinkWithTargetBlank()
    {
        await DefaultPage.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abc");

        await EditorMenu.ClickAddLink();

        await Expect(Page.Locator("#add-link-popover")).ToBeVisibleAsync();
        await Expect(Page.Locator("#add-link-popover input.link-target")).ToBeVisibleAsync();

        await Page.Locator("#add-link-popover .display-text").FillAsync("test");
        await Page.Locator("#add-link-popover .link-url").FillAsync("dscratch.darki.dev");
        await Page.Locator("#add-link-popover input.link-target").CheckAsync();
        await Page.Locator("#add-link-popover button").ClickAsync();

        await Expect(DefaultPage.Paragraph.Link).ToHaveAttributeAsync("target", "_blank");
    }
}