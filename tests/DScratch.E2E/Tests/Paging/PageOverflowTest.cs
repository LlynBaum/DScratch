using DScratch.E2E.Framework;

namespace DScratch.E2E.Tests.Paging;

public class PageOverflowTest : PlaywrightTestBase
{
    [Test]
    public async Task MovesTextToNewPage_WhenTextOverflowsOnPage()
    {
        const string firstPageText =
            "akljfajslöfkjalöskjfölkajsölkfjölkajölkjölkjfölkjaöljsöldjfölkajölskjdfölkjalkjlkjlkjdslkfjljkd";
        const string secondPageText = "kddjfkjdfjkdjfkdjfkj";
        const string overflowText = firstPageText + secondPageText;

        await DefaultPage.ClearAsync();

        for (var i = 0; i < 23; i++)
        {
            await Page.TypeAtCurrentCursorAsync(i.ToString());
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
        }

        await Page.TypeAtCurrentCursorAsync(overflowText);
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(1).TextSpan.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(1).TextSpan.Last).ToHaveTextAsync(firstPageText);
        await Expect(Editor.EditorPage.Nth(2).TextSpan).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(2).TextSpan).ToHaveTextAsync(secondPageText);
    }
}