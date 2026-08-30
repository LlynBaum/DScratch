using DScratch.E2E.Framework;
using DScratch.Interactions;

namespace DScratch.E2E.Tests.Paging;

public class PageOverflowTest : PlaywrightTestBase
{
    protected override bool EnableTracing => false;

    [Test]
    public async Task CreatesNewPage_WhenBlockOverflows_AndNextPageDoesNotExistYet()
    {
        await DefaultPage.ClickAsync();

        for (var i = 0; i < 28; i++)
        {
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
            await Expect(Editor.EditorPage.Paragraph).ToHaveCountAsync(i + 2);
        }

        await Page.EnterAsync();
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph).ToHaveCountAsync(29);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveCountAsync(1);
        
        var expectedSelection = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            FocusId = "Darki-30",
            FocusOffset = 0,
            AnchorId = "Darki-30",
            AnchorOffset = 0
        };
        
        await Page.SetSelectionAsync(expectedSelection);
        var actualSelection = await GetCursorPositionAsync();
        Assert.That(actualSelection, Is.EqualTo(expectedSelection));
    }
    
    [Test]
    public async Task MovesToNextPage_WhenBlockOverflows_AndNextPageExists()
    {
        await DefaultPage.ClickAsync();

        for (var i = 0; i < 29; i++)
        {
            await Page.EnterAsync();
        }

        await Page.ArrowUp();
        await Page.ArrowUp();
        await Page.EnterAsync();
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph).ToHaveCountAsync(29);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveCountAsync(2);
    }
    
    [Test]
    public async Task CreatesNewPage_WhenTextOverflows_AndNextPageDoesNotExistYet()
    {
        const string firstPageText =
            "jlökgfjslkfjasölkj dfölkaj sdlkfj aölksdj flök ajsdfölkj aölskdfj ölaksjdfölk jasöldkfj aölksjdf ölkajsödlfk jasölkdj fölkasj dfölkaj slök fjlköasdj flkasj dlfkjasölkdfj ölkasdjf lökasjdlökfjaslökd fjlökas jflkj öjlöj";
        const string secondPageText = " dwww";
        const string overflowText = firstPageText + secondPageText;

        await DefaultPage.ClickAsync();

        for (var i = 0; i < 28; i++)
        {
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
            await Expect(Editor.EditorPage.Paragraph).ToHaveCountAsync(i + 2);
        }

        await Page.TypeAtCurrentCursorAsync(overflowText);
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(0).TextSpan.Last).ToHaveTextAsync(firstPageText);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveTextAsync(secondPageText);
        
        var expectedSelection = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            FocusId = "Darki-76",
            FocusOffset = overflowText.Length,
            AnchorId = "Darki-76",
            AnchorOffset = overflowText.Length
        };
        
        await Page.SetSelectionAsync(expectedSelection);
        var actualSelection = await GetCursorPositionAsync();
        Assert.That(actualSelection, Is.EqualTo(expectedSelection));
    }
    
    [Test]
    public async Task MergesTextBackOnNextPage_WhenTextWasOverflows_AndIsMovedToSamePageAgain_WhileTyping()
    {
        const string firstPageText =
            "jlökgfjslkfjasölkj dfölkaj sdlkfj aölksdj flök ajsdfölkj aölskdfj ölaksjdfölk jasöldkfj aölksjdf ölkajsödlfk jasölkdj fölkasj dfölkaj slök fjlköasdj flkasj dlfkjasölkdfj ölkasdjf lökasjdlökfjaslökd fjlökas jflkj öjlöj";
        const string secondPageText = " dwww";
        const string overflowText = firstPageText + secondPageText;

        await DefaultPage.ClickAsync();

        for (var i = 0; i < 28; i++)
        {
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
            await Expect(Editor.EditorPage.Paragraph).ToHaveCountAsync(i + 2);
        }

        await Page.TypeAtCurrentCursorAsync(overflowText);
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(0).TextSpan.Last).ToHaveTextAsync(firstPageText);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveTextAsync(secondPageText);
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.None,
            FocusId = "Darki-76",
            FocusOffset = 0,
            AnchorId = "Darki-76",
            AnchorOffset = 0
        });
        await Page.TypeAtCurrentCursorAsync("abc");
        
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(0).TextSpan.Last).ToHaveTextAsync("abc" + firstPageText[..^6]);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveCountAsync(1);
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveTextAsync(firstPageText[^6..] + secondPageText);
    }
    
    [Test]
    public async Task MergesTextBackOnNextPage_WhenTextWasOverflows_AndIsMovedToSamePageAgain_WhenCreatingNewParagraph()
    {
        const string firstPageText =
            "jlökgfjslkfjasölkj dfölkaj sdlkfj aölksdj flök ajsdfölkj aölskdfj ölaksjdfölk jasöldkfj aölksjdf ölkajsödlfk jasölkdj fölkasj dfölkaj slök fjlköasdj flkasj dlfkjasölkdfj ölkasdjf lökasjdlökfjaslökd fjlökas jflkj öjlöj";
        const string secondPageText = " dwww";
        const string overflowText = firstPageText + secondPageText;

        await DefaultPage.ClickAsync();

        for (var i = 0; i < 28; i++)
        {
            await Page.EnterAsync();
            await Expect(Editor.EditorPage).ToHaveCountAsync(1);
            await Expect(Editor.EditorPage.Paragraph).ToHaveCountAsync(i + 2);
        }

        await Page.TypeAtCurrentCursorAsync(overflowText);
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(0).TextSpan.Last).ToHaveTextAsync(firstPageText);
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveTextAsync(secondPageText);
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            Direction = SelectionDirection.None,
            FocusId = "Darki-2",
            FocusOffset = 0,
            AnchorId = "Darki-2",
            AnchorOffset = 0
        });

        await Page.EnterAsync();
        
        await Expect(Editor.EditorPage).ToHaveCountAsync(2);
        await Expect(Editor.EditorPage.Nth(0).Paragraph.Last).ToHaveAttributeAsync("data-split-part", "1");
        await Expect(Editor.EditorPage.Nth(0).TextSpan.Last).ToBeEmptyAsync();
        await Expect(Editor.EditorPage.Nth(1).Paragraph).ToHaveAttributeAsync("data-split-part", "2");
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveCountAsync(1);
        await Expect(Editor.EditorPage.Nth(1).TextSpan).ToHaveTextAsync(overflowText);
    }
}