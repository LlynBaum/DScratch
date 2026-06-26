using DScratch.Interactions;
using DScratch.Tests.E2E.Framework;

namespace DScratch.Tests.E2E.Tests;

public class SwitchBlockTypeTests : PlaywrightTestBase
{
    [Test]
    [TestCase(HeadingLevel.Level1)]
    [TestCase(HeadingLevel.Level2)]
    [TestCase(HeadingLevel.Level3)]
    [TestCase(HeadingLevel.Level4)]
    [TestCase(HeadingLevel.Level5)]
    [TestCase(HeadingLevel.Level6)]
    public async Task HeadingButtons_SwitchesCurrentBlockToExpectedHeading(HeadingLevel level)
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");

        await MenuBar.ClickHeading(level);

        await Expect(Editor.Paragraph).ToHaveCountAsync(0);
        await Expect(Editor.Heading(level)).ToHaveCountAsync(1);
        await Expect(Editor.Heading(level)).ToHaveTextAsync("a");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-3"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task HeadingButtons_SwitchesCurrentBlockToExpectedHeading()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("a");

        await MenuBar.ClickHeading(HeadingLevel.Level1);
        await MenuBar.ClickParagraph();

        await Expect(Editor.Heading(HeadingLevel.Level1)).ToHaveCountAsync(0);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        await Expect(Editor.Paragraph).ToHaveTextAsync("a");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-4"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    [Test]
    [TestCase(HeadingLevel.Level1)]
    [TestCase(HeadingLevel.Level2)]
    [TestCase(HeadingLevel.Level3)]
    [TestCase(HeadingLevel.Level4)]
    [TestCase(HeadingLevel.Level5)]
    [TestCase(HeadingLevel.Level6)]
    public async Task HeadingButtons_SwitchesCurrentBlockToExpectedHeading_WithSelection(HeadingLevel level)
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("ab");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("cd");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("ef");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-4",
            FocusOffset = 1
        });

        await MenuBar.ClickHeading(level);

        await Expect(Editor.Paragraph).ToHaveCountAsync(0);
        await Expect(Editor.Heading(level)).ToHaveCountAsync(3);
        await Expect(Editor.Heading(level).Nth(0)).ToHaveTextAsync("ab");
        await Expect(Editor.Heading(level).Nth(1)).ToHaveTextAsync("cd");
        await Expect(Editor.Heading(level).Nth(2)).ToHaveTextAsync("ef");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-4"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task HeadingButtons_SwitchesCurrentBlockToExpectedHeading_WithSelection()
    {
        await Editor.ClickAsync();
        await Page.TypeAtCurrentCursorAsync("ab");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("cd");
        await Page.EnterAsync();
        await Page.TypeAtCurrentCursorAsync("ef");

        await Page.SetSelectionAsync(new SelectionInfo
        {
            AnchorId = "Darki-2",
            AnchorOffset = 1,
            FocusId = "Darki-4",
            FocusOffset = 1
        });

        await MenuBar.ClickHeading(HeadingLevel.Level1);
        await MenuBar.ClickParagraph();

        await Expect(Editor.Heading(HeadingLevel.Level1)).ToHaveCountAsync(0);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);
        await Expect(Editor.Paragraph.Nth(0)).ToHaveTextAsync("ab");
        await Expect(Editor.Paragraph.Nth(1)).ToHaveTextAsync("cd");
        await Expect(Editor.Paragraph.Nth(2)).ToHaveTextAsync("ef");

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-2"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(1));
            Assert.That(selection.FocusId, Is.EqualTo("Darki-4"));
            Assert.That(selection.FocusOffset, Is.EqualTo(1));
        }
    }
}