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
        
        await EditorMenu.ClickBold();

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
        
        await EditorMenu.ClickItalic();

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
        
        await EditorMenu.ClickItalic();
        await EditorMenu.ClickBold();

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

    [Test]
    public async Task BoldButton_WhenSelectionHasMixedMarks()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abcd");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 0,
            FocusId = "Darki-2",
            FocusOffset = 2
        });
        await EditorMenu.ClickItalic();

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-6",
            AnchorOffset = 0,
            FocusId = "Darki-6",
            FocusOffset = 2
        });
        await EditorMenu.ClickBold();

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-6",
            FocusOffset = 1
        });
        await EditorMenu.ClickBold();
        
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("a");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-weight", "400");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("b");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-weight", "700");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("c");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-style", "normal");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-weight", "700");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(3)).ToHaveTextAsync("d");
        await Expect(Editor.Paragraph.TextSpan.Nth(3)).ToHaveCSSAsync("font-style", "normal");
        await Expect(Editor.Paragraph.TextSpan.Nth(3)).ToHaveCSSAsync("font-weight", "700");
        
        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-8"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-6"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    public async Task BoldButton_WhenSelectionOverTextWithMiddleHasMarks()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("abcdef");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-2",
            FocusOffset = 4
        });
        await EditorMenu.ClickBold();

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 0,
            FocusId = "Darki-12",
            FocusOffset = 2
        });
        await EditorMenu.ClickItalic();

        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("ab");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-weight", "400");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("cd");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-weight", "700");
        
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("ef");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-weight", "400");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-12"));
            Assert.That(selection.FocusOffset, Is.EqualTo(2));
        }
    }
    
    [Test]
    public async Task BoldButtons_SetsPendingMark()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("tet");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-2",
            FocusOffset = 2,
        });
        
        await Expect(EditorMenu.Bold).Not.ToContainClassAsync("active");
        await EditorMenu.ClickBold();
        await Expect(EditorMenu.Bold).ToContainClassAsync("active");
        
        await Page.TypeAtCurrentCursorAsync("s");

        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-weight", "400");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("s");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-weight", "700");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("t");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-weight", "400");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-6"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    public async Task ItalicButtons_SetsPendingMark()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("tet");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-2",
            FocusOffset = 2,
        });
        
        await Expect(EditorMenu.Italic).Not.ToContainClassAsync("active");
        await EditorMenu.ClickItalic();
        await Expect(EditorMenu.Italic).ToContainClassAsync("active");
        
        await Page.TypeAtCurrentCursorAsync("s");

        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-style", "normal");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("s");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("t");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-style", "normal");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-6"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
    
        [Test]
    public async Task BoldButtons_SetsPendingRemoval()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("tet");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 0,
            FocusId = "Darki-2",
            FocusOffset = 3,
        });
        await EditorMenu.ClickBold();
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-2",
            FocusOffset = 2,
        });
        
        await Expect(EditorMenu.Bold).ToContainClassAsync("active");
        await EditorMenu.ClickBold();
        await Expect(EditorMenu.Bold).Not.ToContainClassAsync("active");
        
        await Page.TypeAtCurrentCursorAsync("s");

        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-weight", "700");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("s");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-weight", "400");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("t");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-weight", "700");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-6"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
    
    [Test]
    public async Task ItalicButtons_SetsPendingRemoval()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("tet");
        
        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 0,
            FocusId = "Darki-2",
            FocusOffset = 3,
        });
        await EditorMenu.ClickItalic();

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 2,
            FocusId = "Darki-2",
            FocusOffset = 2,
        });
        
        await Expect(EditorMenu.Italic).ToContainClassAsync("active");
        await EditorMenu.ClickItalic();
        await Expect(EditorMenu.Italic).Not.ToContainClassAsync("active");
        
        await Page.TypeAtCurrentCursorAsync("s");

        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveTextAsync("te");
        await Expect(Editor.Paragraph.TextSpan.Nth(0)).ToHaveCSSAsync("font-style", "italic");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveTextAsync("s");
        await Expect(Editor.Paragraph.TextSpan.Nth(1)).ToHaveCSSAsync("font-style", "normal");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveTextAsync("t");
        await Expect(Editor.Paragraph.TextSpan.Nth(2)).ToHaveCSSAsync("font-style", "italic");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-6"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-6"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
}