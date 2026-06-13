using DScratch.Client.BrowserInteractions.CommandHandlers;

namespace DScratch.Client.Pages.Editor.Components;

public partial class EditorMenu(IEditorCommandDispatcher editorCommandDispatcher)
{
    private string color = "#000000";
    
    private async Task BoldAsync()
    {
        
    }

    private async Task ItalicAsync()
    {
        
    }
    
    private async Task CodeAsync()
    {
        
    }

    private async Task BlockquoteAsync() 
    {
        
    }
    
    private async Task ParagraphAsync()
    {
        await editorCommandDispatcher.ChangeBlockTypeAsync(BlockType.Paragraph);
    }
    
    private async Task HeadingAsync(BlockType blockType)
    {
        await editorCommandDispatcher.ChangeBlockTypeAsync(blockType);
    }
    
    private async Task CodeBlockAsync()
    {
        
    }
    
    private async Task OnColorChangeAsync()
    {
        
    }
}