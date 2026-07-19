using Microsoft.Playwright;

namespace DScratch.E2E.Framework;

public class EditorMenuLocator(ILocator editorMenu)
{
    public ILocator Bold => editorMenu.GetByTitle("Bold");
    public ILocator Italic => editorMenu.GetByTitle("Italic");
    public ILocator Paragraph => editorMenu.GetByTitle("Paragraph");
    
    public async Task ClickBold()
    {
        await Bold.ClickAsync();
    }
    
    public async Task ClickItalic()
    {
        await Italic.ClickAsync();
    }
    
    public async Task ClickParagraph()
    {
        await Paragraph.ClickAsync();
    }
    
    public async Task ClickHeading(HeadingLevel level)
    {
        var headingName = level switch
        { 
            HeadingLevel.Level1 => "Heading 1",
            HeadingLevel.Level2 => "Heading 2",
            HeadingLevel.Level3 => "Heading 3",
            HeadingLevel.Level4 => "Heading 4",
            HeadingLevel.Level5 => "Heading 5",
            HeadingLevel.Level6 => "Heading 6",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };

        await editorMenu.GetByTitle(headingName).ClickAsync();
    }
}