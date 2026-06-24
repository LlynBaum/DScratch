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

        await MenuBar.ClickHeading(level);

        await Expect(Editor.Paragraph).ToHaveCountAsync(0);
        await Expect(Editor.Heading(level)).ToHaveCountAsync(1);

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

        await MenuBar.ClickHeading(HeadingLevel.Level1);
        await MenuBar.ClickParagraph();

        await Expect(Editor.Heading(HeadingLevel.Level1)).ToHaveCountAsync(0);
        await Expect(Editor.Paragraph).ToHaveCountAsync(1);

        var selection = await GetCursorPositionAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(selection.AnchorId, Is.EqualTo("Darki-4"));
            Assert.That(selection.AnchorOffset, Is.EqualTo(0));
        }
    }
    
    // TODO: test with selection
}