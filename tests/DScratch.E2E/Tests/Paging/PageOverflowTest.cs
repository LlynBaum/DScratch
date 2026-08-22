using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests.Paging;

public class PageOverflowTest : PlaywrightTestBase
{
    protected override bool EnableTracing => true;

    [Test]
    public async Task CreatesNewPage_WhenBlockOverflows_AndNextPageDoesNotExistYet()
    {
        await DefaultPage.ClickAsync();

        for (var i = 0; i < 14; i++)
        {
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
            await Expect(Editor.EditorPage.Paragraph).ToHaveCountAsync(i + 2);
        }

        await Page.EnterAsync();
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph).ToHaveCountAsync(15);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveCountAsync(1);
        
        var expectedSelection = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            FocusId = "Darki-15",
            FocusOffset = 0,
            AnchorId = "Darki-15",
            AnchorOffset = 0
        };
        
        await Page.SetSelectionAsync(expectedSelection);
        var actualSelection = await GetCursorPositionAsync();
        Assert.That(actualSelection, Is.EqualTo(expectedSelection));
    }
    
    [Test]
    public async Task CreatesNewPage_WhenTextOverflows_AndNextPageDoesNotExistYet()
    {
        const string firstPageText =
            "akljfajslöfkjalöskjfölkajsölkfjölkajölkjölkjfölkjaöljsöldjfölkajölskjdfölkjalkjlkjlkjdslkfjljkd";
        const string secondPageText = "kddjfkjdfjkdjfkdjfkj";
        const string overflowText = firstPageText + secondPageText;

        await DefaultPage.ClickAsync();

        for (var i = 0; i < 14; i++)
        {
            await Page.TypeAtCurrentCursorAsync(i.ToString());
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
            await Expect(Editor.EditorPage.Paragraph).ToHaveCountAsync(i + 2);
        }

        await Page.TypeAtCurrentCursorAsync(overflowText);
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(1).TextSpan.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(1).TextSpan.Last).ToHaveTextAsync(firstPageText);
        await Expect(Editor.EditorPage.Nth(2).TextSpan).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(2).TextSpan).ToHaveTextAsync(secondPageText);
        
        var expectedSelection = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            FocusId = "Darki-" + (28 + overflowText.Length),
            FocusOffset = overflowText.Length,
            AnchorId = "Darki-" + (28 + overflowText.Length),
            AnchorOffset = overflowText.Length
        };
        
        await Page.SetSelectionAsync(expectedSelection);
        var actualSelection = await GetCursorPositionAsync();
        Assert.That(actualSelection, Is.EqualTo(expectedSelection));
    }
}